using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeLauncher2D : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private UI_Gun ui;

    private Camera cam;

    [Header("Input")]
    [SerializeField] private InputActionReference fire;
    [SerializeField] private InputActionReference reload;

    [Header("Weapon Stats")]
    [SerializeField] private float muzzleVelocity = 75f;
    [SerializeField] private float fireRate = 0.6f;
    [SerializeField] private int magazineSize = 5;
    [SerializeField] private float reloadTime = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;

    private int currentAmmo;
    private bool isReloading;
    private bool canShoot = true;

    private Vector2 mouseScreenPos;

    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;

    void Awake()
{
    cam = Camera.main;
    audioSource = GetComponent<AudioSource>();

    if (audioSource == null)
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    if (audioSource == null)
    {
        Debug.LogError("NO AudioSource found on Gun or children!");
    }
}

    void Start()
    {
        // IMPORTANT: initialize ammo
        currentAmmo = magazineSize;

        if (ui != null)
            ui.UpdateAmmo(currentAmmo, magazineSize);
    }

    void OnEnable()
    {
        fire.action.Enable();
        reload.action.Enable();

        fire.action.performed += OnFire;
        reload.action.performed += OnReload;
    }

    void OnDisable()
    {
        fire.action.performed -= OnFire;
        reload.action.performed -= OnReload;

        fire.action.Disable();
        reload.action.Disable();
    }

    void Update()
    {
        RotateTowardsMouse();
    }

    // ---------------- INPUT ----------------

  

    void OnFire(InputAction.CallbackContext ctx)
    {
        TryFire();
    }

    void OnReload(InputAction.CallbackContext ctx)
    {
        StartReload();
    }

    // ---------------- AIM ----------------

    void RotateTowardsMouse()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f)
        );
        mouseWorld.z = 0;

        Vector2 direction = firePoint.right;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }



    // ---------------- SHOOTING ----------------

    void TryFire()
    {
        if (isReloading || !canShoot) return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        canShoot = false;

        currentAmmo--;

        if (ui != null)
            ui.UpdateAmmo(currentAmmo, magazineSize);

        Fire();

        yield return new WaitForSeconds(fireRate);

        canShoot = true;
    }

    void Fire()
    {
        audioSource.PlayOneShot(fireSound);

        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();

        Vector2 direction = firePoint.right.normalized;

        // slight upward bias for ballistic arc feel
        Vector2 launchDir = (direction + Vector2.up * 0.05f).normalized;

        rb.AddForce(launchDir * muzzleVelocity, ForceMode2D.Impulse);
    }

    // ---------------- RELOAD ----------------

    void StartReload()
    {
        if (isReloading || currentAmmo == magazineSize) return;

        StartCoroutine(ReloadRoutine());

        if (ui != null)
            ui.SetReloading(true);
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;

        audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;

        if (ui != null)
        {
            ui.UpdateAmmo(currentAmmo, magazineSize);
            ui.SetReloading(false);
        }

        isReloading = false;
    }
}