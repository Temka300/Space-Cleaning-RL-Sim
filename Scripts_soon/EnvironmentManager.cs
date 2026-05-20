using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Debris Settings")]
    public GameObject debrisPrefab;
    public int debrisCount = 10;
    [Range(0f, 1f)]
    public float driftingRatio = 0.3f;
    public float maxDriftSpeed = 2f;
    [Tooltip("Inset from arena edges where debris will not spawn.")]
    public float spawnMargin = 1.5f;

    [Header("Arena Settings")]
    public bool matchCameraView = true;
    public float arenaHalfSize = 9f;
    public float arenaHalfWidth = 9f;
    [Tooltip("Margin inset from screen edge.")]
    public float screenMargin = 1f;

    [Header("Training Overrides")]
    public bool useAcademyEnvironmentParameters = true;
    public int curriculumMinDebrisCount = 1;
    [Range(0f, 1f)] public float curriculumMinDriftingRatio = 0f;
    public float curriculumMinMaxDriftSpeed = 0.5f;
    public float curriculumMinArenaHalfSize = 10f;
    public float curriculumMinArenaHalfWidth = 16f;

    [Header("Gravity")]
    public bool enableGravity = false;
    public float gravityConstant = 1f;
    public bool planetContactKills = true;
    public Rigidbody playerRigidbody;
    public List<GravitySource> gravitySources = new List<GravitySource>();

    [Header("Boundary Source")]
    [Tooltip("If assigned, only colliders under this transform are used for arena logic.")]
    public Transform boundaryRoot;

    private List<SpaceDebris> activeDebris = new List<SpaceDebris>();
    private readonly List<Collider> boundaryColliders = new List<Collider>();
    private RocketMovement playerMovement;
    private int defaultDebrisCount;
    private float defaultDriftingRatio;
    private float defaultMaxDriftSpeed;
    private bool defaultMatchCameraView;
    private float defaultArenaHalfSize;
    private float defaultArenaHalfWidth;
    private bool defaultsCaptured;
    private float arenaMinX;
    private float arenaMaxX;
    private float arenaMinZ;
    private float arenaMaxZ;

    public int RemainingDebris => activeDebris.Count;
    public float ArenaMinX => arenaMinX;
    public float ArenaMaxX => arenaMaxX;
    public float ArenaMinZ => arenaMinZ;
    public float ArenaMaxZ => arenaMaxZ;
    public float ArenaWidth => Mathf.Max(0f, arenaMaxX - arenaMinX);
    public float ArenaDepth => Mathf.Max(0f, arenaMaxZ - arenaMinZ);
    public Vector3 ArenaCenterWorld =>
        new Vector3((arenaMinX + arenaMaxX) * 0.5f, transform.position.y, (arenaMinZ + arenaMaxZ) * 0.5f);

    void Awake()
    {
        CaptureDefaults();
        AutoAssignBoundaryRoot();
        RefreshBoundaryColliders();
        AutoDiscoverGravitySources();
        if (playerRigidbody != null)
            playerMovement = playerRigidbody.GetComponent<RocketMovement>();
    }

    void Start()
    {
        PrepareEpisodeArena();
    }

    void FixedUpdate()
    {
        if (!enableGravity || gravitySources.Count == 0) return;

        if (playerMovement != null)
        {
            Vector3 accel = ComputeNetGravity(playerRigidbody.position);
            playerMovement.SetExternalAcceleration(accel);
        }

        foreach (var sd in activeDebris)
        {
            if (sd == null || sd.Rb == null || sd.Rb.isKinematic) continue;
            Vector3 accel = ComputeNetGravity(sd.transform.position);
            sd.Rb.AddForce(accel, ForceMode.Acceleration);
        }
    }

    public void PrepareEpisodeArena()
    {
        ApplyEpisodeSettings();
    }

    public void SpawnDebris()
    {
        SpawnDebris(ArenaCenterWorld);
    }

    public void SpawnDebris(Vector3 avoidWorldPosition)
    {
        ClearDebris();

        float sMinX = arenaMinX + spawnMargin;
        float sMaxX = arenaMaxX - spawnMargin;
        float sMinZ = arenaMinZ + spawnMargin;
        float sMaxZ = arenaMaxZ - spawnMargin;
        if (sMinX > sMaxX) { sMinX = sMaxX = (arenaMinX + arenaMaxX) * 0.5f; }
        if (sMinZ > sMaxZ) { sMinZ = sMaxZ = (arenaMinZ + arenaMaxZ) * 0.5f; }

        for (int i = 0; i < debrisCount; i++)
        {
            Vector3 spawnWorldPos = new Vector3(
                Random.Range(sMinX, sMaxX),
                transform.position.y + 0.5f,
                Random.Range(sMinZ, sMaxZ)
            );

            Vector2 offset = new Vector2(
                spawnWorldPos.x - avoidWorldPosition.x,
                spawnWorldPos.z - avoidWorldPosition.z
            );
            if (offset.magnitude < 2f)
            {
                if (offset.sqrMagnitude < 0.0001f)
                {
                    offset = Random.insideUnitCircle;
                }
                offset = offset.normalized * 2f;
                spawnWorldPos.x = avoidWorldPosition.x + offset.x;
                spawnWorldPos.z = avoidWorldPosition.z + offset.y;
                spawnWorldPos.x = Mathf.Clamp(spawnWorldPos.x, sMinX, sMaxX);
                spawnWorldPos.z = Mathf.Clamp(spawnWorldPos.z, sMinZ, sMaxZ);
            }

            if (enableGravity)
            {
                foreach (var gs in gravitySources)
                {
                    if (gs == null) continue;
                    Vector2 gsOffset = new Vector2(
                        spawnWorldPos.x - gs.transform.position.x,
                        spawnWorldPos.z - gs.transform.position.z);
                    if (gsOffset.magnitude < gs.spawnExclusionRadius)
                    {
                        if (gsOffset.sqrMagnitude < 0.0001f)
                            gsOffset = Random.insideUnitCircle;
                        gsOffset = gsOffset.normalized * gs.spawnExclusionRadius;
                        spawnWorldPos.x = gs.transform.position.x + gsOffset.x;
                        spawnWorldPos.z = gs.transform.position.z + gsOffset.y;
                        spawnWorldPos.x = Mathf.Clamp(spawnWorldPos.x, sMinX, sMaxX);
                        spawnWorldPos.z = Mathf.Clamp(spawnWorldPos.z, sMinZ, sMaxZ);
                    }
                }
            }

            GameObject debris = Instantiate(debrisPrefab, transform);
            debris.transform.position = spawnWorldPos;

            SpaceDebris sd = debris.GetComponent<SpaceDebris>();
            if (sd != null)
            {
                sd.SetBounds(arenaMinX, arenaMaxX, arenaMinZ, arenaMaxZ);
                bool isDrifting = Random.value < driftingRatio;
                if (isDrifting)
                {
                    Vector3 drift = new Vector3(
                        Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)
                    ).normalized * Random.Range(0.5f, maxDriftSpeed);
                    sd.SetDriftVelocity(drift);
                }
                else
                {
                    sd.MakeStatic();
                }
                activeDebris.Add(sd);
            }
        }
    }

    public void ClearDebris()
    {
        foreach (var sd in activeDebris)
        {
            if (sd != null) Destroy(sd.gameObject);
        }
        activeDebris.Clear();
    }

    public void RemoveDebris(GameObject debris)
    {
        SpaceDebris sd = debris.GetComponent<SpaceDebris>();
        if (sd != null) activeDebris.Remove(sd);
        Destroy(debris);
    }

    public Transform GetNearestDebris(Vector3 position)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var sd in activeDebris)
        {
            if (sd == null) continue;
            float dist = Vector3.Distance(position, sd.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = sd.transform;
            }
        }
        return nearest;
    }

    public bool IsInsideArena(Vector3 worldPosition, float padding = 0f)
    {
        return worldPosition.x >= arenaMinX + padding
            && worldPosition.x <= arenaMaxX - padding
            && worldPosition.z >= arenaMinZ + padding
            && worldPosition.z <= arenaMaxZ - padding;
    }

    private void CaptureDefaults()
    {
        if (defaultsCaptured) return;

        defaultDebrisCount = debrisCount;
        defaultDriftingRatio = driftingRatio;
        defaultMaxDriftSpeed = maxDriftSpeed;
        defaultMatchCameraView = matchCameraView;
        defaultArenaHalfSize = arenaHalfSize;
        defaultArenaHalfWidth = arenaHalfWidth;
        defaultsCaptured = true;
    }

    private void ResetToDefaultSettings()
    {
        debrisCount = defaultDebrisCount;
        driftingRatio = defaultDriftingRatio;
        maxDriftSpeed = defaultMaxDriftSpeed;
        matchCameraView = defaultMatchCameraView;
        arenaHalfSize = defaultArenaHalfSize;
        arenaHalfWidth = defaultArenaHalfWidth;
    }

    private void ApplyEpisodeSettings()
    {
        CaptureDefaults();
        ResetToDefaultSettings();
        ApplyAcademyEnvironmentParameters();
        RefreshBoundaryColliders();
        if (!RefreshArenaFromBoundaries())
        {
            UpdateArenaFromCamera();
        }
    }

    private void ApplyAcademyEnvironmentParameters()
    {
        if (!useAcademyEnvironmentParameters || !Academy.IsInitialized) return;

        var envParams = Academy.Instance.EnvironmentParameters;
        float difficultyLevel = envParams.GetWithDefault("difficulty_level", -1f);
        if (difficultyLevel >= 0f)
        {
            float t = Mathf.Clamp01(difficultyLevel);
            debrisCount = Mathf.Max(1, Mathf.RoundToInt(Mathf.Lerp(curriculumMinDebrisCount, defaultDebrisCount, t)));
            driftingRatio = Mathf.Lerp(curriculumMinDriftingRatio, defaultDriftingRatio, t);
            maxDriftSpeed = Mathf.Lerp(curriculumMinMaxDriftSpeed, defaultMaxDriftSpeed, t);
            arenaHalfSize = Mathf.Lerp(curriculumMinArenaHalfSize, defaultArenaHalfSize, t);
            arenaHalfWidth = Mathf.Lerp(curriculumMinArenaHalfWidth, defaultArenaHalfWidth, t);
        }

        debrisCount = Mathf.Max(1, Mathf.RoundToInt(envParams.GetWithDefault("debris_count", debrisCount)));
        driftingRatio = Mathf.Clamp01(envParams.GetWithDefault("drifting_ratio", driftingRatio));
        maxDriftSpeed = Mathf.Max(0f, envParams.GetWithDefault("max_drift_speed", maxDriftSpeed));
        matchCameraView = envParams.GetWithDefault("match_camera_view", matchCameraView ? 1f : 0f) >= 0.5f;
        arenaHalfSize = Mathf.Max(1f, envParams.GetWithDefault("arena_half_size", arenaHalfSize));
        arenaHalfWidth = Mathf.Max(1f, envParams.GetWithDefault("arena_half_width", arenaHalfWidth));
    }

    private void RefreshBoundaryColliders()
    {
        boundaryColliders.Clear();

        AutoAssignBoundaryRoot();

        var seen = new HashSet<Collider>();
        if (boundaryRoot != null)
        {
            var colliders = boundaryRoot.GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy) continue;
                if (!collider.CompareTag("Boundary")) continue;
                if (seen.Add(collider))
                {
                    boundaryColliders.Add(collider);
                }
            }
            return;
        }

        var boundaryObjects = GameObject.FindGameObjectsWithTag("Boundary");
        foreach (var boundaryObject in boundaryObjects)
        {
            var colliders = boundaryObject.GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy) continue;
                if (seen.Add(collider))
                {
                    boundaryColliders.Add(collider);
                }
            }
        }
    }

    private void AutoAssignBoundaryRoot()
    {
        if (boundaryRoot != null) return;

        Transform directChild = transform.Find("BoundaryWalls");
        if (directChild != null)
        {
            boundaryRoot = directChild;
            return;
        }

        var taggedBoundaries = GameObject.FindGameObjectsWithTag("Boundary");
        Transform bestRoot = null;
        float bestDistance = float.PositiveInfinity;
        foreach (var boundary in taggedBoundaries)
        {
            if (boundary == null) continue;
            Transform candidateRoot = boundary.transform.parent;
            if (candidateRoot == null) continue;
            float distance = Vector3.Distance(candidateRoot.position, transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestRoot = candidateRoot;
            }
        }

        if (bestRoot != null)
        {
            boundaryRoot = bestRoot;
        }
    }

    private bool RefreshArenaFromBoundaries()
    {
        if (boundaryColliders.Count == 0) return false;

        Vector3 averageCenter = Vector3.zero;
        int validColliderCount = 0;
        foreach (var boundaryCollider in boundaryColliders)
        {
            if (boundaryCollider == null || !boundaryCollider.enabled || !boundaryCollider.gameObject.activeInHierarchy) continue;
            averageCenter += boundaryCollider.bounds.center;
            validColliderCount++;
        }

        if (validColliderCount == 0) return false;
        averageCenter /= validColliderCount;

        bool hasNorth = false;
        bool hasSouth = false;
        bool hasEast = false;
        bool hasWest = false;
        float northInnerZ = float.PositiveInfinity;
        float southInnerZ = float.NegativeInfinity;
        float eastInnerX = float.PositiveInfinity;
        float westInnerX = float.NegativeInfinity;
        Bounds fallbackBounds = new Bounds();
        bool hasFallback = false;

        foreach (var boundaryCollider in boundaryColliders)
        {
            if (boundaryCollider == null || !boundaryCollider.enabled || !boundaryCollider.gameObject.activeInHierarchy) continue;

            Bounds bounds = boundaryCollider.bounds;
            if (!hasFallback)
            {
                fallbackBounds = bounds;
                hasFallback = true;
            }
            else
            {
                fallbackBounds.Encapsulate(bounds);
            }

            bool horizontalWall = bounds.size.x >= bounds.size.z;
            if (horizontalWall)
            {
                if (bounds.center.z >= averageCenter.z)
                {
                    northInnerZ = Mathf.Min(northInnerZ, bounds.min.z);
                    hasNorth = true;
                }
                else
                {
                    southInnerZ = Mathf.Max(southInnerZ, bounds.max.z);
                    hasSouth = true;
                }
            }
            else
            {
                if (bounds.center.x >= averageCenter.x)
                {
                    eastInnerX = Mathf.Min(eastInnerX, bounds.min.x);
                    hasEast = true;
                }
                else
                {
                    westInnerX = Mathf.Max(westInnerX, bounds.max.x);
                    hasWest = true;
                }
            }
        }

        if (hasNorth && hasSouth && hasEast && hasWest
            && eastInnerX > westInnerX && northInnerZ > southInnerZ)
        {
            arenaMinX = westInnerX;
            arenaMaxX = eastInnerX;
            arenaMinZ = southInnerZ;
            arenaMaxZ = northInnerZ;
        }
        else if (hasFallback)
        {
            arenaMinX = fallbackBounds.min.x;
            arenaMaxX = fallbackBounds.max.x;
            arenaMinZ = fallbackBounds.min.z;
            arenaMaxZ = fallbackBounds.max.z;
        }
        else
        {
            return false;
        }

        arenaHalfWidth = Mathf.Max(1f, ArenaWidth * 0.5f);
        arenaHalfSize = Mathf.Max(1f, ArenaDepth * 0.5f);
        return true;
    }

    public Vector3 ComputeNetGravity(Vector3 position)
    {
        if (!enableGravity) return Vector3.zero;
        Vector3 net = Vector3.zero;
        foreach (var gs in gravitySources)
        {
            if (gs == null) continue;
            net += gs.ComputeAcceleration(position, gravityConstant);
        }
        return net;
    }

    public GravitySource FindStrongestSource(Vector3 position, out float strongestPull)
    {
        GravitySource best = null;
        strongestPull = 0f;
        if (!enableGravity) return null;
        foreach (var gs in gravitySources)
        {
            if (gs == null) continue;
            float pull = gs.ComputeAcceleration(position, gravityConstant).magnitude;
            if (pull > strongestPull)
            {
                strongestPull = pull;
                best = gs;
            }
        }
        return best;
    }

    private void AutoDiscoverGravitySources()
    {
        if (gravitySources.Count > 0) return;
        Transform gsRoot = transform.Find("GravitySources");
        if (gsRoot != null)
            gravitySources.AddRange(gsRoot.GetComponentsInChildren<GravitySource>());
    }

    private void UpdateArenaFromCamera()
    {
        if (!matchCameraView)
        {
            arenaMinX = transform.position.x - arenaHalfWidth;
            arenaMaxX = transform.position.x + arenaHalfWidth;
            arenaMinZ = transform.position.z - arenaHalfSize;
            arenaMaxZ = transform.position.z + arenaHalfSize;
            return;
        }

        var cam = Camera.main;
        if (cam == null || !cam.orthographic)
        {
            arenaMinX = transform.position.x - arenaHalfWidth;
            arenaMaxX = transform.position.x + arenaHalfWidth;
            arenaMinZ = transform.position.z - arenaHalfSize;
            arenaMaxZ = transform.position.z + arenaHalfSize;
            return;
        }

        arenaHalfSize = cam.orthographicSize - screenMargin;
        arenaHalfWidth = cam.orthographicSize * cam.aspect - screenMargin;
        arenaMinX = transform.position.x - arenaHalfWidth;
        arenaMaxX = transform.position.x + arenaHalfWidth;
        arenaMinZ = transform.position.z - arenaHalfSize;
        arenaMaxZ = transform.position.z + arenaHalfSize;
    }
}
