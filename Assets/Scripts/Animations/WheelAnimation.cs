using System.Collections.Generic;
using UnityEngine;

public class WheelVisualAnimator : MonoBehaviour
{
    [SerializeField] private TankWheel[] tankWheels;

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        ChangeWheelPosition();
        ManageWheelParticles();
    }

    private void ChangeWheelPosition()
    {
        foreach (TankWheel wheel in tankWheels)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 targetPos, out Quaternion targetQuat);

            float lerpSpeed = 15f;

            wheel.wheelMesh.position = Vector3.Lerp(
                wheel.wheelMesh.position,
                targetPos,
                Time.deltaTime * lerpSpeed
            );

            wheel.wheelMesh.rotation = Quaternion.Slerp(
                wheel.wheelMesh.rotation,
                targetQuat,
                Time.deltaTime * lerpSpeed
            );
        }
    }

    private void ManageWheelParticles()
    {
        foreach (TankWheel wheel in tankWheels)
        {
            if (wheel.wheelCollider.GetGroundHit(out WheelHit wheelHit))
            {
                if (Mathf.Abs(wheelHit.sidewaysSlip) >= 0.5f || (Mathf.Abs(wheelHit.forwardSlip) >= 0.975f && Mathf.Abs(wheelHit.forwardSlip) <= 1.0f))
                {
                    wheel.driftParticles.Play();
                    continue;
                } 
            }
            wheel.driftParticles.Stop();
        }
    }
}
