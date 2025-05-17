using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public float sensitivityX = 100f;
    public float sensitivityY = 100f;

    public Transform Player;
    public Transform playerCamera;

    float xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouseInput = Mouse.current.delta.ReadValue();
        float mouseX = mouseInput.x * sensitivityX * Time.deltaTime;
        float mouseY = mouseInput.y * sensitivityY * Time.deltaTime;

        // Vertical look
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal look
        Player.Rotate(Vector3.up * mouseX);
    }
}
