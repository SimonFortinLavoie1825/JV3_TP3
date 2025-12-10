using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootGun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float recoil = 50f;
    [SerializeField] private float reloadTime = 2.5f;
    [SerializeField] private GameObject[] gunParticlePrefabs;

    private Transform barrelPosition;

    private Bullet[] bulletList;
    private Rigidbody rb;
    private int bulletListSize = 10;
    private float currentReload;

    private InputAction shootAction;
    private float shootValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentReload = reloadTime;

        barrelPosition = transform.Find("Turret/Barrel");

        rb = GetComponent<Rigidbody>();
        shootAction = InputSystem.actions.FindAction("Shoot");

        bulletList = new Bullet[bulletListSize];
        for (int i = 0; i < bulletListSize; i++)
        {
            GameObject newBullet = Instantiate(bullet);
            newBullet.SetActive(false);
            bulletList[i] = newBullet.GetComponent<Bullet>();
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

            Bullet newBullet = getAvailableBullet();
            //Shoot it
            newBullet.ShootBullet();

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
            rb.AddForce(-newBullet.transform.forward * recoil);
        }
    }

    private Bullet getAvailableBullet()
    {
        for (int i = 0; i < bulletListSize; i++)
        {
            if (!bulletList[i].gameObject.activeInHierarchy)
            {
                bulletList[i].gameObject.SetActive(true);
                bulletList[i].ResetBullet(barrelPosition, rb.linearVelocity);
                return bulletList[i];
            }
        }
        return null;
    }
}
