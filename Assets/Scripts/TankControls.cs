using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankControls : MonoBehaviour
{
    [SerializeField] private WheelCollider[] poweredWheels;
    [SerializeField] private WheelCollider[] steerWheels;
    [SerializeField] private float brakePower = 100;
    [SerializeField] private float speed = 50;
    [SerializeField] private float maxSpeed = 50;
    [SerializeField] private float brakeFactor = 5000;
    [SerializeField] private float steerAngle = 15;
    [SerializeField] private float jumpForce = 5;

    private Rigidbody rb;

    private InputAction moveAction;
    private Vector2 moveValue;

    private InputAction jumpAction;
    private float jumpValue;

    private float currentSpeed;

    private float baseSuspensionDistance;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        baseSuspensionDistance = poweredWheels[0].suspensionDistance;
    }

    private void FixedUpdate()
    {
        moveValue = moveAction.ReadValue<Vector2>();
        jumpValue = jumpAction.ReadValue<float>();

        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        Accelerate();
        Steer();

        foreach (WheelCollider wheel in poweredWheels)
        {
            if (jumpValue == 1)
            {
                wheel.suspensionDistance = baseSuspensionDistance * jumpForce;
            }
            else
            {
                wheel.suspensionDistance = baseSuspensionDistance;
            }
        }
    }

    private void Accelerate()
    {
        foreach (WheelCollider wheel in poweredWheels)
        {
            wheel.motorTorque = CalculateSpeed();
        }
    }

    private void Steer()
    {
        foreach (WheelCollider wheel in steerWheels)
        {
            wheel.steerAngle = moveValue.x * steerAngle;
        }
    } 

    private float CalculateSpeed()
    {
        float percentToMaxSpeed = currentSpeed / maxSpeed;
        Debug.Log(currentSpeed / maxSpeed);

        return speed * moveValue.y * (1 - percentToMaxSpeed);
    }
}