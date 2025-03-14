using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed;
    public float groundFriction;
    public float jumpPower;
    public float jumpCooldown;
    public float airControlMultiplier;
    private bool canJump;

    public float walkSpeed; // Slow walk
    public float sprintSpeed; // Sprint

    [Header("Ground Detection")]
    public float playerSize;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Controls")]
    public KeyCode jumpKey = KeyCode.Space;

    private Rigidbody rb;
    public Transform orientation;
    private float inputX;
    private float inputY;
    private Vector3 movementVector;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        canJump = true;
    }

    private void Update()
    {
        CheckGrounded();
        HandleInput();
        RestrictSpeed();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && canJump && isGrounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        movementVector = orientation.forward * inputY + orientation.right * inputX;

        // on ground
        if (isGrounded)
            rb.AddForce(movementVector.normalized * movementSpeed * 10f, ForceMode.Force);

        // in air
        else if (!isGrounded)
            rb.AddForce(movementVector.normalized * movementSpeed * 10f * airControlMultiplier, ForceMode.Force);
    }

    private void RestrictSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (horizontalVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * movementSpeed;
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
