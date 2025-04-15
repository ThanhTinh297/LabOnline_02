using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionSetup : MonoBehaviour
{
    private Transform PlayerTransform;
    
    private void Start()
    {
        PlayerTransform = transform.parent;
    }

    private void LateUpdate()
    {
        if (PlayerTransform != null)
        {
            transform.position = PlayerTransform.position;
            transform.rotation = Quaternion.identity;
        }
    }
}
