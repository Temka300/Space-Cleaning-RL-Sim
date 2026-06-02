using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using UnityEngine;

[System.Serializable]
public class PVPSpawnSnapshot
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
    public bool drifting;
    public Vector3 velocity;
}

[System.Serializable]
public class PVPReplayLayoutSnapshot
{
    public string sceneName;
    public PVPSpawnSnapshot[] debris;
    public PVPSpawnSnapshot[] fuel;
}

public class PVPEnvironmentManager : MonoBehaviour
{
    [Header("Debris Settings")]
    public GameObject debrisPrefab;
    public GameObject[] debrisPrefabs;
    public int debrisCount = 15;
    [Range(0f, 1f)] public float driftingRatio = 0.3f;
    public float maxDriftSpeed = 2f;
    public Vector2 debrisScaleRange = new Vector2(0.75f, 1.35f);
    public bool randomizeDebrisRotation = true;
    public float spawnMargin = 1.5f;

    [Header("Fuel Canisters")]
    [Tooltip("PropellantCanister prefab rockets refuel from. If null, no canisters spawn.")]
    public GameObject fuelCanisterPrefab;
    [Min(0)] public int fuelCanisterCount = 4;
    [Range(0f, 1f)] public float fuelDriftingRatio = 0f;
    public Vector2 fuelScaleRange = new Vector2(1f, 1f);

    [Header("Arena Settings")]
    public bool matchCameraView = true;
    public float arenaHalfSize = 9f;
    public float arenaHalfWidth = 9f;
    public float screenMargin = 1f;

    [Header("3D Arena")]
    [Tooltip("Enables vertical spawning, vertical arena checks, and expanded PVP observations/actions.")]
    public bool enableThreeDimensionalArena = false;
    public float arenaHalfHeight = 25f;
    public Vector2 spawnHeightRange = new Vector2(0.5f, 0.5f);

    [Header("Camera Boundary")]
    [Tooltip("When enabled, the camera viewport defines the kill arena. Boundary walls are synced so the red border sits on that camera edge.")]
    public bool useCameraViewAsBoundary = false;
    public bool syncBoundaryRootToCameraView = true;
    public float cameraBoundaryThickness = 2f;
    public float cameraBoundaryHeight = 4f;

    [Header("Gravity")]
    public bool enableGravity = false;
    public float gravityConstant = 1f;
    public bool planetContactKills = true;
    [Tooltip("If false, GravitySource is used only for agent observations/penalties/contact; physical pull is left to another system (e.g. NBodyGravityReceiver). Prevents double gravity.")]
    public bool applyGravityForce = true;
    public List<GravitySource> gravitySources = new List<GravitySource>();

    [Header("PVP Settings")]
    public float respawnInvulnerabilityTime = 1.0f;

    [Header("Replay/Test")]
    public bool deterministicLayoutInReplay = true;
    public bool saveLayoutWhenRecording = true;
    public bool saveLayoutOnFirstTestSpawn = true;
    public bool resetDynamicBodiesOnTestRespawn = true;
    public string replayLayoutFileName = "Level6_1_TestLayout";

    [Header("Boundary Source")]
    public Transform boundaryRoot;

    private List<SpaceDebris> activeDebris = new List<SpaceDebris>();
    private readonly List<PropellantCanister> activeFuel = new List<PropellantCanister>();
    private readonly List<Collider> boundaryColliders = new List<Collider>();
    private GameObject[] resourceDebrisPrefabs;

    private float arenaMinX, arenaMaxX, arenaMinY, arenaMaxY, arenaMinZ, arenaMaxZ;

    private PVPAgent agentA;
    private PVPAgent agentB;
    private int scoreA;
    private int scoreB;
    private int fuelPickupsA;
    private int fuelPickupsB;
    private readonly List<NBodyStartState> nbodyStartStates = new List<NBodyStartState>();

    private struct NBodyStartState
    {
        public NBodyCelestialBody body;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
    }

