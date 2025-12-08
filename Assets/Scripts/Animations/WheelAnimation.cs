using System.Collections.Generic;
using UnityEngine;

public class WheelVisualAnimator : MonoBehaviour
{
    [SerializeField] private TankWheel[] tankWheels;
    [SerializeField] private ParticleSystem driftParticles;

    private Dictionary<Transform, WheelCollider> tankWheelsDict;
    private List<ParticleSystem> driftParticlesList;

    private void Start()
    {
        tankWheelsDict = new Dictionary<Transform, WheelCollider>();

        driftParticlesList = new List<ParticleSystem>();
        for (int i = 0; i < tankWheels.Length; i++)
        {
            ParticleSystem particle = Instantiate(driftParticles, transform);
            particle.Stop();
        }

        foreach (TankWheel wheel in tankWheels)
        {
            Transform mesh = wheel.transform.GetChild(0);
            WheelCollider wc = wheel.GetComponent<WheelCollider>();
            tankWheelsDict.Add(mesh, wc);
        }
    }

    private void Update()
    {
        ChangeWheelPosition();
        ManageWheelParticles();
    }

    private void ChangeWheelPosition()
    {
        foreach (KeyValuePair<Transform, WheelCollider> kwp in tankWheelsDict)
        {
            WheelCollider wheelCollider = kwp.Value;
            Transform wheelMesh = kwp.Key;

            wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion quat);

            wheelMesh.SetPositionAndRotation(pos, quat);
        }
    }

    private void ManageWheelParticles()
    {
        WheelHit wheelHit;

        foreach (KeyValuePair<Transform, WheelCollider> kwp in tankWheelsDict)
        {
            WheelCollider wheelCollider = kwp.Value;
            if (wheelCollider.GetGroundHit(out wheelHit))
            {
                if (Mathf.Abs(wheelHit.sidewaysSlip) >= 0.25f)
                {
                    driftParticles.Play();
                } else
                {
                }
            }
        }
    }
}