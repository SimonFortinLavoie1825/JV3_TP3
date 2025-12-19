using UnityEngine;

public class TankWheel : MonoBehaviour
{
    public float steerMult = 0;
    public ParticleSystem driftParticles;
    public Transform wheelMesh;
    public WheelCollider wheelCollider;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelMesh = transform.GetChild(0);
        
        driftParticles = Instantiate(driftParticles, transform);
        driftParticles.Stop();
    }
}
