using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCameraController : MonoBehaviour
{
    private void Start()
    {
        CameraController.Instance.AddCameraToList(GetComponent<CinemachineVirtualCamera>());
    }

    private void OnEnable()
    {
        CameraController.Instance.AddCameraToList(GetComponent<CinemachineVirtualCamera>());
    }

    private void OnDisable()
    {
        CameraController.Instance.RemoveCameraFromList(GetComponent<CinemachineVirtualCamera>());
    }
}
