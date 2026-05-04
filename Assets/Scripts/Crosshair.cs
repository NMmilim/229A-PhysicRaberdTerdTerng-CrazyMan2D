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

        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        Vector2 playerScreen = cam.WorldToScreenPoint(player.position);

        Vector2 dir = mouseScreen - playerScreen;

        if (dir.sqrMagnitude < 0.001f)
            dir = Vector2.right;

        Vector2 dirNormalized = dir.normalized;

        float distance = Mathf.Clamp(dir.magnitude, minDistance, maxDistance);

        Vector2 targetScreenPos = playerScreen + dirNormalized * distance;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            targetScreenPos,
            canvas.worldCamera,
            out Vector2 targetLocal
        );

        rectTransform.localPosition = Vector2.Lerp(
            rectTransform.localPosition,
            targetLocal,
            smoothSpeed * Time.deltaTime
        );
    }
}