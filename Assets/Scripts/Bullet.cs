using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem.HID;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float bulletVelocity = 200f;
    [SerializeField] private GameObject[] hitParticle;

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
        Debug.Log("Bullet, TARRRGET HIT: " + betweenFrameHitInfo.collider.name);
        float angle = CalculateCollisionAngle(rb.linearVelocity, betweenFrameHitInfo.normal);

        // Instantiate and play the hit particle effect
        SpawnParticles();
        
        if (!betweenFrameHitInfo.collider.gameObject.CompareTag("Player"))
        {
            Debug.DrawLine(betweenFrameHitInfo.point - transform.forward * 5, betweenFrameHitInfo.point + transform.forward * 100, Color.white, 5f);
        }
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
        foreach (GameObject particle in hitParticle)
        {
            GameObject hitParticleGO = Instantiate(particle, betweenFrameHitInfo.point, Quaternion.LookRotation(betweenFrameHitInfo.normal));
            ParticleSystem ps = hitParticleGO.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(hitParticleGO, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(hitParticleGO, 2f); // fallback in case no ParticleSystem is found
            }
        }
    }

    private void DestroyBullet()
    {
        currentLife = 0f;
        trail.Clear();
        gameObject.SetActive(false);
    }
}
