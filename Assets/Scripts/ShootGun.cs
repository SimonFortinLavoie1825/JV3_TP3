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
    private ParticleSystem[][] gunParticlesList;

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

        gunParticlesList = new ParticleSystem[gunParticlePrefabs.Length][];
        for (int i = 0; i < gunParticlePrefabs.Length; i++)
        {
            gunParticlesList[i] = new ParticleSystem[bulletListSize];
            for (int i2 = 0; i2 < bulletListSize; i2++)
            {
                ParticleSystem newParticle = Instantiate(gunParticlePrefabs[i]).GetComponent<ParticleSystem>();
                newParticle.Stop();
                gunParticlesList[i][i2] = newParticle;
            }
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
            for (int i = 0; i < gunParticlePrefabs.Length; i++)
            {
                ParticleSystem ps = getAvailableGunParticle(i);
                ps.transform.SetPositionAndRotation(barrelPosition.position, barrelPosition.rotation);
                ps.Play();
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

    private ParticleSystem getAvailableGunParticle(int index)
    {
        for (int i = 0; i < bulletListSize; i++)
        {
            if (!gunParticlesList[index][i].isPlaying)
            {
                return gunParticlesList[index][i];
            }
        }
        return null;
    }
}
