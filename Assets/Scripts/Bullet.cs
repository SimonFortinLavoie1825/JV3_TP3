using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem.HID;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float bulletVelocity = 200f;
    [SerializeField] private GameObject[] hitParticlePrefabs;

    private ParticleSystem[] hitParticle;

    private TrailRenderer trail;
    private Ray betweenFrameRay;
    private RaycastHit betweenFrameHitInfo;
    private Vector3 lastFramePosition;

    private Rigidbody rb;

    private float currentLife = 0f;
    private LayerMask ignoreMask;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody>();
        ignoreMask = ~LayerMask.GetMask("Player", "Bullet");

        hitParticle = new ParticleSystem[hitParticlePrefabs.Length];
        for (int i = 0; i < hitParticlePrefabs.Length; i++)
        {
            ParticleSystem newParticle = Instantiate(hitParticlePrefabs[i]).GetComponent<ParticleSystem>();
            newParticle.Stop();
            hitParticle[i] = newParticle;
        }
    }

    private void FixedUpdate()
    {
        currentLife += Time.fixedDeltaTime;
        if (currentLife >= lifeTime)
        {
            DestroyBullet();
        }

        betweenFrameRay = new Ray(lastFramePosition, transform.position - lastFramePosition);
        Debug.DrawRay(betweenFrameRay.origin, betweenFrameRay.direction * (transform.position - lastFramePosition).magnitude, Color.red, 5f);

        if (Physics.Raycast(betweenFrameRay, out betweenFrameHitInfo, Vector3.Distance(transform.position, lastFramePosition), ignoreMask) && betweenFrameHitInfo.collider.gameObject != gameObject) 
        {
            OnHit();
        }

        lastFramePosition = transform.position;
    }

    void OnHit()
    {
        Debug.Log("Bullet, TARGET HIT: " + betweenFrameHitInfo.collider.name);

        if (betweenFrameHitInfo.collider.TryGetComponent<Target>(out Target target))
            target.OnHit();

        float angle = CalculateCollisionAngle(rb.linearVelocity, betweenFrameHitInfo.normal);

        SpawnParticles();

        DestroyBullet();
    }


    public void ShootBullet()
    {
        rb.linearVelocity = transform.forward * bulletVelocity;
    }

    public void ResetBullet(Transform newTransform, Vector3 newVelocity)
    {
        rb.linearVelocity = newVelocity;
        transform.SetPositionAndRotation(newTransform.transform.position, newTransform.transform.rotation);
        lastFramePosition = newTransform.transform.position;
    }

    private float CalculateCollisionAngle(Vector3 velocity, Vector3 normal)
    {
        Vector3 vNorm = velocity.normalized;
        Vector3 nNorm = normal.normalized;
        return Vector3.Angle(vNorm, -nNorm);
    }

    private void SpawnParticles()
    {
        for (int i = 0; i < hitParticle.Length; i++)
        {
            hitParticle[i].transform.position = betweenFrameHitInfo.point;
            hitParticle[i].transform.rotation = Quaternion.LookRotation(betweenFrameHitInfo.normal);
            hitParticle[i].Play();
        }
    }

    private void DestroyBullet()
    {
        currentLife = 0f;
        trail.Clear();
        gameObject.SetActive(false);
    }
}
