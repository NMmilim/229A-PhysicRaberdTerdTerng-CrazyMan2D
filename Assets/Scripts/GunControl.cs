using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class GunAim : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform playerSprite;
    [SerializeField] Transform gunVisual;

    [SerializeField] float minAngle = -60f;
    [SerializeField] float maxAngle = 60f;
    bool isAiming;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 dir = mouseWorld - transform.position;
        if (dir.magnitude < 0.1f) return;

        float rawAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;


        // Determine facing
        bool facingLeft = mouseWorld.x < player.position.x;

        playerSprite.localScale = facingLeft
            ? new Vector3(-1, 1, 1)
            : new Vector3(1, 1, 1);

        // Convert to LOCAL angle (relative to facing direction)
        float localAngle = facingLeft
            ? Mathf.DeltaAngle(180f, rawAngle)
            : Mathf.DeltaAngle(0f, rawAngle);

        // Clamp in local space
        float clamped = Mathf.Clamp(localAngle, minAngle, maxAngle);

        // Convert BACK to world rotation
        float finalAngle = facingLeft
            ? clamped + 180f
            : clamped;

        float rotateSpeed = 250f; // tweak this

        float current = transform.eulerAngles.z;
        float smooth = Mathf.MoveTowardsAngle(current, finalAngle, rotateSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, smooth);

        // Flip gun visual only
        gunVisual.localScale = facingLeft
            ? new Vector3(1, -1, 1)
            : new Vector3(1, 1, 1);
    }
}