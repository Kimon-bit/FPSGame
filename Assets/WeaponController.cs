using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject Sword;
    public bool canAttack = true;
    public float attackCooldown = 0.6f;
    public bool isAttacking = false;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Normal.Enable();
        controls.Normal.MeleeAttack.performed += _ => MeleeAttack();
    }

    void OnDestroy()
    {
        controls.Normal.MeleeAttack.performed -= _ => MeleeAttack();
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
}