    public int RemainingDebris => activeDebris.Count;
    public int RemainingFuelCanisters => activeFuel.Count;
    public float ArenaMinX => arenaMinX;
    public float ArenaMaxX => arenaMaxX;
    public float ArenaMinY => arenaMinY;
    public float ArenaMaxY => arenaMaxY;
    public float ArenaMinZ => arenaMinZ;
    public float ArenaMaxZ => arenaMaxZ;
    public float ArenaWidth => Mathf.Max(0f, arenaMaxX - arenaMinX);
    public float ArenaHeight => Mathf.Max(0f, arenaMaxY - arenaMinY);
    public float ArenaDepth => Mathf.Max(0f, arenaMaxZ - arenaMinZ);
    public bool UseThreeDimensionalArena => enableThreeDimensionalArena;
    public Vector3 ArenaCenterWorld =>
        new Vector3((arenaMinX + arenaMaxX) * 0.5f, (arenaMinY + arenaMaxY) * 0.5f, (arenaMinZ + arenaMaxZ) * 0.5f);

    public int ScoreA => scoreA;
    public int ScoreB => scoreB;
    public int FuelPickupsA => fuelPickupsA;
    public int FuelPickupsB => fuelPickupsB;

    void Awake()
    {
        AutoAssignBoundaryRoot();
        RefreshBoundaryColliders();
        AutoDiscoverGravitySources();
        CaptureNBodyStartStates();
    }

    void Start()
    {
        PrepareArena();
    }

    void FixedUpdate()
    {
        if (!enableGravity || gravitySources.Count == 0 || !applyGravityForce) return;

        ApplyGravityToAgent(agentA);
        ApplyGravityToAgent(agentB);

        foreach (var sd in activeDebris)
        {
            if (sd == null || sd.Rb == null || sd.Rb.isKinematic) continue;
            Vector3 accel = ComputeNetGravity(sd.transform.position);
            sd.Rb.AddForce(accel, ForceMode.Acceleration);
        }
    }

    private void ApplyGravityToAgent(PVPAgent agent)
    {
        if (agent == null) return;
        var rm = agent.rocketMovement;
        if (rm == null) return;
        Vector3 accel = ComputeNetGravity(agent.transform.position);
        rm.SetExternalAcceleration(accel);
    }

    public void RegisterAgent(PVPAgent agent, int team)
    {
        if (team == 0)
            agentA = agent;
        else
            agentB = agent;
    }

    public PVPAgent GetOpponent(PVPAgent agent)
    {
        return agent == agentA ? agentB : agentA;
    }

    public int GetScore(int team) => team == 0 ? scoreA : scoreB;
    public int GetOpponentScore(int team) => team == 0 ? scoreB : scoreA;
    public int GetFuelPickups(int team) => team == 0 ? fuelPickupsA : fuelPickupsB;
    public float GetFuelPickupRatio(int team)
    {
        int maxPickups = Mathf.Max(1, fuelCanisterCount);
        return Mathf.Clamp01((float)GetFuelPickups(team) / maxPickups);
    }

    public void PrepareArena()
    {
        UpdateVerticalArena();

        if (useCameraViewAsBoundary)
        {
            UpdateArenaFromCamera();
            if (syncBoundaryRootToCameraView)
            {
                SyncBoundaryRootToArena();
                Physics.SyncTransforms();
                RefreshBoundaryColliders();
            }
            return;
        }

        RefreshBoundaryColliders();
        if (!RefreshArenaFromBoundaries())
            UpdateArenaFromCamera();
    }

    public void ResetMatch()
    {
        scoreA = 0;
        scoreB = 0;
        fuelPickupsA = 0;
        fuelPickupsB = 0;
        PrepareArena();

        if (ShouldUseReplayLayout())
            ResetNBodyToStartStates();
    }

    public void SpawnDebris()
    {
        ClearDebris();
        ClearFuelCanisters();

        if (ShouldUseReplayLayout() && TrySpawnSavedReplayLayout())
            return;

        SpawnRandomDebrisAndFuel();

        if (ShouldSaveReplayLayout())
            SaveCurrentReplayLayout();
    }

