using UnityEngine;
using UnityEngine.Events;

public class EndZone : MonoBehaviour
{
    [SerializeField] private UnityEvent endEvent;
    [SerializeField] private ParticleSystem endEffect;

    private GameObject psInstance;

    private void Start()
    {
        if (endEffect != null)
        {
            psInstance = Instantiate(endEffect.gameObject);
            psInstance.transform.position = transform.position;
            psInstance.SetActive(false);
        }
    }

    void OnTriggerEnter()
    {
        endEvent.Invoke();
        psInstance.SetActive(true);

        gameObject.SetActive(false);
    }
}
