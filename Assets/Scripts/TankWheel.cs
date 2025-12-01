using UnityEngine;

public class TankWheel : MonoBehaviour
{
    [SerializeField] private float steerMult = 0;
    
    public float getSteerMult() {
        return steerMult;
    }
}
