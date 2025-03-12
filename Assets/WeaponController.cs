using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Camera fpsCamera;
    public float range = 50f;
    public float damage = 10f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Left mouse click
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);
            // Apply damage to enemies (later)
        }
    }
}
