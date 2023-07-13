using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CameraFeedToggle : MonoBehaviour
{
    [SerializeField] private ARCameraBackground _cameraFeed;
    [SerializeField] private GameObject _ui;

    private void Update()
    {
       if (Input.deviceOrientation is DeviceOrientation.LandscapeLeft or DeviceOrientation.LandscapeRight)
       {
           ToggleCameraFeed(false);
       }
       else
       {
           ToggleCameraFeed(true);
       }
    }

    private void ToggleCameraFeed(bool isVisible)
    {
        _cameraFeed.enabled = isVisible;
        _ui.SetActive(isVisible);
    }
}
