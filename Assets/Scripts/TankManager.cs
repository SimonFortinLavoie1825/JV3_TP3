using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankManager : MonoBehaviour
{
    [SerializeField] private static TankManager Instance;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera freeLookCam;

    [Header("Tanks")]
    [SerializeField] private GameObject greenTank;
    [SerializeField] private GameObject yellowTank;

    [Header("Spawn Points")]
    [SerializeField] private Transform spawnGreen;
    [SerializeField] private Transform spawnYellow;

    [Header("Reset Point")]
    [SerializeField] private Transform resetPlatform;

    [Header("Materials")]
    public Material tankMatBlue;
    public Material tankMatGreen;

    private GameObject activeTank;

    private InputAction switchLeft;
    private InputAction switchRight;

    private bool yellowActive = false;

    private void Awake()
    {
        Instance = this;

        yellowTank.SetActive(false);

        activeTank = greenTank;

        switchLeft = InputSystem.actions.FindAction("SwitchLeft");
        switchRight = InputSystem.actions.FindAction("SwitchRight");

        SetTankScriptsEnabled(greenTank, true);
        SetTankScriptsEnabled(yellowTank, false);
    }

    private void Update()
    {
        if (!yellowActive) return;

        if (switchLeft.WasPerformedThisFrame())
            SetActiveTank(greenTank);

        if (switchRight.WasPerformedThisFrame())
            SetActiveTank(yellowTank);
    }

    private void SetActiveTank(GameObject newTank)
    {
        if (activeTank == newTank) return;

        SetTankScriptsEnabled(activeTank, false);
        SetTankScriptsEnabled(newTank, true);
        SetCameraTarget(newTank);

        activeTank = newTank;
    }

    public void DuplicateTank()
    {
        if (yellowActive) return;

        yellowActive = true;

        SpawnTankSafely(greenTank, spawnGreen);
        yellowTank.SetActive(true);
        SpawnTankSafely(yellowTank, spawnYellow);
        ApplyMaterialToTank(greenTank, tankMatBlue);

        SetActiveTank(greenTank);
    }


    public void ReturnToSingleTank()
    {
        yellowTank.SetActive(false);
        SetTankScriptsEnabled(yellowTank, false);

        yellowActive = false;

        SpawnTankSafely(greenTank, resetPlatform);

        ApplyMaterialToTank(greenTank, tankMatGreen);
        SetTankScriptsEnabled(greenTank, true);
        SetCameraTarget(greenTank);

        activeTank = greenTank;
    }

    private void SpawnTankSafely(GameObject tank, Transform spawnPoint)
    {
        Rigidbody rb = tank.GetComponent<Rigidbody>();

        rb.isKinematic = true;

        Vector3 safePos = spawnPoint.position + Vector3.up * 0.9f;

        tank.transform.SetPositionAndRotation(safePos, spawnPoint.rotation);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        StartCoroutine(ReenablePhysics(rb));
    }
    private System.Collections.IEnumerator ReenablePhysics(Rigidbody rb)
    {
        yield return null;
        rb.isKinematic = false;
    }

    private void ApplyMaterialToTank(GameObject tank, Material newMaterial)
    {
        MeshRenderer[] renderers = tank.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = newMaterial;
        }
    }

    private void SetTankScriptsEnabled(GameObject tank, bool enabled)
    {
        MonoBehaviour[] scripts = tank.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = enabled;
        }
    }

    private void SetCameraTarget(GameObject tank)
    {
        if (freeLookCam == null) return;

        freeLookCam.Follow = tank.transform;

        Transform turret = tank.transform.Find("Turret");

        if (turret != null)
            freeLookCam.LookAt = turret;
        else
            freeLookCam.LookAt = tank.transform;
    }
}
