using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeLauncher2D : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject grenadePrefab;

    private Camera cam;

    [Header("Input")]
    [SerializeField] private InputActionReference aim;
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
    }

    void OnEnable()
    {
        aim.action.Enable();
        fire.action.Enable();
        reload.action.Enable();

        aim.action.performed += OnAim;
        fire.action.performed += OnFire;
        reload.action.performed += OnReload;
    }

    void OnDisable()
    {
        aim.action.performed -= OnAim;
        fire.action.performed -= OnFire;
        reload.action.performed -= OnReload;

        aim.action.Disable();
        fire.action.Disable();
        reload.action.Disable();
    }

    void Update()
    {
        RotateTowardsMouse();
    }

    // ---------------- INPUT ----------------

    void OnAim(InputAction.CallbackContext ctx)
    {
        mouseScreenPos = ctx.ReadValue<Vector2>();
    }

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

        Fire();

        yield return new WaitForSeconds(fireRate);

        canShoot = true;
    }

    void Fire()
    {
        audioSource.PlayOneShot(fireSound);

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorld.z = 0f;

        Vector2 direction = firePoint.right;

        // IMPORTANT: prevent zero direction
        if (direction.sqrMagnitude < 0.01f)
            direction = firePoint.right;

        direction.Normalize();

        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();

        // stronger, more consistent launch
        Vector2 dir = direction.normalized;
        rb.linearVelocity = dir * muzzleVelocity;
    }

    // ---------------- RELOAD ----------------

    void StartReload()
    {
        if (isReloading || currentAmmo == magazineSize) return;

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;



        audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;

        isReloading = false;
    }
}