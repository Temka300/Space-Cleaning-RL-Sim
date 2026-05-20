using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SpaceCleaningAgent : Agent
{
    [Header("References")]
    public RocketMovement rocketMovement;
    public EnvironmentManager environmentManager;

    [Header("Agent Settings")]
    public float timePenalty = -0.001f;
    public float collectReward = 1.0f;
    public float wallPenalty = -1.0f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        rocketMovement.ResetRocket(startPosition, startRotation);
        environmentManager.SpawnDebris();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent position (2: x, z)
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        // Agent velocity (2: x, z)
        sensor.AddObservation(rb.linearVelocity.x);
        sensor.AddObservation(rb.linearVelocity.z);

        // Agent forward direction (2: x, z)
        sensor.AddObservation(transform.forward.x);
        sensor.AddObservation(transform.forward.z);

        // Nearest debris relative position (2: x, z)
        Transform nearest = environmentManager.GetNearestDebris(transform.position);
        if (nearest != null)
        {
            Vector3 toDebris = nearest.position - transform.position;
            sensor.AddObservation(toDebris.x);
            sensor.AddObservation(toDebris.z);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }

        // Remaining debris count (normalized)
        sensor.AddObservation((float)environmentManager.RemainingDebris / environmentManager.debrisCount);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float thrust = Mathf.Clamp(actions.ContinuousActions[0], 0f, 1f);
        float rotation = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        rocketMovement.MoveRocket(thrust, rotation);

        // Time penalty
        AddReward(timePenalty);

        // Check if all debris collected
        if (environmentManager.RemainingDebris <= 0)
        {
            AddReward(collectReward); // Bonus for clearing all
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetKey(KeyCode.W) ? 1f : 0f;
        continuous[1] = Input.GetAxis("Horizontal");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Debris"))
        {
            environmentManager.RemoveDebris(collision.gameObject);
            AddReward(collectReward);
        }
        else if (collision.gameObject.CompareTag("Boundary"))
        {
            AddReward(wallPenalty);
            EndEpisode();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Debris"))
        {
            environmentManager.RemoveDebris(other.gameObject);
            AddReward(collectReward);
        }
    }
}
