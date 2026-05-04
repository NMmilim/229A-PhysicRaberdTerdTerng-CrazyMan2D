using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 5f;
    [SerializeField] Vector3 offset = new Vector3(0, 4, -10);
    [SerializeField] float verticalOffset = 2f;
    [SerializeField] float lookAhead = 2f;
    [SerializeField] float minX, maxX, minY, maxY;
    [SerializeField] float maxLookDistance = 5f;

    Vector3 GetMouseWorld()
    {
        Vector2 mouse = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

        Vector3 world = Camera.main.ScreenToWorldPoint(
            new Vector3(mouse.x, mouse.y, -Camera.main.transform.position.z)
        );

        world.z = 0;
        return world;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 mouseWorld = GetMouseWorld();

        // Direction from player to mouse
        Vector3 dir = Vector3.ClampMagnitude(mouseWorld - target.position, maxLookDistance);

        // LookAhead movement
        Vector3 aimOffset = dir * lookAhead;

        Vector3 desiredPosition = target.position
                                + aimOffset
                                + offset
                                + new Vector3(0, verticalOffset, 0);

        // Clamp to MAP bounds (with camera size)
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX + camWidth, maxX - camWidth);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY + camHeight, maxY - camHeight);

        // Clamp camera so it DOESN’T go past cursor
        desiredPosition.x = Mathf.Clamp(
            desiredPosition.x,
            Mathf.Min(target.position.x, mouseWorld.x),
            Mathf.Max(target.position.x, mouseWorld.x)
        );

        desiredPosition.y = Mathf.Clamp(
            desiredPosition.y,
            Mathf.Min(target.position.y, mouseWorld.y),
            Mathf.Max(target.position.y, mouseWorld.y)
        );

        // Smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}