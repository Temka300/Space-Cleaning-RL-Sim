using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public enum PVPAgentBehaviorType
{
    MLAgents,
    RecordGameplay,
    TestReplay
}

[System.Serializable]
public class PVPReplayFrame
{
    public float time;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class PVPReplayData
{
    public string sceneName;
    public string agentName;
    public PVPReplayFrame[] frames;
}

public class PVPAgent : Agent
{
    [Header("References")]
    public RocketMovement rocketMovement;
    public PVPEnvironmentManager pvpEnvironment;

    [Header("Team")]
    [Tooltip("0 = Team A (spawns left/bottom), 1 = Team B (spawns right/top)")]
    public int team = 0;

    [Header("Behavior Type")]
    [Tooltip("MLAgents uses the normal Behavior Parameters. RecordGameplay saves this rocket's path. TestReplay follows the saved path.")]
    public PVPAgentBehaviorType behaviorType = PVPAgentBehaviorType.MLAgents;
    public string replayFileName = "";
    [Min(0.01f)] public float replaySampleInterval = 0.02f;
    public bool saveReplayOnStop = true;

    [Header("Rewards")]
    public float timePenalty = -0.0003f;
    public float collectReward = 1.5f;
    public float opponentCollectPenalty = -0.5f;
    public float winBonus = 3.0f;
    public float loseBonus = -1.0f;
    public float drawBonus = 0.2f;
    public float wallPenalty = -1.0f;
    public float planetHitPenalty = -0.8f;
    public float respawnPenalty = -0.3f;
    public float progressRewardScale = 0.03f;
    public float idlePenalty = -0.002f;
    public float idleSpeedThreshold = 0.15f;
    public float spinPenaltyScale = -0.001f;
    public float spinPenaltyThreshold = 0.5f;
    public float straightApproachRewardScale = 0.004f;
    public float lateralSpeedPenaltyScale = -0.001f;
    public float wallProximityPenaltyScale = -0.001f;
    [Range(0.5f, 0.99f)] public float wallProximityThreshold = 0.80f;
    public float planetProximityPenaltyScale = -0.001f;
    public float planetProximityThreshold = 6f;
    public float stealProximityBonus = 0.002f;
    public float stealProximityRange = 3f;
    public float refuelReward = 0.5f;

    [Header("Close-Range Precision")]
    public float precisionApproachThreshold = 3f;
    public float closeOverspeedPenaltyScale = -0.01f;

    [Header("Normalization")]
    public float maxObsSpeed = 8f;
    public float maxObsAngular = 2f;
    public float maxObsGravity = 5f;

    private Rigidbody rb;
    private Collider bodyCollider;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float arenaHalfZ, arenaHalfX, arenaHalfY, diagonal;
    private Vector3 arenaCenter;
    private float prevNearestDist;
    private bool episodeEnding;
    private int myCollected;
    private float invulnerabilityTimer;
    private int opponentCollectedLast;
    private readonly List<PVPReplayFrame> recordedFrames = new List<PVPReplayFrame>();
    private PVPReplayFrame[] playbackFrames;
    private float replayElapsed;
    private float replaySampleTimer;
    private int replayFrameIndex;
    private bool replayLoaded;
    private bool replaySaved;

    public Vector3 SpawnPosition => spawnPosition;
    public Quaternion SpawnRotation => spawnRotation;
    public bool IsRecordingReplay => behaviorType == PVPAgentBehaviorType.RecordGameplay;
    public bool IsTestReplay => behaviorType == PVPAgentBehaviorType.TestReplay;
    public bool UsesStableReplayLayout => IsRecordingReplay || IsTestReplay;
    private bool UsesThreeDimensionalArena => pvpEnvironment != null && pvpEnvironment.UseThreeDimensionalArena;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<Collider>();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        SyncArenaCache();
        pvpEnvironment.RegisterAgent(this, team);
    }

