using System.Collections.Generic;
using UnityEngine;

public class WheelVisualAnimator : MonoBehaviour
{
    [SerializeField] private TankWheel[] tankWheels;

    private Dictionary<Transform, WheelCollider> tankWheelsDict;

    private void Start()
    {
        tankWheelsDict = new Dictionary<Transform, WheelCollider>();

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
}
