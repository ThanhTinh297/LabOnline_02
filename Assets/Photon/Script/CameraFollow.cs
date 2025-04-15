using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public void AssighCamera(Transform playerTransform)
    {
        virtualCamera.Follow = playerTransform.transform;
        virtualCamera.LookAt = playerTransform.transform;
    }
}
