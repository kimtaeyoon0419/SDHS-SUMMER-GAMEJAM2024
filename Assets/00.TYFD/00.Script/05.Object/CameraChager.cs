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
        if(CameraManager.instance.virtualCamera != null)
        {
            CameraManager.instance.virtualCamera.gameObject.SetActive(false);
        }
        CameraManager.instance.virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
}
