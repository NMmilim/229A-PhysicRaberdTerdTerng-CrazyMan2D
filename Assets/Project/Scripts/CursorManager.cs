using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "MainGame"; // your gameplay scene name
    [SerializeField] private bool startInGame = true;

    private bool inGame;

    void Start()
    {
        // Check current scene
        bool isGameScene = SceneManager.GetActiveScene().name == gameSceneName;

        if (!isGameScene)
        {
            SetDefaultCursor();
            enabled = false; //  disable script completely
            return;
        }

        inGame = startInGame;
        ApplyCursorState();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            inGame = false;
            ApplyCursorState();
        }

        if (!inGame && Mouse.current.leftButton.wasPressedThisFrame)
        {
            inGame = true;
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

    void SetDefaultCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void OnDisable()
    {
        SetDefaultCursor();
    }

    void OnDestroy()
    {
        SetDefaultCursor();
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SetDefaultCursor();
        }
    }
}