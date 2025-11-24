using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankControls : MonoBehaviour
{
    [SerializeField] private Transform[] poweredWheels;
    [SerializeField] private float brakePower = 100;
    [SerializeField] private float speed = 50;
    [SerializeField] private float maxSpeed = 50;
    [SerializeField] private float tireGripFactor = 1;
    [SerializeField] private float brakeFactor = 5000;

    private Rigidbody rb;
    private GroundedManager groundManager;

    private InputAction moveAction;
    private Vector2 moveValue;

    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        groundManager = GetComponent<GroundedManager>();
        moveAction = InputSystem.actions.FindAction("Move"); 
    }

    private void FixedUpdate()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (moveValue.magnitude != 0)
        {
            accelerate();
        }
    }

    private void accelerate()
    {
        foreach (Transform wheel in poweredWheels)
        {
            rb.AddForceAtPosition(transform.forward * moveValue.y * speed, wheel.position);
        }
    }

    private void slowDown(IEnumerable affectedWheels)
    {
        foreach (Transform wheel in affectedWheels)
        {
            Vector3 wheelWorldVelocity = rb.GetPointVelocity(wheel.position);
            float wheelVelocity = Vector3.Dot(wheel.forward, wheelWorldVelocity);

            rb.AddForceAtPosition(-wheel.forward * Mathf.Sign(wheelVelocity) * tireGripFactor * brakeFactor, wheel.position);
            Debug.DrawLine(wheel.position, wheel.position + (-wheel.forward * Mathf.Sign(wheelVelocity) * tireGripFactor * brakeFactor / 5000), Color.yellow);
        }
    }
}