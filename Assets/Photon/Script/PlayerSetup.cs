using Fusion;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private GameObject cameraPosition;
    public void SetUpCamera()
    {
        if (Object.HasInputAuthority)
        {
            CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
            if(cameraFollow != null)
            {
                cameraFollow.AssighCamera(cameraPosition.transform);
            }
        }
    }
}
