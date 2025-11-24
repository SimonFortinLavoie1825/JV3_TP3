using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankControlsWheelColliders : MonoBehaviour
{
    [SerializeField] private WheelCollider[] poweredWheels;
    [SerializeField] private WheelCollider[] steerWheels;
    [SerializeField] private float brakePower = 100;
    [SerializeField] private float speed = 50;
    [SerializeField] private float maxSpeed = 50;
    [SerializeField] private float brakeFactor = 5000;

    private Rigidbody rb;

    private InputAction moveAction;
    private Vector2 moveValue;

    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.up;
        moveAction = InputSystem.actions.FindAction("Move"); 
    }

    private void FixedUpdate()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        Accelerate();
        Steer();
    }

    private void Accelerate()
    {
        foreach (WheelCollider wheel in poweredWheels)
        {
            wheel.motorTorque = moveValue.y * speed;
        }
    }

    private void Steer()
    {
        foreach (WheelCollider wheel in steerWheels)
        {
            wheel.steerAngle = moveValue.x * 30;
        }
    }
}