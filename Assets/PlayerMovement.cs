using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed;
    public float groundFriction;
    public float jumpPower;
    public float jumpCooldown;
    public float airControlMultiplier;
    private bool canJump;

    public float walkSpeed;
    public float sprintSpeed;

    [Header("Ground Detection")]
    public float playerSize;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody rb;
    public Transform orientation;

    private Vector2 movementInput;
    private bool jumpPressed;
    private bool isSprinting;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Normal.Jump.performed += ctx => OnJumpPressed();
        controls.Normal.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Normal.Move.canceled += ctx => movementInput = Vector2.zero;
        controls.Player.Sprint.performed += ctx => isSprinting = true;
        controls.Player.Sprint.canceled += ctx => isSprinting = false;
    }

    private void OnEnable()
    {
        controls.Normal.Enable();
    }

    private void OnDisable()
    {
        controls.Normal.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        canJump = true;
    }

    private void Update()
    {
        CheckGrounded();
        HandleJump();
        RestrictSpeed();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnJumpPressed()
    {
        jumpPressed = true;
    }

    private void HandleJump()
    {
        if (jumpPressed && canJump && isGrounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        jumpPressed = false;
    }

    private void MovePlayer()
    {
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 moveDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x;

        if (isGrounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airControlMultiplier, ForceMode.Force);
    }

    private void RestrictSpeed()
    {
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (horizontalVelocity.magnitude > currentSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerSize * 0.5f + 0.1f, groundLayer);
    }

    public float GetSpeed()
    {
        return rb.velocity.magnitude;
    }
}