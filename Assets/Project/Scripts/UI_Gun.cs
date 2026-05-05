using UnityEngine;
using TMPro;

public class UI_Gun : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;

    [Header("Low Ammo")]
    [SerializeField] private int lowAmmoThreshold = 2;

    private bool isLowAmmo;
    private bool isReloading;

    void Start()
    {
        ammoText.text = "6 / 6";
        reloadText.gameObject.SetActive(false);
    }

    public void UpdateAmmo(int current, int max)
    {
        ammoText.text = $"{current} / {max}";
        isLowAmmo = current <= lowAmmoThreshold;
    }

    public void SetReloading(bool state)
    {
        isReloading = state;

        reloadText.gameObject.SetActive(state);

        if (state)
        {
            reloadText.text = "RELOADING...";
        }
    }

    void Update()
    {
        // RELOADING EFFECT (highest priority)
        if (isReloading)
        {
            reloadText.color = Color.Lerp(
                Color.red,
                Color.white,
                Mathf.PingPong(Time.time * 5f, 1f)
            );
            return;
        }

        // LOW AMMO EFFECT
        if (isLowAmmo)
        {
            ammoText.color = Color.Lerp(
                Color.red,
                Color.white,
                Mathf.PingPong(Time.time * 5f, 1f)
            );
        }
        else
        {
            ammoText.color = Color.white;
        }
    }
}