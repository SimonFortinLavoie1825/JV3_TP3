using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WheelsSuspensions : MonoBehaviour
{
    [SerializeField] private float suspensionRestDistance = 0.8f;
    [SerializeField] private float springStrength;
    [SerializeField] private float springDamping;
    [SerializeField] private float maxSuspensionDistance = 0.25f;

    //Wheels arrays
    [SerializeField] private Transform[] wheels;
    private Dictionary<Transform, Transform> wheelDict = new Dictionary<Transform, Transform>();

    private Rigidbody rb;
    private GroundedManager groundManager;

    private float baseSuspensionRestDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseSuspensionRestDistance = suspensionRestDistance;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        groundManager = GetComponent<GroundedManager>();

        foreach (Transform wheel in wheels)
        {
            wheelDict.Add(wheel, wheel.GetChild(0));
        }
    }

    private void FixedUpdate()
    {
        foreach (KeyValuePair<Transform, Transform> dictElement in wheelDict)
        {
            Transform wheel = dictElement.Key;
            Transform wheelMesh = dictElement.Value;

            if (groundManager.isUnderGround(wheel) || groundManager.isGrounded(wheel))
            {
                Vector3 springDirection = wheel.up;

                Vector3 tireWorldVelocity = rb.GetPointVelocity(wheel.position);

                float offset = suspensionRestDistance - groundManager.getDistance();

                float springVelocity = Vector3.Dot(springDirection, tireWorldVelocity);

                float springForce = (offset * springStrength) - (springVelocity * springDamping);

                rb.AddForceAtPosition(springDirection * springForce, wheel.position);

                Debug.Log("Suspensions: " + offset);
                offset = Mathf.Clamp(offset, -maxSuspensionDistance - suspensionRestDistance, maxSuspensionDistance + suspensionRestDistance);
                
                wheelMesh.position = wheel.position - new Vector3(0, suspensionRestDistance - offset, 0);
            }
        }
    }

    public void setSuspensionRestDistance(float newRestDistance)
    {
        suspensionRestDistance = newRestDistance;
    }

    public void resetSuspensionRestDistance()
    {
        suspensionRestDistance = baseSuspensionRestDistance;
    }
}
