using UnityEngine;

public class WheelVisualAnimator : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform[] wheelMeshes;
    [SerializeField] private Transform[] steerPivots;
    [SerializeField] private Transform[] suspensionPivots;

    [SerializeField] private float wheelRadius = 0.4f;

    [SerializeField] private float suspensionExtendAmount = 1.5f;
    [SerializeField] private float suspensionLerpSpeed = 5f;

    private float currentSuspensionOffset = 0f;
    private float targetSuspensionOffset = 0f;
    private float[] pivotBaseY;

    private float[] wheelRotation;
    public float currentSteerAngle;


    private void Start()
    {
        wheelRotation = new float[wheelMeshes.Length];

        pivotBaseY = new float[suspensionPivots.Length];
        for (int i = 0; i < suspensionPivots.Length; i++)
        {
            pivotBaseY[i] = suspensionPivots[i].localPosition.y;
        }
    }


    private void Update()
    {
        AnimateWheelSpin();
        AnimateSteering();
        AnimateSuspension();
    }

    private void AnimateWheelSpin()
    {
        float forwardSpeed = rb.transform.InverseTransformDirection(rb.linearVelocity).z;
        float distance = forwardSpeed * Time.deltaTime;
        float rotationAmount = (distance / wheelRadius) * Mathf.Rad2Deg;

        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            wheelRotation[i] += rotationAmount;
            wheelMeshes[i].localRotation = Quaternion.Euler(wheelRotation[i], 0f, 0f);
        }
    }

    private void AnimateSteering()
    {
        Quaternion steerRot = Quaternion.Euler(0f, currentSteerAngle, 0f);

        foreach (Transform pivot in steerPivots)
        {
            pivot.localRotation = steerRot;
        }
    }

    private void AnimateSuspension()
    {
        currentSuspensionOffset = Mathf.Lerp(
            currentSuspensionOffset,
            targetSuspensionOffset,
            suspensionLerpSpeed * Time.deltaTime
        );

        for (int i = 0; i < suspensionPivots.Length; i++)
        {
            Transform pivot = suspensionPivots[i];

            pivot.localPosition = new Vector3(
                pivot.localPosition.x,
                pivotBaseY[i] + currentSuspensionOffset,
                pivot.localPosition.z
            );
        }
    }


    public void SetSuspensionExtended(bool extended)
    {
        targetSuspensionOffset = extended ? -suspensionExtendAmount : 0f;
    }
}