    private void SpawnRandomDebrisAndFuel()
    {

        float sMinX = arenaMinX + spawnMargin;
        float sMaxX = arenaMaxX - spawnMargin;
        GetSpawnYRange(out float sMinY, out float sMaxY);
        float sMinZ = arenaMinZ + spawnMargin;
        float sMaxZ = arenaMaxZ - spawnMargin;
        if (sMinX > sMaxX) sMinX = sMaxX = (arenaMinX + arenaMaxX) * 0.5f;
        if (sMinZ > sMaxZ) sMinZ = sMaxZ = (arenaMinZ + arenaMaxZ) * 0.5f;

        List<Vector3> avoidPositions = new List<Vector3>();
        if (agentA != null) avoidPositions.Add(agentA.transform.position);
        if (agentB != null) avoidPositions.Add(agentB.transform.position);

        for (int i = 0; i < debrisCount; i++)
        {
            Vector3 spawnWorldPos = new Vector3(
                Random.Range(sMinX, sMaxX),
                Random.Range(sMinY, sMaxY),
                Random.Range(sMinZ, sMaxZ));

            foreach (var avoidPos in avoidPositions)
            {
                spawnWorldPos = PushSpawnAway(
                    spawnWorldPos, avoidPos, 2f,
                    sMinX, sMaxX, sMinY, sMaxY, sMinZ, sMaxZ);
            }

            if (enableGravity)
            {
                foreach (var gs in gravitySources)
                {
                    if (gs == null) continue;
                    spawnWorldPos = PushSpawnAway(
                        spawnWorldPos, gs.transform.position, gs.spawnExclusionRadius,
                        sMinX, sMaxX, sMinY, sMaxY, sMinZ, sMaxZ);
                }
            }

            GameObject prefab = GetRandomDebrisPrefab();
            if (prefab == null)
            {
                Debug.LogWarning("No debris prefab assigned or found under Resources/Debris.", this);
                continue;
            }

            GameObject debris = Instantiate(prefab, transform);
            debris.transform.position = spawnWorldPos;
            if (randomizeDebrisRotation)
                debris.transform.rotation = Random.rotation;

            float minScale = Mathf.Min(debrisScaleRange.x, debrisScaleRange.y);
            float maxScale = Mathf.Max(debrisScaleRange.x, debrisScaleRange.y);
            float scale = Random.Range(minScale, maxScale);
            debris.transform.localScale = Vector3.Scale(debris.transform.localScale, Vector3.one * scale);

            SpaceDebris sd = debris.GetComponent<SpaceDebris>();
            if (sd != null)
            {
                SetDebrisBounds(sd);
                bool isDrifting = Random.value < driftingRatio;
                if (isDrifting)
                    sd.SetDriftVelocity(GetRandomDriftVelocity());
                else
                    sd.MakeStatic();
                activeDebris.Add(sd);
            }
        }

        SpawnFuelCanisters();
    }

    private void SpawnFuelCanisters()
    {
        if (fuelCanisterPrefab == null || fuelCanisterCount <= 0) return;

        float sMinX = arenaMinX + spawnMargin;
        float sMaxX = arenaMaxX - spawnMargin;
        GetSpawnYRange(out float sMinY, out float sMaxY);
        float sMinZ = arenaMinZ + spawnMargin;
        float sMaxZ = arenaMaxZ - spawnMargin;
        if (sMinX > sMaxX) sMinX = sMaxX = (arenaMinX + arenaMaxX) * 0.5f;
        if (sMinZ > sMaxZ) sMinZ = sMaxZ = (arenaMinZ + arenaMaxZ) * 0.5f;

        for (int i = 0; i < fuelCanisterCount; i++)
        {
            Vector3 spawnWorldPos = new Vector3(
                Random.Range(sMinX, sMaxX),
                Random.Range(sMinY, sMaxY),
                Random.Range(sMinZ, sMaxZ));

            if (enableGravity)
            {
                foreach (var gs in gravitySources)
                {
                    if (gs == null) continue;
                    spawnWorldPos = PushSpawnAway(
                        spawnWorldPos, gs.transform.position, gs.spawnExclusionRadius,
                        sMinX, sMaxX, sMinY, sMaxY, sMinZ, sMaxZ);
                }
            }

            GameObject obj = Instantiate(fuelCanisterPrefab, transform);
            obj.transform.position = spawnWorldPos;

            float minScale = Mathf.Min(fuelScaleRange.x, fuelScaleRange.y);
            float maxScale = Mathf.Max(fuelScaleRange.x, fuelScaleRange.y);
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = Vector3.Scale(obj.transform.localScale, Vector3.one * scale);

            PropellantCanister pc = obj.GetComponent<PropellantCanister>();
            if (pc != null)
            {
                SetCanisterBounds(pc);
                if (Random.value < fuelDriftingRatio)
                    pc.SetDriftVelocity(GetRandomDriftVelocity());
                else
                    pc.MakeStatic();
                activeFuel.Add(pc);
            }
        }
    }

