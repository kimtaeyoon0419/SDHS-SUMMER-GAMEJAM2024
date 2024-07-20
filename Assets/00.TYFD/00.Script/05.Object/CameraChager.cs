// # System
using Cinemachine;
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class CameraChager : MonoBehaviour
{
    private void Start()
    {
        transform.position = Vector3.zero;
        if(CameraManager.instance.virtualCamera != null)
        {
            CameraManager.instance.virtualCamera.gameObject.SetActive(false);
        }
        CameraManager.instance.virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
}
