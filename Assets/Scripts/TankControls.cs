using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankControls : MonoBehaviour
{
    [SerializeField] private TankWheel[] tankWheels;
    [SerializeField] private float brakePower = 100;
    [SerializeField] private float speed = 50;
    [SerializeField] private float maxSpeed = 50;
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
        rb.centerOfMass = Vector3.down;
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        baseSuspensionDistance = tankWheels[0].wheelCollider.suspensionDistance;
    }

    private void FixedUpdate()
    {
        moveValue = moveAction.ReadValue<Vector2>();
        jumpValue = jumpAction.ReadValue<float>();

        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (moveValue.y != 0)
        {
            Move();
        } else
        {
            SlowDown();
        }
        Steer();

        
        foreach (TankWheel wheel in tankWheels)
        {
            if (jumpValue == 1)
            {
                wheel.wheelCollider.suspensionDistance = baseSuspensionDistance * jumpForce;
            }
            else
            {
                wheel.wheelCollider.suspensionDistance = baseSuspensionDistance;
            }
        }
        
    }

    private void SlowDown()
    {
        foreach (TankWheel wheel in tankWheels)
        {
            wheel.wheelCollider.motorTorque = 0;
            wheel.wheelCollider.brakeTorque = brakePower / 2.5f;
        }
    }

    private void Move()
    {
        if (Mathf.Sign(currentSpeed) == Mathf.Sign(moveValue.y) || (-1 < currentSpeed && currentSpeed < 1))
        {
            foreach (TankWheel wheel in tankWheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
                wheel.wheelCollider.motorTorque = CalculateSpeed();
            }
        } else
        {
            foreach(TankWheel wheel in tankWheels)
            {
                wheel.wheelCollider.motorTorque = 0;
                wheel.wheelCollider.brakeTorque = brakePower;
            }
        }
    }

    private void Steer()
    {
        foreach (TankWheel wheel in tankWheels)
        {
            wheel.wheelCollider.steerAngle = Mathf.MoveTowards(wheel.wheelCollider.steerAngle, moveValue.x * steerAngle * wheel.steerMult, 10);
        }
    }

    private float CalculateSpeed()
    {
        float percentToMaxSpeed = Mathf.Abs(currentSpeed) / maxSpeed;

        return speed * moveValue.y * (1 - percentToMaxSpeed);
    }
}