using UnityEngine;

public class LootPickup : MonoBehaviour
{
    [SerializeField] private int value = 10;
    [SerializeField] private AudioClip pickupSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Add score
        ScoreManager.Instance.AddScore(value);

        // Play sound
        if (pickupSound)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}