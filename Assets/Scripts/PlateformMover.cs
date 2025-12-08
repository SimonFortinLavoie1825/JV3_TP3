using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 2f;

    private bool activated;

    public void Activate()
    {
        activated = true;
    }

    private void Update()
    {
        if (activated)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                endPoint.position,
                speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPoint.position,
                speed * Time.deltaTime);
        }
    }
}
