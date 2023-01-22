using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using LocationService = PlanetX.Map.LocationService;

public class MyLocationMapObject : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    
    private Camera _mapCamera;
    private RectTransform _renderTexture;
    private MyLocationTracker _tracker;
    private bool _initialized;
    
    public void Init(MyLocationTracker tracker)
    {
        _tracker = tracker;
        _mapCamera = GameObject.FindWithTag("MapCamera").GetComponent<Camera>();
        _renderTexture = FindObjectOfType<RenderTextureMap>().GetComponent<RectTransform>();
        _initialized = true;
        Debug.Log("MyLocationObject Initialized");
    }

    // Update is called once per frame
    void Update()
    {
        //if (!_initialized) return;
        //if (!_locationService.Initialized) return;
        FollowMapTracker();
        Rotate();
        HideOffMap();
    }

    private void FollowMapTracker()
    {
        var trackingPos = _tracker.transform.position;
        Vector3 screenPos = _mapCamera.WorldToViewportPoint(trackingPos);
        screenPos.Scale(new Vector3(_renderTexture.rect.width, _renderTexture.rect.height, 1f));
        transform.position = screenPos;
    }

    private void Rotate()
    {
        transform.rotation = GetScreenSpaceHeading(LocationService.Instance.heading);
    }
    
    private Quaternion GetScreenSpaceHeading(float heading)
    {
        var screenHeading = Quaternion.Euler(0, 0, -heading);
        return screenHeading;
    }
    
    private void HideOffMap()
    {
        if (_rectTransform.position.y <= 805) _canvasGroup.alpha = 1;
        else _canvasGroup.alpha = 0;
    }
}
