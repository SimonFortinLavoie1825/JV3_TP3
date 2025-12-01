using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TurnTurret : MonoBehaviour
{
    [SerializeField] private float turretRotationSpeed;
    [SerializeField] private float gunElevationSpeed;
    [SerializeField] private float gunDepression;
    [SerializeField] private float gunElevation;
    [SerializeField] private Transform cameraLook;

    private Transform targetRotation;
    private Transform turret;

    private Transform barrel;

    private LayerMask ignoreMask;
    RaycastHit cameraLookPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetRotation = GameObject.Find("TargetRotation").transform;
        turret = GameObject.Find("Turret").transform;
        barrel = turret.GetChild(0);
        ignoreMask = LayerMask.GetMask("Player");

    }

    // Update is called once per frame
    void Update()
    {
        float newY = turret.localEulerAngles.y;
        float newX = barrel.localEulerAngles.x;
        Physics.Raycast(cameraLook.position + Vector3.up * 5, cameraLook.forward, out cameraLookPoint, 1000, ~ignoreMask);
        Vector3 lookPoint = cameraLookPoint.point;

        if (cameraLookPoint.distance == 0 || cameraLookPoint.distance >= 100)
        {
            lookPoint = cameraLook.position + cameraLook.TransformDirection(Vector3.forward) * 100;
        }

        targetRotation.LookAt(lookPoint);

        newY = Mathf.MoveTowardsAngle(turret.localEulerAngles.y, targetRotation.localEulerAngles.y, turretRotationSpeed * Time.deltaTime);
        newX = Mathf.MoveTowardsAngle(barrel.localEulerAngles.x, targetRotation.localEulerAngles.x, gunElevationSpeed * Time.deltaTime);

        //Use a -180 to 180 range instead of 0 to 360
        if (newX > 180) newX -= 360;

        //Unity reverses the angles so we need to invert the value for it to be accurate (Idk why it works like that ngl)
        newX = Mathf.Clamp(newX, -gunElevation, gunDepression);

        turret.localEulerAngles = new Vector3(turret.localEulerAngles.x, newY, turret.localEulerAngles.z);
        barrel.localEulerAngles = new Vector3(newX, barrel.localEulerAngles.y, barrel.localEulerAngles.z);
    }
}
