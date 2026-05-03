using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [SerializeField] bool startInGame = true;

    bool inGame;

    void Start()
    {
        inGame = startInGame;
        ApplyCursorState();
    }

    void Update()
    {
        // Toggle with ESC (example)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            inGame = !inGame;
            ApplyCursorState();
        }
    }

    void ApplyCursorState()
    {
        if (inGame)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Optional: auto show when alt-tab
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}