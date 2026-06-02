using UnityEngine;

public enum GravityFalloffMode
{
    InverseDistance,
    InverseSquare
}

public class GravitySource : MonoBehaviour
{
    [Header("Gravity Properties")]
    public float mass = 50f;
    public float customIntensity = 1f;
    public float influenceRange = 30f;
    public float minimumDistance = 1f;
    public GravityFalloffMode falloffMode = GravityFalloffMode.InverseSquare;
    [Tooltip("Keeps legacy top-down gravity behavior. Disable for scenes that use vertical/Y-axis gameplay.")]
    public bool ignoreVerticalOffset = true;

    [Header("Spawning")]
    [Tooltip("Debris and agent will not spawn within this radius.")]
    public float spawnExclusionRadius = 3f;

    public Vector3 ComputeAcceleration(Vector3 targetPosition, float gravityConstant)
    {
        Vector3 delta = transform.position - targetPosition;
        if (ignoreVerticalOffset)
            delta.y = 0f;
        float distance = delta.magnitude;

        if (distance > influenceRange)
            return Vector3.zero;

        float clampedDist = Mathf.Max(distance, minimumDistance);
        Vector3 direction = distance > 0.0001f ? delta / distance : Vector3.zero;

        switch (falloffMode)
        {
            case GravityFalloffMode.InverseDistance:
                return direction * customIntensity / clampedDist;
            case GravityFalloffMode.InverseSquare:
                return direction * gravityConstant * mass * customIntensity / (clampedDist * clampedDist);
            default:
                return Vector3.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, influenceRange);
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, minimumDistance);
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, spawnExclusionRadius);
    }
}
