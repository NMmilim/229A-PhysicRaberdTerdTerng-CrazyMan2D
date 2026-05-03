using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] Transform player;

    [Header("Settings")]
    [SerializeField] float smoothSpeed = 20f;
    [SerializeField] float minDistance = 50f; // pixels
    [SerializeField] float maxDistance = 300f;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Convert mouse to UI local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.worldCamera,
            out Vector2 mouseLocal
        );

        // Convert player world position -> screen -> UI local
        Vector2 playerScreen = cam.WorldToScreenPoint(player.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            playerScreen,
            canvas.worldCamera,
            out Vector2 playerLocal
        );

        // Direction from player -> mouse
        Vector2 dir = mouseLocal - playerLocal;

        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right;

        Vector2 dirNormalized = dir.normalized;

        float distance = Mathf.Clamp(dir.magnitude, minDistance, maxDistance);

        Vector2 targetPos = playerLocal + dirNormalized * distance;

        // Smooth movement
        rectTransform.localPosition = Vector2.Lerp(
            rectTransform.localPosition,
            targetPos,
            smoothSpeed * Time.deltaTime
        );
    }
}