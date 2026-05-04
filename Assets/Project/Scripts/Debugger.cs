using UnityEngine;
using UnityEngine.InputSystem;

public class InputDebug : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.aKey.isPressed)
            Debug.Log("A key works");

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Debug.Log("Space works");
    }
}