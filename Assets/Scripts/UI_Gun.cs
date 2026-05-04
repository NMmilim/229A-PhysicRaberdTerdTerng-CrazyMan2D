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
        ammoText.text = string.Empty;
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
            reloadText.text = "RELOADING...";
    }

    void Update()
    {
        if (isReloading)
            return;

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