using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 5f;
    [SerializeField] Vector3 offset = new Vector3(0, 4, -10);
   // [SerializeField] float verticalOffset = 2f;
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

        Vector3 dir = mouseWorld - target.position;
        dir = Vector3.ClampMagnitude(dir, maxLookDistance);

        Vector3 desiredPosition = target.position + offset + dir * lookAhead;



        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}