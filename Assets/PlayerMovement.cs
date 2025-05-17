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
    public float playerSize = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius;
    private bool isGrounded;

    [Header("Ground Pound Settings")]
    public float groundPoundForce = 50f;
    public float groundPoundRadius = 5f;
    public LayerMask enemyLayer;
    private bool isGroundPounding;

    private Rigidbody rb;
    public Transform orientation;

    private Vector2 movementInput;
    private bool jumpPressed;
    private bool isSprinting;
    private bool isWalking;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Normal.Jump.performed += ctx => OnJumpPressed();
        controls.Normal.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Normal.Move.canceled += ctx => movementInput = Vector2.zero;
        controls.Normal.Sprint.performed += ctx => isSprinting = true;
        controls.Normal.Sprint.canceled += ctx => isSprinting = false;
        controls.Normal.Walking.performed += ctx => isWalking = true;
        controls.Normal.Walking.canceled += ctx => isWalking = false;
        controls.Normal.GroundPound.performed += ctx => TryGroundPound();
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

        if (isGroundPounding && isGrounded)
        {
            //GroundPoundImpact();
            isGroundPounding = false;
        }
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
        float currentSpeed = isSprinting ? sprintSpeed : isWalking ? walkSpeed : movementSpeed;
        Vector3 moveDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x;

        if (isGrounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airControlMultiplier, ForceMode.Force);
    }

    private void RestrictSpeed()
    {
        float currentSpeed = isSprinting ? sprintSpeed : isWalking ? walkSpeed : movementSpeed;
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
    }

    private void TryGroundPound()
    {
        if (!isGrounded && !isGroundPounding)
        {
            isGroundPounding = true;
            rb.velocity = new Vector3(rb.velocity.x, -groundPoundForce, rb.velocity.z);
        }
    }

    //private void DoGroundPoundImpact()
    //{
    //    Collider[] hitColliders = 
    //    foreach (var hit in hitColliders)
    //    {
    //        Rigidbody enemyRb = 
    //    }
    //}

    public float GetSpeed()
    {
        return rb.velocity.magnitude;
    }
}