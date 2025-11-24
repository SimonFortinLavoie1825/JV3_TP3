using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GroundedManager : MonoBehaviour
{
    [SerializeField] private float maxRaycastDistance = 1.2f;
    [SerializeField] private LayerMask ignoreMask;
    private RaycastHit wheelRay;

    void Start()
    {
    }

    public bool isGrounded(Transform currentWheel)
    {
        Physics.Raycast(currentWheel.position, -currentWheel.up, out wheelRay, int.MaxValue, ~ignoreMask);
        return wheelRay.distance <= maxRaycastDistance && wheelRay.distance != 0;
    }

    public bool isUnderGround(Transform currentWheel)
    {
        Physics.Raycast(currentWheel.position, currentWheel.up, out wheelRay, 3, ~ignoreMask);
        return wheelRay.distance <= 1 && wheelRay.distance != 0;
    }

    public float getDistance() { return wheelRay.distance; }
}
