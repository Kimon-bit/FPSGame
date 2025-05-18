using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 8f;
    public float walkSpeed = 4f;
    public float sprintSpeed = 12f;
    private bool jumpPressed;
    private bool isSprinting;
    private bool isWalking;
    public float groundFriction = 20f;
    public float jumpPower = 10f;
    public float jumpCooldown = 0.1f;
    public int jumpCount = 1;
    private int jumps = 0;
    public float airControlMultiplier = 2.5f;
    private bool canJump;

    [Header("Ground Detection Settings")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius = 0.1f;
    private bool isGrounded;

    [Header("Ground Pound Settings")]
    public float groundPoundForce = 50f;
    //public float groundPoundRadius = 5f;
    //public LayerMask enemyLayer;
    private bool isGroundPounding;

    [Header("Sliding Settings")]
    public float slideStopThreshold = 1f;
    public float slideDrag = 0.1f;
    private bool isSliding;
    private bool wantsToSlide;
    private float GroundPoundSpeed;

    [Header("Other Settings")]
    public Transform orientation;
    private Rigidbody rb;
    private Vector2 movementInput;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Normal.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Normal.Move.canceled += ctx => movementInput = Vector2.zero;
        controls.Normal.Sprint.performed += ctx => isSprinting = true;
        controls.Normal.Sprint.canceled += ctx => isSprinting = false;
        controls.Normal.Walking.performed += ctx => isWalking = true;
        controls.Normal.Walking.canceled += ctx => isWalking = false;
        controls.Normal.Jump.performed += ctx => OnJumpPressed();
        controls.Normal.SlidePound.performed += ctx => OnGroundPoundPressed();
        controls.Normal.SlidePound.canceled += ctx => OnGroundPoundReleased();
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
        if (isSliding && rb.velocity.magnitude < slideStopThreshold)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (!isSliding)
        {
            MovePlayer();
        }

        if (isGroundPounding && isGrounded)
        {
            isGroundPounding = false;

            if (wantsToSlide)
            {
                float impactVelocity = GroundPoundSpeed;
                float slideSpeedFromImpact = Mathf.Max(impactVelocity, sprintSpeed);

                StartSlide(slideSpeedFromImpact);
            }
        }
        if (isSliding)
        {
            Vector3 horizontal = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 friction = -horizontal.normalized * slideDrag * 10f;
            rb.AddForce(friction, ForceMode.Force);
        }
    }

    private void MovePlayer()
    {
        float currentSpeed = isSprinting ? sprintSpeed : isWalking ? walkSpeed : movementSpeed;
        Vector3 moveDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x;

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airControlMultiplier, ForceMode.Force);
        }
    }

    private void RestrictSpeed()
    {
        if (isSliding) return;

        float currentSpeed = isSprinting ? sprintSpeed : isWalking ? walkSpeed : movementSpeed;
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        if (horizontalVelocity.magnitude > currentSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void OnJumpPressed()
    {
        jumpPressed = true;
        if (isGrounded)
        {
            jumps = 0; // Reset jumps on ground
        }
    }

    private void HandleJump()
    {
        if (jumpPressed && canJump && jumps < jumpCount)
        {
            canJump = false;
            Jump();
            jumps++; // Increase jump count
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        jumpPressed = false;
    }

    private void CheckGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);

        if (isGroundPounding && !wasGrounded && isGrounded)
        {
            GroundPoundSpeed = Mathf.Abs(rb.velocity.y);
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

    private void StartSlide(float initialSpeed)
    {
        isSliding = true;

        Vector3 slideDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z).normalized;
        if (slideDirection == Vector3.zero)
        {
            slideDirection = orientation.forward;
        }
        rb.velocity = slideDirection * initialSpeed;
        rb.drag = slideDrag;
    }

    private void StopSlide()
    {
        isSliding = false;
        rb.drag = 0;
    }

    private void OnGroundPoundPressed()
    {
        if (isGrounded)
        {
            Vector3 forwardDirection = orientation.forward;
            float initialSpeed = Mathf.Max(GetHorizontalSpeed(), sprintSpeed);
            rb.velocity = new Vector3(forwardDirection.x, 0f, forwardDirection.z).normalized * initialSpeed;
            StartSlide(initialSpeed);
        }
        else
        {
            wantsToSlide = true;
            isGroundPounding = true;
            rb.velocity = new Vector3(rb.velocity.x, -groundPoundForce, rb.velocity.z);
        }
    }

    private void OnGroundPoundReleased()
    {
        wantsToSlide = false;

        if (isSliding)
        {
            StopSlide();
        }
    }

    public float GetSpeed()
    {
        return rb.velocity.magnitude;
    }
    
    private float GetHorizontalSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        return horizontalVelocity.magnitude;
    }
}