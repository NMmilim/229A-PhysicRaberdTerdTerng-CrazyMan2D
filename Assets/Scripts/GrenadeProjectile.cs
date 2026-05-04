using System.Collections;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [Header("Arming")]
    [SerializeField] private float armTime = 0.05f;

    [Header("Unarmed Safety")]
    [SerializeField] private float unarmedDestroyDelay = 10f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionForce = 800f;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioSource audioSource;


    [Header("Air Resistance")]
    [SerializeField] private float airResistance = 0.015f;

    private Rigidbody2D rb;

    private bool armed;
    private bool exploded;

    private Vector2 spawnPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        spawnPos = transform.position;
        StartCoroutine(ArmRoutine());
    }

    IEnumerator ArmRoutine()
    {
        yield return new WaitForSeconds(armTime);
        armed = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (exploded) return;

        float speed = collision.relativeVelocity.magnitude;

        if (speed > 2f && impactSound != null)
            audioSource.PlayOneShot(impactSound);

        // UNARMED -> safe delayed destruction
        if (!armed)
        {
            StartCoroutine(UnarmedDestroy());
            return;
        }

        // ARMED -> explode on valid impact
        Explode();
    }

    IEnumerator UnarmedDestroy()
    {
        // prevent multiple triggers
        if (exploded) yield break;
        exploded = true;

        yield return new WaitForSeconds(unarmedDestroyDelay);

        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        if (rb == null || exploded) return;

        Vector2 v = rb.linearVelocity;

        // Only reduce horizontal speed (keeps natural arc)
        Vector2 horizontal = new Vector2(v.x, 0f);
        horizontal *= (1f - airResistance);

        rb.linearVelocity = new Vector2(horizontal.x, v.y);
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionVFX)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        if (explosionSound)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            Rigidbody2D target = hit.attachedRigidbody;

            if (target)
            {
                Vector2 dir = (target.transform.position - transform.position).normalized;
                target.AddForce(dir * explosionForce);
            }
        }

        Destroy(gameObject);
    }
}