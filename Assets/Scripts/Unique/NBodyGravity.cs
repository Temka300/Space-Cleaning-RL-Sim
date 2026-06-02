using UnityEngine;

public enum GravityMode
{
    Center,
    Surface
}

public static class NBodyGravity
{
    const float MIN_DISTANCE = 0.5f;

    public static Vector3 Calculate(Vector3 point, Vector3 bodyPosition, float bodyMass, float bodyRadius,
        GravityMode mode, float strengthMultiplier = 1f)
    {
        Vector3 offset = bodyPosition - point;
        float centerDist = offset.magnitude;
        if (centerDist < 0.001f) return Vector3.zero;

        Vector3 direction = offset / centerDist;
        float dist;

        switch (mode)
        {
            case GravityMode.Surface:
                dist = Mathf.Max(centerDist - bodyRadius, MIN_DISTANCE);
                break;
            default:
                dist = Mathf.Max(centerDist, MIN_DISTANCE);
                break;
        }

        return direction * NBodyUniverse.gravitationalConstant * bodyMass * strengthMultiplier / (dist * dist);
    }
}
