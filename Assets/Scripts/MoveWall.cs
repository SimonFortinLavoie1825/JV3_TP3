using UnityEngine;

public class MoveWall : MonoBehaviour
{
    [SerializeField] private Vector3 moveOffset;
    [SerializeField] private float speed = 3f;

    private bool activated;
    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position + moveOffset;
    }

    public void Activate()
    {
        activated = true;
    }

    private void Update()
    {
        if (!activated) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }
}
