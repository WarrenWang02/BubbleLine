using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardTest : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            Debug.Log("W key pressed!");
        }
    }
}
