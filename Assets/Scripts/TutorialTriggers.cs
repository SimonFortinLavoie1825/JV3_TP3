using UnityEngine;
using UnityEngine.Events;

public class TutorialTriggers : MonoBehaviour
{
    [SerializeField] private UnityEvent onEnter;

    void OnTriggerEnter()
    {
        onEnter.Invoke();

        gameObject.SetActive(false);
    }
}
