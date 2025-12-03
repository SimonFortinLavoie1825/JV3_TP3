using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    public UnityEvent onHit;

    public bool disableAfterHit = false;

    public void OnHit()
    {
        Debug.Log($"{name} was hit!");

        onHit.Invoke();

        if (disableAfterHit)
            gameObject.SetActive(false);
    }
}
