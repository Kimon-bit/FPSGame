using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject Sword;
    public bool canAttack = true;
    public float attackCooldown = 0.6f;
    public bool isAttacking = false;

    public GameObject ThrowingKnife;
    public Transform KnifeSpawnPoint;
    public float KnifeThrowForce = 20f;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Normal.Enable();
        controls.Normal.MeleeAttack.performed += _ => MeleeAttack();
        controls.Normal.ThrowKnife.performed += _ => ThrowKnife();
    }

    void OnDestroy()
    {
        controls.Normal.MeleeAttack.performed -= _ => MeleeAttack();
        controls.Normal.ThrowKnife.performed -= _ => ThrowKnife();
        controls.Normal.Disable();
    }

    public void MeleeAttack()
    {
        if (!canAttack) return;

        isAttacking = true;
        Animator animate = Sword.GetComponent<Animator>();
        animate.SetTrigger("Attack");
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    // Possibly switch Invoke to IEnumerator
    void ResetAttack()
    {
        canAttack = true;
        isAttacking = false;
    }

    public void ThrowKnife()
    {
        GameObject knife = Instantiate(ThrowingKnife, KnifeSpawnPoint.position, KnifeSpawnPoint.rotation);
        Rigidbody rb = knife.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = KnifeSpawnPoint.forward * KnifeThrowForce;
        }
    }
}