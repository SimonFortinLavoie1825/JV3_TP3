using System.Collections.Generic;
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

    private Dictionary<TankWheel, WheelCollider> tankWheelsDict;

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

        tankWheelsDict = new Dictionary<TankWheel, WheelCollider>();
        foreach (TankWheel wheel in tankWheels)
        {
            WheelCollider wc = wheel.GetComponent<WheelCollider>();
            tankWheelsDict.Add(wheel, wc);
        }


        tankWheelsDict.TryGetValue(tankWheels[0], out WheelCollider baseWheel);
        baseSuspensionDistance = baseWheel.suspensionDistance;
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

        
        foreach (KeyValuePair<TankWheel, WheelCollider> kwp in tankWheelsDict)
        {
            WheelCollider wheel = kwp.Value;

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

    private void SlowDown()
    {
        foreach (KeyValuePair<TankWheel, WheelCollider> kwp in tankWheelsDict)
        {
            WheelCollider wheel = kwp.Value;
            wheel.motorTorque = 0;
            wheel.brakeTorque = brakePower / 2.5f;
        }
    }

    private void Move()
    {
        if (Mathf.Sign(currentSpeed) == Mathf.Sign(moveValue.y))
        {
            foreach (KeyValuePair<TankWheel, WheelCollider> kwp in tankWheelsDict)
            {
                WheelCollider wheel = kwp.Value;

                wheel.brakeTorque = 0;
                wheel.motorTorque = CalculateSpeed();
            }
        } else
        {
            foreach (KeyValuePair<TankWheel, WheelCollider> kwp in tankWheelsDict)
            {
                WheelCollider wheel = kwp.Value;
                wheel.motorTorque = 0;
                wheel.brakeTorque = brakePower;
            }
        }
        
    }

    private void Steer()
    {
        foreach (KeyValuePair<TankWheel, WheelCollider> kwp in tankWheelsDict) { 
            WheelCollider wheelCollider = kwp.Value;
            TankWheel wheel = kwp.Key;

            wheelCollider.steerAngle = Mathf.MoveTowards(wheelCollider.steerAngle, moveValue.x * steerAngle * wheel.getSteerMult(), 10);
        }
    }

    private float CalculateSpeed()
    {
        float percentToMaxSpeed = Mathf.Abs(currentSpeed) / maxSpeed;

        return speed * moveValue.y * (1 - percentToMaxSpeed);
    }
}