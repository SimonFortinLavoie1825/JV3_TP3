using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootGun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform barrelPosition;
    [SerializeField] private float recoil = 50f;
    [SerializeField] private float reloadTime = 2.5f;
    [SerializeField] private GameObject[] gunParticlePrefabs;

    private GameObject[] bulletList;
    private Rigidbody rb;
    private int bulletListSize = 10;
    private float currentReload;

    private InputAction shootAction;
    private float shootValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentReload = reloadTime;
        rb = GetComponent<Rigidbody>();
        shootAction = InputSystem.actions.FindAction("Shoot");

        bulletList = new GameObject[bulletListSize];
        for (int i = 0; i < bulletListSize; i++)
        {
            GameObject newBullet = Instantiate(bullet);
            newBullet.SetActive(false);
            bulletList[i] = newBullet;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        shootValue = shootAction.ReadValue<float>();
        currentReload += Time.fixedDeltaTime; 


        //shoot
        if (shootValue == 1 && currentReload >= reloadTime)
        {
            //Reset reload
            currentReload = 0f;

            //Get bullet from pool

            GameObject newBullet = getAvailableBullet();
            //Shoot it
            newBullet.GetComponent<Bullet>().ShootBullet();

            // Instantiate and play each particle effect, then destroy after it finishes
            foreach (GameObject particlePrefab in gunParticlePrefabs)
            {
                GameObject particleGO = Instantiate(particlePrefab, barrelPosition.position, barrelPosition.rotation);
                ParticleSystem ps = particleGO.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    Destroy(particleGO, ps.main.duration + ps.main.startLifetime.constantMax);
                }
                else
                {
                    Destroy(particleGO, 2f); // fallback if no ParticleSystem found
                }
            }

            //Move tank back
            rb.AddForceAtPosition(-newBullet.transform.forward * recoil, barrelPosition.position);
        }
    }

    private GameObject getAvailableBullet()
    {
        for (int i = 0; i < bulletListSize; i++)
        {
            if (!bulletList[i].activeInHierarchy)
            {
                bulletList[i].SetActive(true);
                bulletList[i].GetComponent<Bullet>().ResetBullet(barrelPosition, rb.linearVelocity);
                return bulletList[i];
            }
        }
        return null;
    }
}
