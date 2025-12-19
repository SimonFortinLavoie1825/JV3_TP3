using System.Collections.Generic;
using UnityEngine;

public class WheelVisualAnimator : MonoBehaviour
{
    [SerializeField] private TankWheel[] tankWheels;

    private void Start()
    {

    }

    private void Update()
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
        WheelHit wheelHit;

        foreach (TankWheel wheel in tankWheels)
        {
            if (wheel.wheelCollider.GetGroundHit(out wheelHit))
            {
                if (Mathf.Abs(wheelHit.sidewaysSlip) >= 0.25f)
                {
                    wheel.driftParticles.Play();
                } else
                {
                    wheel.driftParticles.Stop();
                }
            }
        }
    }
}
