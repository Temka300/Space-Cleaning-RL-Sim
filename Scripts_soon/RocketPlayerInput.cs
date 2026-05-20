using UnityEngine;

[RequireComponent(typeof(RocketMovement))]
public class RocketPlayerInput : MonoBehaviour
{
    private RocketMovement movement;

    void Awake()
    {
        movement = GetComponent<RocketMovement>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (movement.movementType == RocketMovementType.AdaptiveRotation)
            movement.MoveRocketDirection(new Vector2(h, v));
        else
            movement.MoveRocket(v, h);
    }
}