    public void ClearFuelCanisters()
    {
        foreach (var pc in activeFuel)
            if (pc != null) Destroy(pc.gameObject);
        activeFuel.Clear();
    }

    public Transform GetNearestFuel(Vector3 position)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var pc in activeFuel)
        {
            if (pc == null) continue;
            float dist = Vector3.Distance(position, pc.transform.position);
            if (dist < minDist) { minDist = dist; nearest = pc.transform; }
        }
        return nearest;
    }

    public bool RefuelFromCanister(GameObject canister, RocketMovement rocket)
    {
        PropellantCanister pc = canister.GetComponent<PropellantCanister>();
        if (pc == null) return false;
        bool ok = pc.ApplyTo(rocket);
        activeFuel.Remove(pc);
        Destroy(pc.gameObject);
        return ok;
    }

    public bool RefuelFromCanister(GameObject canister, PVPAgent collector)
    {
        if (collector == null) return false;

        bool ok = RefuelFromCanister(canister, collector.rocketMovement);
        if (ok)
        {
            if (collector.team == 0)
                fuelPickupsA++;
            else
                fuelPickupsB++;
        }
        return ok;
    }

    public void CollectDebris(GameObject debris, PVPAgent collector)
    {
        SpaceDebris sd = debris.GetComponent<SpaceDebris>();
        if (sd != null) activeDebris.Remove(sd);
        Destroy(debris);

        if (collector.team == 0)
            scoreA++;
        else
            scoreB++;
    }

    public void RespawnAgent(PVPAgent agent)
    {
        if (resetDynamicBodiesOnTestRespawn && ShouldUseReplayLayout())
            ResetNBodyToStartStates();

        agent.rocketMovement.ResetRocket(agent.SpawnPosition, agent.SpawnRotation);
        agent.OnRespawned();
    }

    public void ClearDebris()
    {
        foreach (var sd in activeDebris)
        {
            if (sd != null) Destroy(sd.gameObject);
        }
        activeDebris.Clear();
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
        bool inside = worldPosition.x >= arenaMinX + padding
            && worldPosition.x <= arenaMaxX - padding
            && worldPosition.z >= arenaMinZ + padding
            && worldPosition.z <= arenaMaxZ - padding;

        if (enableThreeDimensionalArena)
        {
            inside = inside
                && worldPosition.y >= arenaMinY + padding
                && worldPosition.y <= arenaMaxY - padding;
        }

        return inside;
    }

    private void UpdateVerticalArena()
    {
        if (!enableThreeDimensionalArena)
        {
            arenaMinY = transform.position.y;
            arenaMaxY = transform.position.y;
            return;
        }

        float halfHeight = Mathf.Max(0.5f, arenaHalfHeight);
        arenaMinY = transform.position.y - halfHeight;
        arenaMaxY = transform.position.y + halfHeight;
    }

    private void GetSpawnYRange(out float minYWorld, out float maxYWorld)
    {
        if (!enableThreeDimensionalArena)
        {
            minYWorld = transform.position.y + 0.5f;
            maxYWorld = minYWorld;
            return;
        }

        float low = Mathf.Min(spawnHeightRange.x, spawnHeightRange.y);
        float high = Mathf.Max(spawnHeightRange.x, spawnHeightRange.y);
        minYWorld = Mathf.Clamp(transform.position.y + low, arenaMinY + spawnMargin, arenaMaxY - spawnMargin);
        maxYWorld = Mathf.Clamp(transform.position.y + high, arenaMinY + spawnMargin, arenaMaxY - spawnMargin);

        if (minYWorld > maxYWorld)
            minYWorld = maxYWorld = (arenaMinY + arenaMaxY) * 0.5f;
    }

    private Vector3 PushSpawnAway(Vector3 spawnWorldPos, Vector3 avoidWorldPos, float minDistance,
        float minXWorld, float maxXWorld, float minYWorld, float maxYWorld, float minZWorld, float maxZWorld)
    {
        if (minDistance <= 0f) return spawnWorldPos;

        if (enableThreeDimensionalArena)
        {
            Vector3 offset = spawnWorldPos - avoidWorldPos;
            if (offset.magnitude < minDistance)
            {
                if (offset.sqrMagnitude < 0.0001f)
                    offset = Random.onUnitSphere;
                spawnWorldPos = avoidWorldPos + offset.normalized * minDistance;
                spawnWorldPos.y = Mathf.Clamp(spawnWorldPos.y, minYWorld, maxYWorld);
            }
        }
        else
        {
            Vector2 offset = new Vector2(
                spawnWorldPos.x - avoidWorldPos.x,
                spawnWorldPos.z - avoidWorldPos.z);
            if (offset.magnitude < minDistance)
            {
                if (offset.sqrMagnitude < 0.0001f)
                    offset = Random.insideUnitCircle;
                offset = offset.normalized * minDistance;
                spawnWorldPos.x = avoidWorldPos.x + offset.x;
                spawnWorldPos.z = avoidWorldPos.z + offset.y;
            }
        }

        spawnWorldPos.x = Mathf.Clamp(spawnWorldPos.x, minXWorld, maxXWorld);
        spawnWorldPos.z = Mathf.Clamp(spawnWorldPos.z, minZWorld, maxZWorld);
        return spawnWorldPos;
    }

    private Vector3 GetRandomDriftVelocity()
    {
        Vector3 direction = enableThreeDimensionalArena
            ? Random.onUnitSphere
            : new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        if (direction.sqrMagnitude < 0.0001f)
            direction = enableThreeDimensionalArena ? Vector3.up : Vector3.forward;

        return direction.normalized * Random.Range(0.5f, maxDriftSpeed);
    }

    private void SetDebrisBounds(SpaceDebris debris)
    {
        if (enableThreeDimensionalArena)
            debris.SetBounds(arenaMinX, arenaMaxX, arenaMinY, arenaMaxY, arenaMinZ, arenaMaxZ);
        else
            debris.SetBounds(arenaMinX, arenaMaxX, arenaMinZ, arenaMaxZ);
    }

    private void SetCanisterBounds(PropellantCanister canister)
    {
        if (enableThreeDimensionalArena)
            canister.SetBounds(arenaMinX, arenaMaxX, arenaMinY, arenaMaxY, arenaMinZ, arenaMaxZ);
        else
            canister.SetBounds(arenaMinX, arenaMaxX, arenaMinZ, arenaMaxZ);
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

    private GameObject GetRandomDebrisPrefab()
    {
        GameObject prefab = PickRandomValidPrefab(debrisPrefabs);
        if (prefab != null) return prefab;
        if (resourceDebrisPrefabs == null || resourceDebrisPrefabs.Length == 0)
            resourceDebrisPrefabs = Resources.LoadAll<GameObject>("Debris");
        prefab = PickRandomValidPrefab(resourceDebrisPrefabs);
        return prefab != null ? prefab : debrisPrefab;
    }

    private static GameObject PickRandomValidPrefab(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) return null;
        int start = Random.Range(0, prefabs.Length);
        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[(start + i) % prefabs.Length];
            if (prefab != null) return prefab;
        }
        return null;
    }

    private bool ShouldUseReplayLayout()
    {
        if (!deterministicLayoutInReplay) return false;
        return IsReplayLayoutAgent(agentA) || IsReplayLayoutAgent(agentB);
    }

    private bool ShouldSaveReplayLayout()
    {
        if (!deterministicLayoutInReplay) return false;
        if (saveLayoutWhenRecording && (IsRecordingAgent(agentA) || IsRecordingAgent(agentB))) return true;
        if (saveLayoutOnFirstTestSpawn && (IsTestAgent(agentA) || IsTestAgent(agentB)))
            return !File.Exists(GetReplayLayoutPath());
        return false;
    }

    private static bool IsReplayLayoutAgent(PVPAgent agent)
    {
        return agent != null && agent.UsesStableReplayLayout;
    }

    private static bool IsRecordingAgent(PVPAgent agent)
    {
        return agent != null && agent.IsRecordingReplay;
    }

    private static bool IsTestAgent(PVPAgent agent)
    {
        return agent != null && agent.IsTestReplay;
    }

    private string GetReplayLayoutPath()
    {
        string dir = Path.Combine(Application.persistentDataPath, "PVPReplays");
        string fileName = string.IsNullOrWhiteSpace(replayLayoutFileName)
            ? "PVP_TestLayout"
            : replayLayoutFileName;
        foreach (char invalid in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(invalid, '_');
        return Path.Combine(dir, fileName + ".json");
    }

    private bool TrySpawnSavedReplayLayout()
    {
        string path = GetReplayLayoutPath();
        if (!File.Exists(path)) return false;

        try
        {
            PVPReplayLayoutSnapshot snapshot = JsonUtility.FromJson<PVPReplayLayoutSnapshot>(File.ReadAllText(path));
            if (snapshot == null || snapshot.debris == null) return false;

            SpawnSnapshotObjects(snapshot.debris, true);
            if (snapshot.fuel != null)
                SpawnSnapshotObjects(snapshot.fuel, false);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Could not load replay layout '{path}': {ex.Message}", this);
            return false;
        }
    }

    private void SpawnSnapshotObjects(PVPSpawnSnapshot[] snapshots, bool isDebris)
    {
        foreach (PVPSpawnSnapshot snapshot in snapshots)
        {
            if (snapshot == null) continue;

            GameObject prefab = isDebris
                ? GetDebrisPrefabByName(snapshot.prefabName)
                : fuelCanisterPrefab;
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab, transform);
            obj.transform.SetPositionAndRotation(snapshot.position, snapshot.rotation);
            obj.transform.localScale = snapshot.localScale;

            if (isDebris)
            {
                SpaceDebris sd = obj.GetComponent<SpaceDebris>();
                if (sd == null) continue;
                SetDebrisBounds(sd);
                if (snapshot.drifting) sd.SetDriftVelocity(snapshot.velocity);
                else sd.MakeStatic();
                activeDebris.Add(sd);
            }
            else
            {
                PropellantCanister pc = obj.GetComponent<PropellantCanister>();
                if (pc == null) continue;
                SetCanisterBounds(pc);
                if (snapshot.drifting) pc.SetDriftVelocity(snapshot.velocity);
                else pc.MakeStatic();
                activeFuel.Add(pc);
            }
        }
    }

    private GameObject GetDebrisPrefabByName(string prefabName)
    {
        if (!string.IsNullOrEmpty(prefabName))
        {
            GameObject resourcePrefab = Resources.Load<GameObject>("Debris/" + prefabName);
            if (resourcePrefab != null) return resourcePrefab;

            GameObject[] prefabs = debrisPrefabs;
            if (prefabs != null)
            {
                for (int i = 0; i < prefabs.Length; i++)
                    if (prefabs[i] != null && prefabs[i].name == prefabName)
                        return prefabs[i];
            }

            if (debrisPrefab != null && debrisPrefab.name == prefabName)
                return debrisPrefab;
        }

        return GetRandomDebrisPrefab();
    }

    private void SaveCurrentReplayLayout()
    {
        try
        {
            string path = GetReplayLayoutPath();
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            PVPReplayLayoutSnapshot snapshot = new PVPReplayLayoutSnapshot
            {
                sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                debris = CaptureDebrisSnapshots(),
                fuel = CaptureFuelSnapshots()
            };

            File.WriteAllText(path, JsonUtility.ToJson(snapshot, true));
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Could not save replay layout: {ex.Message}", this);
        }
    }

    private PVPSpawnSnapshot[] CaptureDebrisSnapshots()
    {
        List<PVPSpawnSnapshot> snapshots = new List<PVPSpawnSnapshot>();
        foreach (SpaceDebris sd in activeDebris)
        {
            if (sd == null) continue;
            snapshots.Add(CaptureSnapshot(sd.gameObject, sd.Rb));
        }
        return snapshots.ToArray();
    }

    private PVPSpawnSnapshot[] CaptureFuelSnapshots()
    {
        List<PVPSpawnSnapshot> snapshots = new List<PVPSpawnSnapshot>();
        foreach (PropellantCanister pc in activeFuel)
        {
            if (pc == null) continue;
            snapshots.Add(CaptureSnapshot(pc.gameObject, pc.Rb));
        }
        return snapshots.ToArray();
    }

    private static PVPSpawnSnapshot CaptureSnapshot(GameObject obj, Rigidbody body)
    {
        PVPSpawnSnapshot snapshot = new PVPSpawnSnapshot
        {
            prefabName = CleanCloneName(obj.name),
            position = obj.transform.position,
            rotation = obj.transform.rotation,
            localScale = obj.transform.localScale,
            drifting = body != null && !body.isKinematic,
            velocity = Vector3.zero
        };

        if (body != null)
        {
#if UNITY_6000_0_OR_NEWER
            snapshot.velocity = body.linearVelocity;
#else
            snapshot.velocity = body.velocity;
#endif
        }

        return snapshot;
    }

    private static string CleanCloneName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return objectName;
        return objectName.Replace("(Clone)", string.Empty).Trim();
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
        if (bestRoot != null) boundaryRoot = bestRoot;
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
                if (seen.Add(collider)) boundaryColliders.Add(collider);
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
                if (seen.Add(collider)) boundaryColliders.Add(collider);
            }
        }
    }

    private bool RefreshArenaFromBoundaries()
    {
        if (boundaryColliders.Count == 0) return false;
        Vector3 averageCenter = Vector3.zero;
        int validCount = 0;
        foreach (var bc in boundaryColliders)
        {
            if (bc == null || !bc.enabled || !bc.gameObject.activeInHierarchy) continue;
            averageCenter += bc.bounds.center;
            validCount++;
        }
        if (validCount == 0) return false;
        averageCenter /= validCount;

        bool hasNorth = false, hasSouth = false, hasEast = false, hasWest = false;
        float northZ = float.PositiveInfinity, southZ = float.NegativeInfinity;
        float eastX = float.PositiveInfinity, westX = float.NegativeInfinity;
        Bounds fallback = new Bounds();
        bool hasFallback = false;

        foreach (var bc in boundaryColliders)
        {
            if (bc == null || !bc.enabled || !bc.gameObject.activeInHierarchy) continue;
            Bounds b = bc.bounds;
            if (!hasFallback) { fallback = b; hasFallback = true; }
            else fallback.Encapsulate(b);

            bool horizontal = b.size.x >= b.size.z;
            if (horizontal)
            {
                if (b.center.z >= averageCenter.z) { northZ = Mathf.Min(northZ, b.min.z); hasNorth = true; }
                else { southZ = Mathf.Max(southZ, b.max.z); hasSouth = true; }
            }
            else
            {
                if (b.center.x >= averageCenter.x) { eastX = Mathf.Min(eastX, b.min.x); hasEast = true; }
                else { westX = Mathf.Max(westX, b.max.x); hasWest = true; }
            }
        }

        if (hasNorth && hasSouth && hasEast && hasWest && eastX > westX && northZ > southZ)
        {
            arenaMinX = westX; arenaMaxX = eastX;
            arenaMinZ = southZ; arenaMaxZ = northZ;
        }
        else if (hasFallback)
        {
            arenaMinX = fallback.min.x; arenaMaxX = fallback.max.x;
            arenaMinZ = fallback.min.z; arenaMaxZ = fallback.max.z;
        }
        else return false;

        arenaHalfWidth = Mathf.Max(1f, ArenaWidth * 0.5f);
        arenaHalfSize = Mathf.Max(1f, ArenaDepth * 0.5f);
        return true;
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
        if (cam == null || !cam.orthographic || !TryGetCameraXZBounds(cam, out Bounds cameraBounds))
        {
            arenaMinX = transform.position.x - arenaHalfWidth;
            arenaMaxX = transform.position.x + arenaHalfWidth;
            arenaMinZ = transform.position.z - arenaHalfSize;
            arenaMaxZ = transform.position.z + arenaHalfSize;
            return;
        }

        arenaMinX = cameraBounds.min.x + screenMargin;
        arenaMaxX = cameraBounds.max.x - screenMargin;
        arenaMinZ = cameraBounds.min.z + screenMargin;
        arenaMaxZ = cameraBounds.max.z - screenMargin;

        if (arenaMinX > arenaMaxX)
            arenaMinX = arenaMaxX = cameraBounds.center.x;
        if (arenaMinZ > arenaMaxZ)
            arenaMinZ = arenaMaxZ = cameraBounds.center.z;

        arenaHalfWidth = Mathf.Max(1f, ArenaWidth * 0.5f);
        arenaHalfSize = Mathf.Max(1f, ArenaDepth * 0.5f);
    }

    private bool TryGetCameraXZBounds(Camera cam, out Bounds bounds)
    {
        bounds = new Bounds(transform.position, Vector3.zero);
        Plane arenaPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
        Vector2[] corners =
        {
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f)
        };

        bool hasPoint = false;
        for (int i = 0; i < corners.Length; i++)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(corners[i].x, corners[i].y, 0f));
            if (!arenaPlane.Raycast(ray, out float distance)) continue;

            Vector3 point = ray.GetPoint(distance);
            if (!hasPoint)
            {
                bounds = new Bounds(point, Vector3.zero);
                hasPoint = true;
            }
            else
            {
                bounds.Encapsulate(point);
            }
        }

        return hasPoint;
    }

    private void SyncBoundaryRootToArena()
    {
        if (boundaryRoot == null)
        {
            GameObject root = new GameObject("BoundaryWalls");
            root.transform.SetParent(transform, false);
            boundaryRoot = root.transform;
        }

        float thickness = Mathf.Max(0.1f, cameraBoundaryThickness);
        float height = enableThreeDimensionalArena
            ? Mathf.Max(0.1f, ArenaHeight + cameraBoundaryHeight)
            : Mathf.Max(0.1f, cameraBoundaryHeight);
        float centerY = enableThreeDimensionalArena
            ? (arenaMinY + arenaMaxY) * 0.5f
            : transform.position.y + height * 0.5f;
        float centerX = (arenaMinX + arenaMaxX) * 0.5f;
        float centerZ = (arenaMinZ + arenaMaxZ) * 0.5f;
        float width = Mathf.Max(0.1f, ArenaWidth);
        float depth = Mathf.Max(0.1f, ArenaDepth);

        Material boundaryMaterial = FindBoundaryMaterial();
        SetBoundaryWall("West", new Vector3(arenaMinX - thickness * 0.5f, centerY, centerZ),
            new Vector3(thickness, height, depth + thickness * 2f), boundaryMaterial);
        SetBoundaryWall("East", new Vector3(arenaMaxX + thickness * 0.5f, centerY, centerZ),
            new Vector3(thickness, height, depth + thickness * 2f), boundaryMaterial);
        SetBoundaryWall("South", new Vector3(centerX, centerY, arenaMinZ - thickness * 0.5f),
            new Vector3(width + thickness * 2f, height, thickness), boundaryMaterial);
        SetBoundaryWall("North", new Vector3(centerX, centerY, arenaMaxZ + thickness * 0.5f),
            new Vector3(width + thickness * 2f, height, thickness), boundaryMaterial);
    }

    private void SetBoundaryWall(string wallName, Vector3 position, Vector3 scale, Material material)
    {
        Transform wall = boundaryRoot.Find(wallName);
        if (wall == null)
        {
            GameObject wallObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallObject.name = wallName;
            wallObject.transform.SetParent(boundaryRoot, false);
            wall = wallObject.transform;
        }

        wall.gameObject.tag = "Boundary";
        wall.SetPositionAndRotation(position, Quaternion.identity);
        wall.localScale = scale;

        Collider collider = wall.GetComponent<Collider>();
        if (collider != null) collider.isTrigger = true;

        Renderer renderer = wall.GetComponent<Renderer>();
        if (renderer != null && material != null)
            renderer.sharedMaterial = material;
    }

    private Material FindBoundaryMaterial()
    {
        if (boundaryRoot == null) return null;
        Renderer renderer = boundaryRoot.GetComponentInChildren<Renderer>(true);
        return renderer != null ? renderer.sharedMaterial : null;
    }

    private void CaptureNBodyStartStates()
    {
        nbodyStartStates.Clear();
#if UNITY_6000_0_OR_NEWER
        NBodyCelestialBody[] bodies = FindObjectsByType<NBodyCelestialBody>(FindObjectsSortMode.None);
#else
        NBodyCelestialBody[] bodies = FindObjectsOfType<NBodyCelestialBody>();
#endif
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] == null) continue;
            nbodyStartStates.Add(new NBodyStartState
            {
                body = bodies[i],
                position = bodies[i].transform.position,
                rotation = bodies[i].transform.rotation,
                velocity = bodies[i].Velocity
            });
        }
    }

    private void ResetNBodyToStartStates()
    {
        for (int i = 0; i < nbodyStartStates.Count; i++)
        {
            NBodyStartState state = nbodyStartStates[i];
            if (state.body == null) continue;
            state.body.ResetState(state.position, state.rotation, state.velocity);
        }
    }

    private void AutoDiscoverGravitySources()
    {
        if (gravitySources.Count > 0) return;
        Transform gsRoot = transform.Find("GravitySources");
        if (gsRoot != null)
            gravitySources.AddRange(gsRoot.GetComponentsInChildren<GravitySource>());
    }
}
