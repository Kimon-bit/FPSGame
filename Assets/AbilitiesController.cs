using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;

    private Rigidbody rb;
    private PlayerMovement playerMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float originalSpeed = playerMovement.moveSpeed;
        playerMovement.moveSpeed = dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        playerMovement.moveSpeed = originalSpeed;
        isDashing = false;
    }
}
