using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController weaponController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && weaponController != null && weaponController.isAttacking)
        {
            Debug.Log("[Sword] Hit enemy: " + other.name);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("[Knife] Hit enemy: " + collision.collider.name);
        }
    }
}

