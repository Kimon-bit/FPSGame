using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform cameraPosition;
    
    void Update()
    {
        // A slight workaround for dealing with rigidbody cameras
        transform.position = cameraPosition.position;
    }
}
