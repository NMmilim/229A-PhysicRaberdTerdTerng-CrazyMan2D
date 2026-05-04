using UnityEngine;
using System.Collections;

public class GrenadeProjectile : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private GameObject explosionVFX;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip impactSound;

    [Header("Explosion")]
    [SerializeField] private float fuseTime = 2f;
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private float explosionForce = 700f;

    [Header("Arming")]
    [SerializeField] private float armingTime = 0.2f;

    private Rigidbody2D rb;
    private bool armed;
    private bool exploded;
    private bool canExplode;

    Coroutine fuseRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        armed = false;
        canExplode = false;

        Invoke(nameof(Arm), armingTime);
        StartCoroutine(Fuse());
    }

    void Arm()
    {
        armed = true;
        canExplode = true;
    }

    IEnumerator Fuse()
    {
        yield return new WaitForSeconds(fuseTime);

        if (canExplode)
            Explode();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canExplode || exploded) return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        bool isGround = collision.collider.CompareTag("Ground");
        bool isPlayer = collision.collider.CompareTag("Player");

        // Only play sound on real impact
        if (impactSpeed > 2f)
        {
            audioSource.PlayOneShot(impactSound);
        }

        // explosion trigger
        if (isPlayer)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded || !armed) return;

        exploded = true;

        CancelInvoke();

        // VFX
        Instantiate(explosionVFX, transform.position, Quaternion.identity);

        // spawn sound object so it is NOT destroyed with grenade
        GameObject audioObj = new GameObject("ExplosionSound");
        AudioSource src = audioObj.AddComponent<AudioSource>();

        src.clip = explosionSound;
        src.spatialBlend = 1f; // 3D sound
        src.Play();

        Destroy(audioObj, explosionSound.length);

        // physics
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
        }

        // explosion force
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

        Destroy(gameObject, 0.05f);
    }

    void OnDisable()
    {
        if (fuseRoutine != null)
            StopCoroutine(fuseRoutine);
    }
}