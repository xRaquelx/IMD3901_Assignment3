using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls_Desktop : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSens = 20f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    public CharacterController characterController;
    public Transform playerCamTransform;

    private float yVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        moveInput = Keyboard.current != null ? new Vector2
            (
                (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0),
                (Keyboard.current.sKey.isPressed ? -1 : 0) + (Keyboard.current.wKey.isPressed ? 1 : 0)
            ) : Vector2.zero;

        if (characterController.isGrounded && yVelocity < 0)
            yVelocity = -0.1f;
        yVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y + Vector3.up * yVelocity;
        characterController.Move(move * speed * Time.deltaTime);

        lookInput = Mouse.current.delta.ReadValue();

        float mouseX = lookInput.x * mouseSens * Time.deltaTime;
        float mouseY = lookInput.y * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        playerCamTransform.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
