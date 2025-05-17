using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController weaponController;

    private void OnTriggerEnter(Collider enemy)
    {
        if (enemy.tag == "Enemy" && weaponController.isAttacking)
        {
            Debug.Log(enemy.name);
        }
    }
}
