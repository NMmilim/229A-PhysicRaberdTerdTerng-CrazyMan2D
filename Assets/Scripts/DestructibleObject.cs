using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public GameObject prefab;
    public float weight; // higher = more common
}

public class DestructibleObject : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float health = 50f;

    [Header("Detection")]
    [SerializeField] private float explosionDetectionRadius = 3f;

    [Header("Loot Table")]
    [SerializeField] private LootEntry[] lootTable;
    [SerializeField] private int minDrop = 1;
    [SerializeField] private int maxDrop = 3;
    [SerializeField] private float dropChance = 1f; // 1 = always drop

    [Header("VFX")]
    [SerializeField] private GameObject destroyVFX;

    [Header("Audio")]
    [SerializeField] private AudioClip destroySound;
    [SerializeField] private AudioSource audioSource;

    private bool destroyed;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (destroyed) return;

        float impact = collision.relativeVelocity.magnitude;

        if (collision.collider.CompareTag("Grenade") && impact > 2f)
        {
            TakeDamage(100f);
        }
    }

    void Update()
    {
        if (destroyed) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionDetectionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Explosion"))
            {
                TakeDamage(100f);
                break;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (destroyed) return;

        health -= amount;

        if (health <= 0f)
        {
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        destroyed = true;

        // SOUND
        if (destroySound != null)
            AudioSource.PlayClipAtPoint(destroySound, transform.position);

        // VFX
        if (destroyVFX)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);

        // DROP LOOT
        if (Random.value <= dropChance)
        {
            int dropCount = Random.Range(minDrop, maxDrop + 1);

            for (int i = 0; i < dropCount; i++)
            {
                if (lootTable.Length == 0) break;

                GameObject loot = GetRandomLoot();

                if (loot == null) continue;

                Vector2 offset = Random.insideUnitCircle * 0.5f;

                Instantiate(loot, (Vector2)transform.position + offset, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }

    GameObject GetRandomLoot()
    {
        float totalWeight = 0f;

        foreach (var entry in lootTable)
            totalWeight += entry.weight;

        float random = Random.Range(0, totalWeight);

        float current = 0f;

        foreach (var entry in lootTable)
        {
            current += entry.weight;

            if (random <= current)
                return entry.prefab;
        }

        return null;
    }
}