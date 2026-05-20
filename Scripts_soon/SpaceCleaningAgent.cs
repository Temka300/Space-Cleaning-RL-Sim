using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SpaceCleaningAgent : Agent
{
    [Header("References")]
    public RocketMovement rocketMovement;
    public EnvironmentManager environmentManager;

    [Header("Rewards")]
    public float timePenalty = -0.0005f;
    public float collectReward = 1.5f;
    public float clearBonus = 5.0f;
    public float wallPenalty = -1.5f;
    public float planetPenalty = -1.5f;
    public float progressRewardScale = 0.05f;
    public float idlePenalty = -0.002f;
    public float idleSpeedThreshold = 0.15f;
    public float spinPenaltyScale = -0.001f;
    public float spinPenaltyThreshold = 0.5f;
    public float proximityRewardScale = 0f;
    public float straightApproachRewardScale = 0.006f;
    public float lateralSpeedPenaltyScale = -0.002f;
    public float wallProximityPenaltyScale = -0.001f;
    [Range(0.5f, 0.99f)] public float wallProximityThreshold = 0.80f;
    public float planetProximityPenaltyScale = -0.001f;
    public float planetProximityThreshold = 6f;

    [Header("Close-Range Precision")]
    public float precisionApproachThreshold = 3.0f;
    public float precisionApproachRewardScale = 0.01f;
    public float closeLateralPenaltyScale = -0.004f;

    [Header("Normalization")]
    public float maxObsSpeed = 8f;
    public float maxObsAngular = 2f;
    public float maxObsGravity = 5f;

    [Header("Training Metrics")]
    public bool recordTrainingMetrics = true;
    [Min(1)] public int metricSampleInterval = 25;

    private Rigidbody rb;
    private Collider bodyCollider;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float arenaHalfZ;
    private float arenaHalfX;
    private float diagonal;
    private Vector3 arenaCenter;
    private float prevNearestDist;
    private bool episodeEnding;
    private int debrisCollectedThisEpisode;
    private int idleStepsThisEpisode;
    private float closestDistanceThisEpisode;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<Collider>();
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
        SyncArenaCache();
    }

    public override void OnEpisodeBegin()
    {
        if (environmentManager != null)
        {
            environmentManager.PrepareEpisodeArena();
        }
        SyncArenaCache();

        Vector3 resetWorldStart =
            transform.parent != null ? transform.parent.TransformPoint(startPosition) : startPosition;
        if (environmentManager != null && !environmentManager.IsInsideArena(resetWorldStart, 1f))
        {
            resetWorldStart = environmentManager.ArenaCenterWorld;
            resetWorldStart.y =
                transform.parent != null ? transform.parent.TransformPoint(startPosition).y : startPosition.y;
        }

        rocketMovement.ResetRocket(
            resetWorldStart,
            startRotation);
        environmentManager.SpawnDebris(resetWorldStart);
        SyncArenaCache();

        prevNearestDist = DistanceToNearest();
        episodeEnding = false;
        debrisCollectedThisEpisode = 0;
        idleStepsThisEpisode = 0;
        closestDistanceThisEpisode = prevNearestDist > 0f ? prevNearestDist : float.PositiveInfinity;

        RecordEpisodeStartMetrics();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 pos = transform.position - arenaCenter;
        sensor.AddObservation(pos.x / Mathf.Max(1f, arenaHalfX));
        sensor.AddObservation(pos.z / Mathf.Max(1f, arenaHalfZ));

        Vector3 v =
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity;
#else
            rb.velocity;
#endif
        sensor.AddObservation(Mathf.Clamp(v.x / maxObsSpeed, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(v.z / maxObsSpeed, -1f, 1f));

        sensor.AddObservation(transform.forward.x);
        sensor.AddObservation(transform.forward.z);

        sensor.AddObservation(Mathf.Clamp(rb.angularVelocity.y / maxObsAngular, -1f, 1f));

        Transform nearest = environmentManager.GetNearestDebris(transform.position);
        if (nearest != null)
        {
            Vector3 to = nearest.position - transform.position;
            sensor.AddObservation(Mathf.Clamp(to.x / (2f * Mathf.Max(1f, arenaHalfX)), -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(to.z / (2f * Mathf.Max(1f, arenaHalfZ)), -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(to.magnitude / diagonal, 0f, 1f));
            Vector3 toFlat = new Vector3(to.x, 0f, to.z);
            Vector3 toN = toFlat.sqrMagnitude > 0.0001f ? toFlat.normalized : Vector3.zero;
            sensor.AddObservation(Vector3.Dot(toN, transform.forward));
            sensor.AddObservation(Vector3.Dot(toN, transform.right));

            Vector3 flatVelocity = new Vector3(v.x, 0f, v.z);
            float obsSpeed = Mathf.Max(0.001f, maxObsSpeed);
            float closingSpeed = Vector3.Dot(flatVelocity, toN);
            Vector3 targetRight = new Vector3(toN.z, 0f, -toN.x);
            float lateralSpeed = Vector3.Dot(flatVelocity, targetRight);
            sensor.AddObservation(Mathf.Clamp(closingSpeed / obsSpeed, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(lateralSpeed / obsSpeed, -1f, 1f));
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(1f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }

        float maxDebris = Mathf.Max(1, environmentManager.debrisCount);
        sensor.AddObservation(environmentManager.RemainingDebris / maxDebris);

        float stepRatio = MaxStep > 0 ? (float)StepCount / MaxStep : 0f;
        sensor.AddObservation(Mathf.Clamp01(stepRatio));

        float obsHalfArena = Mathf.Min(arenaHalfX, arenaHalfZ);
        if (environmentManager != null && obsHalfArena > 0.001f)
        {
            float wdMinX = transform.position.x - environmentManager.ArenaMinX;
            float wdMaxX = environmentManager.ArenaMaxX - transform.position.x;
            float wdMinZ = transform.position.z - environmentManager.ArenaMinZ;
            float wdMaxZ = environmentManager.ArenaMaxZ - transform.position.z;
            float minWD = Mathf.Min(Mathf.Min(wdMinX, wdMaxX), Mathf.Min(wdMinZ, wdMaxZ));

            float velToWall = 0f;
            if (minWD == wdMinX) velToWall = -v.x;
            else if (minWD == wdMaxX) velToWall = v.x;
            else if (minWD == wdMinZ) velToWall = -v.z;
            else velToWall = v.z;

            sensor.AddObservation(Mathf.Clamp01(minWD / obsHalfArena));
            sensor.AddObservation(Mathf.Clamp(velToWall / maxObsSpeed, -1f, 1f));
        }
        else
        {
            sensor.AddObservation(1f);
            sensor.AddObservation(0f);
        }

        Vector3 netGrav = Vector3.zero;
        Vector3 toStrongest = Vector3.zero;
        float strongestDistRatio = 0f;
        float strongestPullRatio = 0f;

        if (environmentManager != null && environmentManager.enableGravity)
        {
            netGrav = environmentManager.ComputeNetGravity(transform.position);
            float strongestPull;
            GravitySource strongest = environmentManager.FindStrongestSource(
                transform.position, out strongestPull);
            if (strongest != null)
            {
                Vector3 toS = strongest.transform.position - transform.position;
                toS.y = 0f;
                strongestDistRatio = Mathf.Clamp01(toS.magnitude / diagonal);
                toStrongest = toS;
                strongestPullRatio = Mathf.Clamp01(
                    strongestPull / Mathf.Max(0.001f, maxObsGravity));
            }
        }

        float gravNorm = Mathf.Max(0.001f, maxObsGravity);
        sensor.AddObservation(Mathf.Clamp(netGrav.x / gravNorm, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(netGrav.z / gravNorm, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(
            toStrongest.x / (2f * Mathf.Max(1f, arenaHalfX)), -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(
            toStrongest.z / (2f * Mathf.Max(1f, arenaHalfZ)), -1f, 1f));
        sensor.AddObservation(strongestDistRatio);
        sensor.AddObservation(strongestPullRatio);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float rawThrust = actions.ContinuousActions[0];
        float thrust = (rawThrust + 1f) * 0.5f;
        float rotation = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        rocketMovement.MoveRocket(thrust, rotation);

        AddRewardWithMetric(timePenalty, "Rewards/TimePenalty");

        Transform nearest = environmentManager.GetNearestDebris(transform.position);
        float currentDist = nearest != null
            ? Vector3.Distance(transform.position, nearest.position)
            : -1f;

        if (currentDist > 0f)
        {
            closestDistanceThisEpisode = Mathf.Min(closestDistanceThisEpisode, currentDist);
        }

        if (prevNearestDist > 0f && currentDist > 0f)
        {
            float delta = prevNearestDist - currentDist;
            AddRewardWithMetric(delta * progressRewardScale, "Rewards/Progress");
            RecordMetric("Target/ProgressDelta", delta);
        }
        prevNearestDist = currentDist;

        if (nearest != null)
        {
            Vector3 toTarget = nearest.position - transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Vector3 toTargetN = toTarget.normalized;
                float facing = Vector3.Dot(toTargetN, transform.forward);
                RecordMetric("Target/FacingDot", facing);

                Vector3 vel =
#if UNITY_6000_0_OR_NEWER
                    rb.linearVelocity;
#else
                    rb.velocity;
#endif
                Vector3 flatVelocity = new Vector3(vel.x, 0f, vel.z);
                float obsSpeed = Mathf.Max(0.001f, maxObsSpeed);
                float closingSpeed = Vector3.Dot(flatVelocity, toTargetN);
                float normalizedClosing = Mathf.Clamp01(closingSpeed / obsSpeed);
                RecordMetric("Target/ClosingSpeed", closingSpeed);

                Vector3 lateralVelocity = flatVelocity - toTargetN * closingSpeed;
                float lateralSpeed = lateralVelocity.magnitude;
                float normalizedLateral = Mathf.Clamp01(lateralSpeed / obsSpeed);
                AddRewardWithMetric(normalizedLateral * lateralSpeedPenaltyScale, "Rewards/LateralSpeedPenalty");
                RecordMetric("Target/LateralSpeed", lateralSpeed);

                float straightApproach = Mathf.Clamp01(facing) * normalizedClosing * (1f - normalizedLateral);
                AddRewardWithMetric(straightApproach * straightApproachRewardScale, "Rewards/StraightApproach");
                RecordMetric("Target/StraightApproach", straightApproach);

                if (currentDist > 0f && currentDist < precisionApproachThreshold)
                {
                    float proximityFactor = 1f - (currentDist / precisionApproachThreshold);

                    float precisionBonus = normalizedClosing * (1f - normalizedLateral) * proximityFactor;
                    AddRewardWithMetric(precisionBonus * precisionApproachRewardScale, "Rewards/PrecisionApproach");

                    AddRewardWithMetric(normalizedLateral * proximityFactor * closeLateralPenaltyScale, "Rewards/CloseLateralPenalty");

                    RecordMetric("Target/PrecisionProximityFactor", proximityFactor);
                }
            }
        }

        float angSpeed = Mathf.Abs(rb.angularVelocity.y);
        if (angSpeed > spinPenaltyThreshold)
        {
            AddRewardWithMetric(angSpeed * spinPenaltyScale, "Rewards/SpinPenalty");
            RecordMetric("Movement/SpinPenaltyEvents", 1f, StatAggregationMethod.Sum);
        }

        if (wallProximityPenaltyScale < 0f)
        {
            float dMinX = transform.position.x - environmentManager.ArenaMinX;
            float dMaxX = environmentManager.ArenaMaxX - transform.position.x;
            float dMinZ = transform.position.z - environmentManager.ArenaMinZ;
            float dMaxZ = environmentManager.ArenaMaxZ - transform.position.z;
            float minWallDist = Mathf.Min(Mathf.Min(dMinX, dMaxX), Mathf.Min(dMinZ, dMaxZ));
            float halfArena = Mathf.Min(arenaHalfX, arenaHalfZ);
            float thresholdDist = halfArena * (1f - wallProximityThreshold);
            if (thresholdDist > 0.001f && minWallDist < thresholdDist)
            {
                float penaltyStrength = 1f - (minWallDist / thresholdDist);

                Vector3 wallVel =
#if UNITY_6000_0_OR_NEWER
                    rb.linearVelocity;
#else
                    rb.velocity;
#endif
                float velTowardWall = 0f;
                if (minWallDist == dMinX) velTowardWall = -wallVel.x;
                else if (minWallDist == dMaxX) velTowardWall = wallVel.x;
                else if (minWallDist == dMinZ) velTowardWall = -wallVel.z;
                else velTowardWall = wallVel.z;

                if (velTowardWall > 0f)
                {
                    float velocityFactor = 1f + Mathf.Clamp01(velTowardWall / maxObsSpeed);
                    AddRewardWithMetric(penaltyStrength * velocityFactor * wallProximityPenaltyScale, "Rewards/WallProximity");
                }

                RecordMetric("Movement/VelocityTowardWall", velTowardWall);
                RecordMetric("Movement/MinWallDistance", minWallDist);
            }
        }

        if (planetProximityPenaltyScale < 0f
            && environmentManager != null
            && environmentManager.enableGravity)
        {
            float sPull;
            GravitySource strongest = environmentManager.FindStrongestSource(
                transform.position, out sPull);
            if (strongest != null)
            {
                Vector3 toPlanet = strongest.transform.position - transform.position;
                toPlanet.y = 0f;
                float distToPlanet = toPlanet.magnitude;

                if (distToPlanet < planetProximityThreshold && distToPlanet > 0.001f)
                {
                    float penaltyStrength = 1f - (distToPlanet / planetProximityThreshold);

                    Vector3 planetVel =
#if UNITY_6000_0_OR_NEWER
                        rb.linearVelocity;
#else
                        rb.velocity;
#endif
                    Vector3 toPlanetN = toPlanet / distToPlanet;
                    float velTowardPlanet = Vector3.Dot(
                        new Vector3(planetVel.x, 0f, planetVel.z), toPlanetN);

                    if (velTowardPlanet > 0f)
                    {
                        float velocityFactor = 1f + Mathf.Clamp01(velTowardPlanet / maxObsSpeed);
                        AddRewardWithMetric(
                            penaltyStrength * velocityFactor * planetProximityPenaltyScale,
                            "Rewards/PlanetProximity");
                    }

                    RecordMetric("Movement/VelocityTowardPlanet", velTowardPlanet);
                    RecordMetric("Movement/MinPlanetDistance", distToPlanet);
                }
            }
        }

        if (IsTouchingOrOutsideArena())
        {
            EndEpisodeWithWallPenalty();
            return;
        }

#if UNITY_6000_0_OR_NEWER
        float speed = rb.linearVelocity.magnitude;
#else
        float speed = rb.velocity.magnitude;
#endif
        if (speed < idleSpeedThreshold)
        {
            idleStepsThisEpisode++;
            AddRewardWithMetric(idlePenalty, "Rewards/IdlePenalty");
            RecordMetric("Movement/IdleSteps", 1f, StatAggregationMethod.Sum);
        }

        RecordStepMetrics(thrust, rotation, speed, angSpeed, currentDist);

        if (environmentManager.RemainingDebris <= 0)
        {
            if (episodeEnding) return;
            episodeEnding = true;
            AddRewardWithMetric(clearBonus, "Rewards/ClearBonus");
            RecordEpisodeEndMetrics(true, false);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;
        c[0] = Input.GetKey(KeyCode.W) ? 1f : -1f;
        c[1] = Input.GetAxis("Horizontal");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Debris"))
        {
            environmentManager.RemoveDebris(other.gameObject);
            debrisCollectedThisEpisode++;
            AddRewardWithMetric(collectReward, "Rewards/Collect");
            RecordMetric("Debris/Collected", 1f, StatAggregationMethod.Sum);
            RecordMetric("Debris/Remaining", environmentManager.RemainingDebris);
            prevNearestDist = DistanceToNearest();
        }
        else if (other.CompareTag("Boundary"))
        {
            EndEpisodeWithWallPenalty();
        }
        else if (environmentManager.planetContactKills
            && other.GetComponentInParent<GravitySource>() != null)
        {
            EndEpisodeWithPlanetPenalty();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;
        if (collision.collider.CompareTag("Boundary"))
        {
            EndEpisodeWithWallPenalty();
        }
        else if (environmentManager.planetContactKills
            && collision.collider.GetComponentInParent<GravitySource>() != null)
        {
            EndEpisodeWithPlanetPenalty();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider == null) return;
        if (collision.collider.CompareTag("Boundary"))
        {
            EndEpisodeWithWallPenalty();
        }
        else if (environmentManager.planetContactKills
            && collision.collider.GetComponentInParent<GravitySource>() != null)
        {
            EndEpisodeWithPlanetPenalty();
        }
    }

    private float DistanceToNearest()
    {
        Transform t = environmentManager.GetNearestDebris(transform.position);
        if (t == null) return -1f;
        return Vector3.Distance(transform.position, t.position);
    }

    private void AddRewardWithMetric(float reward, string metricName)
    {
        AddReward(reward);
        RecordMetric(metricName, reward, StatAggregationMethod.Sum);
    }

    private void RecordMetric(
        string metricName,
        float value,
        StatAggregationMethod aggregationMethod = StatAggregationMethod.Average)
    {
        if (!recordTrainingMetrics) return;
        Academy.Instance.StatsRecorder.Add(metricName, value, aggregationMethod);
    }

    private void RecordEpisodeStartMetrics()
    {
        if (environmentManager == null) return;

        RecordMetric("Environment/DebrisCount", environmentManager.debrisCount, StatAggregationMethod.MostRecent);
        RecordMetric("Environment/DriftingRatio", environmentManager.driftingRatio, StatAggregationMethod.MostRecent);
        RecordMetric("Environment/ArenaWidth", environmentManager.ArenaWidth, StatAggregationMethod.MostRecent);
        RecordMetric("Environment/ArenaDepth", environmentManager.ArenaDepth, StatAggregationMethod.MostRecent);
    }

    private void RecordStepMetrics(float thrust, float rotation, float speed, float angularSpeed, float nearestDistance)
    {
        int sampleInterval = Mathf.Max(1, metricSampleInterval);
        if (!recordTrainingMetrics || StepCount % sampleInterval != 0) return;

        RecordMetric("Action/Thrust", thrust);
        RecordMetric("Action/Rotation", rotation);
        RecordMetric("Action/AbsRotation", Mathf.Abs(rotation));
        RecordMetric("Movement/Speed", speed);
        RecordMetric("Movement/AngularSpeed", angularSpeed);

        if (environmentManager != null)
        {
            float maxDebris = Mathf.Max(1, environmentManager.debrisCount);
            RecordMetric("Debris/Remaining", environmentManager.RemainingDebris);
            RecordMetric("Debris/RemainingRatio", environmentManager.RemainingDebris / maxDebris);
        }

        if (nearestDistance > 0f)
        {
            RecordMetric("Target/NearestDebrisDistance", nearestDistance);
        }

        if (environmentManager != null && environmentManager.enableGravity)
        {
            Vector3 grav = environmentManager.ComputeNetGravity(transform.position);
            RecordMetric("Gravity/NetMagnitude", grav.magnitude);
            float sPull;
            GravitySource strongest = environmentManager.FindStrongestSource(
                transform.position, out sPull);
            if (strongest != null)
            {
                RecordMetric("Gravity/StrongestPull", sPull);
                Vector3 toS = strongest.transform.position - transform.position;
                toS.y = 0f;
                RecordMetric("Gravity/StrongestDistance", toS.magnitude);
            }
        }
    }

    private void RecordEpisodeEndMetrics(bool clearedAllDebris, bool hitWall, bool hitPlanet = false)
    {
        RecordMetric("Episode/Finished", 1f, StatAggregationMethod.Sum);
        RecordMetric("Episode/ClearRate", clearedAllDebris ? 1f : 0f);
        RecordMetric("Episode/WallHitRate", hitWall ? 1f : 0f);
        RecordMetric("Episode/PlanetHitRate", hitPlanet ? 1f : 0f);
        RecordMetric("Episode/Steps", StepCount);
        RecordMetric("Episode/StepRatio", MaxStep > 0 ? (float)StepCount / MaxStep : 0f);
        RecordMetric("Episode/IdleSteps", idleStepsThisEpisode);
        RecordMetric("Episode/IdleStepRatio", StepCount > 0 ? (float)idleStepsThisEpisode / StepCount : 0f);
        RecordMetric("Debris/CollectedPerEpisode", debrisCollectedThisEpisode);

        if (environmentManager != null)
        {
            float maxDebris = Mathf.Max(1, environmentManager.debrisCount);
            RecordMetric("Debris/RemainingAtEnd", environmentManager.RemainingDebris);
            RecordMetric("Debris/CollectedRatio", debrisCollectedThisEpisode / maxDebris);
        }

        if (!float.IsPositiveInfinity(closestDistanceThisEpisode))
        {
            RecordMetric("Target/ClosestDistanceEpisode", closestDistanceThisEpisode);
        }

        if (clearedAllDebris)
        {
            RecordMetric("Episode/ClearedAllDebris", 1f, StatAggregationMethod.Sum);
        }

        if (hitWall)
        {
            RecordMetric("Episode/WallHit", 1f, StatAggregationMethod.Sum);
        }

        if (hitPlanet)
        {
            RecordMetric("Episode/PlanetHit", 1f, StatAggregationMethod.Sum);
        }
    }

    private void SyncArenaCache()
    {
        if (environmentManager != null)
        {
            arenaHalfZ = environmentManager.arenaHalfSize;
            arenaHalfX = environmentManager.arenaHalfWidth;
            arenaCenter = environmentManager.ArenaCenterWorld;
        }
        else
        {
            arenaHalfZ = 20f;
            arenaHalfX = 20f;
            arenaCenter = Vector3.zero;
        }

        diagonal = Mathf.Sqrt((arenaHalfX * 2f) * (arenaHalfX * 2f) + (arenaHalfZ * 2f) * (arenaHalfZ * 2f));
    }

    private bool IsTouchingOrOutsideArena()
    {
        if (environmentManager == null) return false;
        if (bodyCollider == null) return !environmentManager.IsInsideArena(transform.position);

        Bounds bounds = bodyCollider.bounds;
        return bounds.min.x <= environmentManager.ArenaMinX
            || bounds.max.x >= environmentManager.ArenaMaxX
            || bounds.min.z <= environmentManager.ArenaMinZ
            || bounds.max.z >= environmentManager.ArenaMaxZ;
    }

    private void EndEpisodeWithWallPenalty()
    {
        if (episodeEnding) return;
        episodeEnding = true;
        AddRewardWithMetric(wallPenalty, "Rewards/WallPenalty");
        RecordEpisodeEndMetrics(false, true);
        EndEpisode();
    }

    private void EndEpisodeWithPlanetPenalty()
    {
        if (episodeEnding) return;
        episodeEnding = true;
        AddRewardWithMetric(planetPenalty, "Rewards/PlanetPenalty");
        RecordEpisodeEndMetrics(false, false, true);
        EndEpisode();
    }
}
