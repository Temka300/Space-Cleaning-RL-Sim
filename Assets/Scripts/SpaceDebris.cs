using UnityEngine;

public class SpaceDebris : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    public void SetDriftVelocity(Vector3 velocity)
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = velocity;
        }
    }

    public void MakeStatic()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
}