    public override void OnEpisodeBegin()
    {
        episodeEnding = false;
        myCollected = 0;
        invulnerabilityTimer = 0f;
        opponentCollectedLast = 0;

        if (team == 0)
        {
            pvpEnvironment.ResetMatch();
            rocketMovement.ResetRocket(spawnPosition, spawnRotation);
            pvpEnvironment.SpawnDebris();
        }
        else
        {
            rocketMovement.ResetRocket(spawnPosition, spawnRotation);
        }

        SyncArenaCache();
        prevNearestDist = DistanceToNearest();

        if (IsRecordingReplay)
            BeginReplayRecording();
        else if (IsTestReplay)
            BeginReplayPlayback();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        bool use3D = UsesThreeDimensionalArena;
        Vector3 pos = transform.position - arenaCenter;
        sensor.AddObservation(pos.x / Mathf.Max(1f, arenaHalfX));
        if (use3D)
            sensor.AddObservation(pos.y / Mathf.Max(1f, arenaHalfY));
        sensor.AddObservation(pos.z / Mathf.Max(1f, arenaHalfZ));

        Vector3 v = GetVelocity();
        sensor.AddObservation(Mathf.Clamp(v.x / maxObsSpeed, -1f, 1f));
        if (use3D)
            sensor.AddObservation(Mathf.Clamp(v.y / maxObsSpeed, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(v.z / maxObsSpeed, -1f, 1f));

        sensor.AddObservation(transform.forward.x);
        sensor.AddObservation(transform.forward.z);

        sensor.AddObservation(Mathf.Clamp(rb.angularVelocity.y / maxObsAngular, -1f, 1f));

        Transform nearest = pvpEnvironment.GetNearestDebris(transform.position);
        if (nearest != null)
        {
            Vector3 to = nearest.position - transform.position;
            sensor.AddObservation(Mathf.Clamp(to.x / (2f * Mathf.Max(1f, arenaHalfX)), -1f, 1f));
            if (use3D)
                sensor.AddObservation(Mathf.Clamp(to.y / (2f * Mathf.Max(1f, arenaHalfY)), -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(to.z / (2f * Mathf.Max(1f, arenaHalfZ)), -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(to.magnitude / diagonal, 0f, 1f));
            Vector3 toForSteering = use3D ? to : new Vector3(to.x, 0f, to.z);
            Vector3 toN = toForSteering.sqrMagnitude > 0.0001f ? toForSteering.normalized : Vector3.zero;
            sensor.AddObservation(Vector3.Dot(toN, transform.forward));
            sensor.AddObservation(Vector3.Dot(toN, transform.right));

            Vector3 movementV = use3D ? v : new Vector3(v.x, 0f, v.z);
            float closingSpeed = Vector3.Dot(movementV, toN);
            float lateralSpeed;
            if (use3D)
            {
                Vector3 lateralV = movementV - toN * closingSpeed;
                lateralSpeed = lateralV.magnitude;
            }
            else
            {
                Vector3 targetRight = new Vector3(toN.z, 0f, -toN.x);
                lateralSpeed = Vector3.Dot(movementV, targetRight);
            }
            sensor.AddObservation(Mathf.Clamp(closingSpeed / maxObsSpeed, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(lateralSpeed / maxObsSpeed, -1f, 1f));
        }
        else
        {
            int targetObs = use3D ? 8 : 7;
            for (int i = 0; i < targetObs; i++) sensor.AddObservation(0f);
        }

        float maxDebris = Mathf.Max(1, pvpEnvironment.debrisCount);
        sensor.AddObservation(pvpEnvironment.RemainingDebris / maxDebris);

        float stepRatio = MaxStep > 0 ? (float)StepCount / MaxStep : 0f;
        sensor.AddObservation(Mathf.Clamp01(stepRatio));

        PVPAgent opponent = pvpEnvironment.GetOpponent(this);
        if (opponent != null)
        {
            Vector3 toOpp = opponent.transform.position - transform.position;
            sensor.AddObservation(Mathf.Clamp(toOpp.x / (2f * Mathf.Max(1f, arenaHalfX)), -1f, 1f));
            if (use3D)
                sensor.AddObservation(Mathf.Clamp(toOpp.y / (2f * Mathf.Max(1f, arenaHalfY)), -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(toOpp.z / (2f * Mathf.Max(1f, arenaHalfZ)), -1f, 1f));

            Vector3 oppV = opponent.GetVelocity();
            sensor.AddObservation(Mathf.Clamp(oppV.x / maxObsSpeed, -1f, 1f));
            if (use3D)
                sensor.AddObservation(Mathf.Clamp(oppV.y / maxObsSpeed, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(oppV.z / maxObsSpeed, -1f, 1f));

            int myScore = pvpEnvironment.GetScore(team);
            int oppScore = pvpEnvironment.GetOpponentScore(team);
            float scoreDiff = (myScore - oppScore) / Mathf.Max(1f, maxDebris);
            sensor.AddObservation(Mathf.Clamp(scoreDiff, -1f, 1f));

            if (nearest != null)
            {
                float myDist = Vector3.Distance(transform.position, nearest.position);
                float oppDist = Vector3.Distance(opponent.transform.position, nearest.position);
                float distAdvantage = (oppDist - myDist) / diagonal;
                sensor.AddObservation(Mathf.Clamp(distAdvantage, -1f, 1f));
            }
            else
            {
                sensor.AddObservation(0f);
            }
        }
        else
        {
            int opponentObs = use3D ? 8 : 6;
            for (int i = 0; i < opponentObs; i++) sensor.AddObservation(0f);
        }

        float obsHalfArena = use3D ? Mathf.Min(Mathf.Min(arenaHalfX, arenaHalfY), arenaHalfZ) : Mathf.Min(arenaHalfX, arenaHalfZ);
        if (obsHalfArena > 0.001f)
        {
            GetNearestArenaBoundary(v, out float minWD, out float velToWall);
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

        if (pvpEnvironment.enableGravity)
        {
            netGrav = pvpEnvironment.ComputeNetGravity(transform.position);
            float sPull;
            GravitySource strongest = pvpEnvironment.FindStrongestSource(transform.position, out sPull);
            if (strongest != null)
            {
                Vector3 toS = strongest.transform.position - transform.position;
                if (!use3D)
                    toS.y = 0f;
                float surfaceDist = Mathf.Max(0f, toS.magnitude - strongest.minimumDistance);
                strongestDistRatio = Mathf.Clamp01(surfaceDist / diagonal);
                toStrongest = toS;
                strongestPullRatio = Mathf.Clamp01(sPull / Mathf.Max(0.001f, maxObsGravity));
            }
        }

        float gravNorm = Mathf.Max(0.001f, maxObsGravity);
        sensor.AddObservation(Mathf.Clamp(netGrav.x / gravNorm, -1f, 1f));
        if (use3D)
            sensor.AddObservation(Mathf.Clamp(netGrav.y / gravNorm, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(netGrav.z / gravNorm, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(toStrongest.x / (2f * Mathf.Max(1f, arenaHalfX)), -1f, 1f));
        if (use3D)
            sensor.AddObservation(Mathf.Clamp(toStrongest.y / (2f * Mathf.Max(1f, arenaHalfY)), -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(toStrongest.z / (2f * Mathf.Max(1f, arenaHalfZ)), -1f, 1f));
        sensor.AddObservation(strongestDistRatio);
        sensor.AddObservation(strongestPullRatio);

        sensor.AddObservation(invulnerabilityTimer > 0f ? 1f : 0f);

        // Fuel: current ratio + vector/distance to nearest canister.
        sensor.AddObservation(rocketMovement != null ? rocketMovement.FuelRatio : 1f);
        Transform nearestFuel = pvpEnvironment.GetNearestFuel(transform.position);
        if (nearestFuel != null)
        {
            Vector3 toFuel = nearestFuel.position - transform.position;
            sensor.AddObservation(Mathf.Clamp(toFuel.x / (2f * Mathf.Max(1f, arenaHalfX)), -1f, 1f));
            if (use3D)
                sensor.AddObservation(Mathf.Clamp(toFuel.y / (2f * Mathf.Max(1f, arenaHalfY)), -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(toFuel.z / (2f * Mathf.Max(1f, arenaHalfZ)), -1f, 1f));
            Vector3 toFuelVector = use3D ? toFuel : new Vector3(toFuel.x, 0f, toFuel.z);
            sensor.AddObservation(Mathf.Clamp01(toFuelVector.magnitude / diagonal));
        }
        else
        {
            sensor.AddObservation(0f);
            if (use3D)
                sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(1f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        bool use3D = UsesThreeDimensionalArena;

        if (invulnerabilityTimer > 0f && !IsTestReplay)
            invulnerabilityTimer -= Time.fixedDeltaTime;

        if (IsTestReplay)
        {
            if (rocketMovement != null)
                rocketMovement.MoveRocket(0f, 0f, 0f);
            return;
        }

        float rawThrust;
        float rotation;
        float vertical;
        if (IsRecordingReplay)
        {
            GetManualControls(out rawThrust, out rotation, out vertical);
        }
        else
        {
            rawThrust = actions.ContinuousActions[0];
            rotation = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
            vertical = use3D && actions.ContinuousActions.Length > 2
                ? Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f)
                : 0f;
        }

        float thrust = (rawThrust + 1f) * 0.5f;
        Transform nearest = pvpEnvironment.GetNearestDebris(transform.position);
        if (rocketMovement != null)
            rocketMovement.SetAutoHeightTarget(nearest);
        rocketMovement.MoveRocket(thrust, rotation, use3D ? vertical : 0f);

        AddReward(timePenalty);

        int currentOppScore = pvpEnvironment.GetOpponentScore(team);
        if (currentOppScore > opponentCollectedLast)
        {
            int oppNewCollections = currentOppScore - opponentCollectedLast;
            AddReward(opponentCollectPenalty * oppNewCollections);
            opponentCollectedLast = currentOppScore;
        }

        float currentDist = nearest != null
            ? Vector3.Distance(transform.position, nearest.position) : -1f;

        if (prevNearestDist > 0f && currentDist > 0f)
        {
            float delta = prevNearestDist - currentDist;
            AddReward(delta * progressRewardScale);
        }
        prevNearestDist = currentDist;

        if (nearest != null)
        {
            Vector3 toTarget = nearest.position - transform.position;
            if (!use3D)
                toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Vector3 toTargetN = toTarget.normalized;
                Vector3 vel = GetVelocity();
                Vector3 movementV = use3D ? vel : new Vector3(vel.x, 0f, vel.z);
                float closingSpeed = Vector3.Dot(movementV, toTargetN);
                float normalizedClosing = Mathf.Clamp01(closingSpeed / maxObsSpeed);
                Vector3 lateralV = movementV - toTargetN * closingSpeed;
                float normalizedLateral = Mathf.Clamp01(lateralV.magnitude / maxObsSpeed);
                float facing = Mathf.Clamp01(Vector3.Dot(toTargetN, transform.forward));

                float straightApproach = facing * normalizedClosing * (1f - normalizedLateral);
                AddReward(straightApproach * straightApproachRewardScale);
                AddReward(normalizedLateral * lateralSpeedPenaltyScale);

                // Brake near the target: penalize speed above a distance-scaled allowance (-> 0 at contact).
                if (currentDist > 0f && currentDist < precisionApproachThreshold)
                {
                    float proximityFactor = 1f - (currentDist / precisionApproachThreshold);
                    float allowedSpeed = maxObsSpeed * (currentDist / precisionApproachThreshold);
                    float overspeed = Mathf.Clamp01(Mathf.Max(0f, movementV.magnitude - allowedSpeed) / Mathf.Max(0.001f, maxObsSpeed));
                    AddReward(overspeed * proximityFactor * closeOverspeedPenaltyScale);
                }

                PVPAgent opponent = pvpEnvironment.GetOpponent(this);
                if (opponent != null && currentDist > 0f && currentDist < stealProximityRange)
                {
                    float oppDist = Vector3.Distance(opponent.transform.position, nearest.position);
                    if (oppDist < currentDist && oppDist < stealProximityRange)
                    {
                        AddReward(stealProximityBonus * (1f - currentDist / stealProximityRange));
                    }
                }
            }
        }

        float angSpeed = Mathf.Abs(rb.angularVelocity.y);
        if (angSpeed > spinPenaltyThreshold)
            AddReward(angSpeed * spinPenaltyScale);

        if (wallProximityPenaltyScale < 0f)
        {
            GetNearestArenaBoundary(GetVelocity(), out float minWallDist, out float velTowardWall);
            float halfArena = use3D ? Mathf.Min(Mathf.Min(arenaHalfX, arenaHalfY), arenaHalfZ) : Mathf.Min(arenaHalfX, arenaHalfZ);
            float thresholdDist = halfArena * (1f - wallProximityThreshold);
            if (thresholdDist > 0.001f && minWallDist < thresholdDist)
            {
                float penaltyStr = 1f - (minWallDist / thresholdDist);
                if (velTowardWall > 0f)
                {
                    float velocityFactor = 1f + Mathf.Clamp01(velTowardWall / maxObsSpeed);
                    AddReward(penaltyStr * velocityFactor * wallProximityPenaltyScale);
                }
            }
        }

        if (planetProximityPenaltyScale < 0f && pvpEnvironment.enableGravity)
        {
            float sPull;
            GravitySource strongest = pvpEnvironment.FindStrongestSource(transform.position, out sPull);
            if (strongest != null)
            {
                Vector3 toPlanet = strongest.transform.position - transform.position;
                if (!use3D)
                    toPlanet.y = 0f;
                float distToPlanet = toPlanet.magnitude;
                float surfaceDist = distToPlanet - strongest.minimumDistance;
                if (surfaceDist < planetProximityThreshold && distToPlanet > 0.001f)
                {
                    float penaltyStr = 1f - (Mathf.Max(0f, surfaceDist) / planetProximityThreshold);
                    Vector3 planetVel = GetVelocity();
                    Vector3 toPlanetN = toPlanet / distToPlanet;
                    Vector3 movementV = use3D ? planetVel : new Vector3(planetVel.x, 0f, planetVel.z);
                    float velToward = Vector3.Dot(movementV, toPlanetN);
                    if (velToward > 0f)
                    {
                        float velocityFactor = 1f + Mathf.Clamp01(velToward / maxObsSpeed);
                        AddReward(penaltyStr * velocityFactor * planetProximityPenaltyScale);
                    }
                }
            }
        }

        if (IsTouchingOrOutsideArena())
        {
            HandleWallHit();
            return;
        }

        float speed = GetVelocity().magnitude;
        bool nearTargetForIdle = currentDist > 0f && currentDist < precisionApproachThreshold;
        if (speed < idleSpeedThreshold && !nearTargetForIdle)
            AddReward(idlePenalty);

        if (pvpEnvironment.RemainingDebris <= 0)
        {
            if (episodeEnding) return;
            episodeEnding = true;
            EndMatchWithScores();
        }
    }

    void FixedUpdate()
    {
        if (IsRecordingReplay)
        {
            RecordReplayFrame(false);
            return;
        }

        if (!IsTestReplay) return;

        if (invulnerabilityTimer > 0f)
            invulnerabilityTimer -= Time.fixedDeltaTime;

        StepReplayPlayback();

        if (IsTouchingOrOutsideArena())
        {
            HandleWallHit();
            return;
        }

        if (pvpEnvironment.RemainingDebris <= 0 && !episodeEnding)
        {
            episodeEnding = true;
            EndMatchWithScores();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;
        GetManualControls(out float thrust, out float rotation, out float vertical);
        c[0] = thrust;
        c[1] = rotation;
        if (c.Length > 2)
            c[2] = vertical;
    }

    private void GetManualControls(out float thrust, out float rotation, out float vertical)
    {
        if (team == 0)
        {
            // Team A: WASD, Q/E for vertical movement in 3D scenes.
            thrust = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : -1f);
            rotation = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
            vertical = (Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f);
        }
        else
        {
            // Team B: Arrow keys, PageUp/PageDown for vertical movement in 3D scenes.
            thrust = Input.GetKey(KeyCode.UpArrow) ? 1f : (Input.GetKey(KeyCode.DownArrow) ? -1f : -1f);
            rotation = (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
            vertical = (Input.GetKey(KeyCode.PageUp) ? 1f : 0f) - (Input.GetKey(KeyCode.PageDown) ? 1f : 0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (invulnerabilityTimer > 0f && other.GetComponentInParent<GravitySource>() != null)
            return;

        if (other.CompareTag("Debris"))
        {
            pvpEnvironment.CollectDebris(other.gameObject, this);
            myCollected++;
            AddReward(collectReward);
            prevNearestDist = DistanceToNearest();
        }
        else if (other.CompareTag("Fuel"))
        {
            float before = rocketMovement != null ? rocketMovement.FuelRatio : 1f;
            if (pvpEnvironment.RefuelFromCanister(other.gameObject, this))
                AddReward(refuelReward * (1f - before));
        }
        else if (other.CompareTag("Boundary"))
        {
            HandleWallHit();
        }
        else if (pvpEnvironment.planetContactKills
            && other.GetComponentInParent<GravitySource>() != null)
        {
            HandlePlanetHit();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;
        if (invulnerabilityTimer > 0f
            && collision.collider.GetComponentInParent<GravitySource>() != null)
            return;

        if (collision.collider.CompareTag("Boundary"))
            HandleWallHit();
        else if (pvpEnvironment.planetContactKills
            && collision.collider.GetComponentInParent<GravitySource>() != null)
            HandlePlanetHit();
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider == null) return;
        if (collision.collider.CompareTag("Boundary"))
            HandleWallHit();
    }

    private void HandleWallHit()
    {
        if (episodeEnding) return;
        AddReward(wallPenalty);
        AddReward(respawnPenalty);
        pvpEnvironment.RespawnAgent(this);
        invulnerabilityTimer = pvpEnvironment.respawnInvulnerabilityTime;
        prevNearestDist = DistanceToNearest();
    }

    private void HandlePlanetHit()
    {
        if (episodeEnding) return;
        AddReward(planetHitPenalty);
        AddReward(respawnPenalty);
        pvpEnvironment.RespawnAgent(this);
        invulnerabilityTimer = pvpEnvironment.respawnInvulnerabilityTime;
        prevNearestDist = DistanceToNearest();
    }

    public void OnRespawned()
    {
        if (IsTestReplay)
            RestartReplayPlayback();
        else if (IsRecordingReplay)
            RecordReplayFrame(true);
    }

    private void BeginReplayRecording()
    {
        recordedFrames.Clear();
        replayElapsed = 0f;
        replaySampleTimer = 0f;
        replaySaved = false;
        RecordReplayFrame(true);
    }

    private void BeginReplayPlayback()
    {
        replayElapsed = 0f;
        replayFrameIndex = 0;
        if (!replayLoaded || playbackFrames == null || playbackFrames.Length == 0)
            LoadReplay();

        ApplyReplayFrame(0);
    }

    private void RestartReplayPlayback()
    {
        replayElapsed = 0f;
        replayFrameIndex = 0;
        ApplyReplayFrame(0);
    }

    private void RecordReplayFrame(bool force)
    {
        if (rb == null) return;

        replayElapsed += Time.fixedDeltaTime;
        replaySampleTimer += Time.fixedDeltaTime;
        if (!force && replaySampleTimer < replaySampleInterval) return;
        replaySampleTimer = 0f;

        recordedFrames.Add(new PVPReplayFrame
        {
            time = replayElapsed,
            position = transform.position,
            rotation = transform.rotation
        });
    }

    private void StepReplayPlayback()
    {
        if (playbackFrames == null || playbackFrames.Length == 0) return;

        replayElapsed += Time.fixedDeltaTime;
        while (replayFrameIndex < playbackFrames.Length - 1
            && playbackFrames[replayFrameIndex + 1].time <= replayElapsed)
        {
            replayFrameIndex++;
        }

        ApplyInterpolatedReplayFrame();
    }

    private void ApplyInterpolatedReplayFrame()
    {
        if (playbackFrames == null || playbackFrames.Length == 0) return;

        PVPReplayFrame current = playbackFrames[Mathf.Clamp(replayFrameIndex, 0, playbackFrames.Length - 1)];
        if (replayFrameIndex >= playbackFrames.Length - 1)
        {
            ApplyReplayFrame(current);
            return;
        }

        PVPReplayFrame next = playbackFrames[replayFrameIndex + 1];
        float duration = Mathf.Max(0.0001f, next.time - current.time);
        float t = Mathf.Clamp01((replayElapsed - current.time) / duration);

        ApplyReplayPose(
            Vector3.Lerp(current.position, next.position, t),
            Quaternion.Slerp(current.rotation, next.rotation, t));
    }

    private void ApplyReplayFrame(int index)
    {
        if (playbackFrames == null || playbackFrames.Length == 0) return;
        ApplyReplayFrame(playbackFrames[Mathf.Clamp(index, 0, playbackFrames.Length - 1)]);
    }

    private void ApplyReplayFrame(PVPReplayFrame frame)
    {
        if (frame == null) return;
        ApplyReplayPose(frame.position, frame.rotation);
    }

    private void ApplyReplayPose(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);

        if (rb != null)
        {
            rb.position = position;
            rb.rotation = rotation;
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
        }

        if (rocketMovement != null)
            rocketMovement.MoveRocket(0f, 0f, 0f);
    }

    private void LoadReplay()
    {
        replayLoaded = true;
        string path = GetReplayFilePath();
        if (!File.Exists(path))
        {
            playbackFrames = null;
            Debug.LogWarning($"Replay file not found for {name}: {path}", this);
            return;
        }

        try
        {
            PVPReplayData data = JsonUtility.FromJson<PVPReplayData>(File.ReadAllText(path));
            playbackFrames = data != null ? data.frames : null;
            if (playbackFrames == null || playbackFrames.Length == 0)
                Debug.LogWarning($"Replay file has no frames for {name}: {path}", this);
        }
        catch (System.Exception ex)
        {
            playbackFrames = null;
            Debug.LogWarning($"Could not load replay for {name}: {ex.Message}", this);
        }
    }

    private void SaveReplay()
    {
        if (!IsRecordingReplay || replaySaved || recordedFrames.Count == 0) return;

        try
        {
            string path = GetReplayFilePath();
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            PVPReplayData data = new PVPReplayData
            {
                sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                agentName = name,
                frames = recordedFrames.ToArray()
            };
            File.WriteAllText(path, JsonUtility.ToJson(data, true));
            replaySaved = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Could not save replay for {name}: {ex.Message}", this);
        }
    }

    private string GetReplayFilePath()
    {
        string dir = Path.Combine(Application.persistentDataPath, "PVPReplays");
        string fileName = string.IsNullOrWhiteSpace(replayFileName)
            ? name + "_Replay"
            : replayFileName;
        foreach (char invalid in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(invalid, '_');
        return Path.Combine(dir, fileName + ".json");
    }

    void OnDestroy()
    {
        if (saveReplayOnStop)
            SaveReplay();
    }

    void OnApplicationQuit()
    {
        SaveReplay();
    }

    private void EndMatchWithScores()
    {
        int myScore = pvpEnvironment.GetScore(team);
        int oppScore = pvpEnvironment.GetOpponentScore(team);

        if (myScore > oppScore)
            AddReward(winBonus);
        else if (myScore < oppScore)
            AddReward(loseBonus);
        else
            AddReward(drawBonus);

        EndEpisode();
    }

    private float DistanceToNearest()
    {
        Transform t = pvpEnvironment.GetNearestDebris(transform.position);
        if (t == null) return -1f;
        return Vector3.Distance(transform.position, t.position);
    }

    public Vector3 GetVelocity()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    private void GetNearestArenaBoundary(Vector3 velocity, out float minDistance, out float velocityTowardBoundary)
    {
        float dMinX = transform.position.x - pvpEnvironment.ArenaMinX;
        float dMaxX = pvpEnvironment.ArenaMaxX - transform.position.x;
        float dMinZ = transform.position.z - pvpEnvironment.ArenaMinZ;
        float dMaxZ = pvpEnvironment.ArenaMaxZ - transform.position.z;

        minDistance = dMinX;
        velocityTowardBoundary = -velocity.x;

        if (dMaxX < minDistance)
        {
            minDistance = dMaxX;
            velocityTowardBoundary = velocity.x;
        }

        if (dMinZ < minDistance)
        {
            minDistance = dMinZ;
            velocityTowardBoundary = -velocity.z;
        }

        if (dMaxZ < minDistance)
        {
            minDistance = dMaxZ;
            velocityTowardBoundary = velocity.z;
        }

        if (!UsesThreeDimensionalArena) return;

        float dMinY = transform.position.y - pvpEnvironment.ArenaMinY;
        float dMaxY = pvpEnvironment.ArenaMaxY - transform.position.y;
        if (dMinY < minDistance)
        {
            minDistance = dMinY;
            velocityTowardBoundary = -velocity.y;
        }

        if (dMaxY < minDistance)
        {
            minDistance = dMaxY;
            velocityTowardBoundary = velocity.y;
        }
    }

    private bool IsTouchingOrOutsideArena()
    {
        if (bodyCollider == null)
            return !pvpEnvironment.IsInsideArena(transform.position);
        Bounds bounds = bodyCollider.bounds;
        bool outside = bounds.min.x <= pvpEnvironment.ArenaMinX
            || bounds.max.x >= pvpEnvironment.ArenaMaxX
            || bounds.min.z <= pvpEnvironment.ArenaMinZ
            || bounds.max.z >= pvpEnvironment.ArenaMaxZ;

        if (UsesThreeDimensionalArena)
        {
            outside = outside
                || bounds.min.y <= pvpEnvironment.ArenaMinY
                || bounds.max.y >= pvpEnvironment.ArenaMaxY;
        }

        return outside;
    }

    private void SyncArenaCache()
    {
        arenaHalfZ = pvpEnvironment.arenaHalfSize;
        arenaHalfX = pvpEnvironment.arenaHalfWidth;
        arenaHalfY = pvpEnvironment.UseThreeDimensionalArena
            ? Mathf.Max(1f, pvpEnvironment.ArenaHeight * 0.5f)
            : 1f;
        arenaCenter = pvpEnvironment.ArenaCenterWorld;
        float width = arenaHalfX * 2f;
        float height = pvpEnvironment.UseThreeDimensionalArena ? arenaHalfY * 2f : 0f;
        float depth = arenaHalfZ * 2f;
        diagonal = Mathf.Sqrt(width * width + height * height + depth * depth);
    }
}
