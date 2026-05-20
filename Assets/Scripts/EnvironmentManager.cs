using UnityEngine;
using System.Collections.Generic;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Debris Settings")]
    public GameObject debrisPrefab;
    public int debrisCount = 10;
    [Range(0f, 1f)]
    public float driftingRatio = 0.3f;
    public float maxDriftSpeed = 2f;

    [Header("Arena Settings")]
    public float arenaHalfSize = 9f;

    private List<GameObject> activeDebris = new List<GameObject>();

    public int RemainingDebris => activeDebris.Count;

    public void SpawnDebris()
    {
        ClearDebris();

        for (int i = 0; i < debrisCount; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(-arenaHalfSize, arenaHalfSize),
                0.5f,
                Random.Range(-arenaHalfSize, arenaHalfSize)
            );

            // Avoid spawning too close to center (where rocket spawns)
            if (spawnPos.magnitude < 2f)
            {
                spawnPos = spawnPos.normalized * 2f;
            }

            GameObject debris = Instantiate(debrisPrefab, transform);
            debris.transform.localPosition = spawnPos;

            SpaceDebris sd = debris.GetComponent<SpaceDebris>();
            if (sd != null)
            {
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
            }

            activeDebris.Add(debris);
        }
    }

    public void ClearDebris()
    {
        foreach (var d in activeDebris)
        {
            if (d != null) Destroy(d);
        }
        activeDebris.Clear();
    }

    public void RemoveDebris(GameObject debris)
    {
        activeDebris.Remove(debris);
        Destroy(debris);
    }

    public Transform GetNearestDebris(Vector3 position)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var d in activeDebris)
        {
            if (d == null) continue;
            float dist = Vector3.Distance(position, d.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = d.transform;
            }
        }
        return nearest;
    }
}
