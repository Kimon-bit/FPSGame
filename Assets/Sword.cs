// not used anymore, moved to weapon controller
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttack : MonoBehaviour
{
    public float attackCooldown = 0.6f;
    private bool canAttack = true;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Normal.Enable();
        controls.Normal.MeleeAttack.performed += _ => Attack();
    }

    void OnDestroy()
    {
        controls.Normal.MeleeAttack.performed -= _ => Attack();
        controls.Normal.Disable();
    }

    void Attack()
    {
        
    }

    void ResetAttack()
    {
        canAttack = true;
    }
}
