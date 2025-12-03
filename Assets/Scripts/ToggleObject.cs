using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject target;

    public void Activate()
    {
        target.SetActive(true);
    }

    public void Deactivate()
    {
        target.SetActive(false);
    }
}
