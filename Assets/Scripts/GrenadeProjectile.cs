using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [Header("Lifetime Safety")]
    [SerializeField] private float maxLifeTime = 5f;

    [Header("VFX")]
    [SerializeField] private GameObject explosionVFX;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip impactSound;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private float explosionForce = 700f;

    [Header("Safety")]
    [SerializeField] private float minImpactSpeed = 4f;
    [SerializeField] private float armTime = 0.1f;

    private Rigidbody2D rb;
    private bool exploded;
    private float spawnTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        spawnTime = Time.time;
        Invoke(nameof(AutoDestroy), maxLifeTime);
    }
    void AutoDestroy()
    {
        if (!exploded)
        {
            Destroy(gameObject, 0.25f);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (exploded) return;

        // ignore very early collision (spawn overlap fix)
        if (Time.time - spawnTime < armTime) return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        // play impact sound only if real hit
        if (impactSpeed > 2f && impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        // safety check: only explode if impact is strong enough
        if (impactSpeed >= minImpactSpeed)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        CancelInvoke(); // stop AutoDestroy

        // VFX
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        // Sound (safe 3D)
        if (explosionSound != null)
        {
            GameObject audioObj = new GameObject("ExplosionSound");
            AudioSource src = audioObj.AddComponent<AudioSource>();

            src.clip = explosionSound;
            src.spatialBlend = 1f;
            src.Play();

            Destroy(audioObj, explosionSound.length);
        }

        // physics explosion
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            Rigidbody2D targetRb = hit.attachedRigidbody;

            if (targetRb != null)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                targetRb.AddForce(dir * explosionForce);
            }
        }

        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
        }

        Destroy(gameObject, 0.05f);
    }
}