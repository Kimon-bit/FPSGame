using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackRate = 1f;
    public int damage = 25;
    public Transform attackPoint;

    private float nextAttackTime = 0f;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Normal.Enable();
        controls.Normal.MeleeAttack.performed += OnAttack;
    }

    private void OnDisable()
    {
        controls.Normal.MeleeAttack.performed -= OnAttack;
        controls.Normal.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + 1f / attackRate;
        PerformAttack();
    }

    private void PerformAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(attackPoint.position, attackPoint.forward, out hit, attackRange))
        {
            Debug.Log("Hit " + hit.collider.name);
            // Add damage handling here, e.g.:
            // hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(damage);
        }
    }
}
