using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTracker : MonoBehaviour
{
    private Camera _mapCamera;
    private RectTransform _renderTexture;

    private FutureBuildingMapObject _mapObject;
    private bool _initialized;

    private void Start()
    {
        _mapCamera = GameObject.FindWithTag("MapCamera").GetComponent<Camera>();
        _renderTexture = FindObjectOfType<RenderTextureMap>().GetComponent<RectTransform>();
    }
    
    public void Init(FutureBuildingMapObject mapObject)
    {
        _mapObject = mapObject;
        _initialized = true;
    }
    
    void Update()
    {
        if (!_initialized) return;
        var trackingPos = _mapObject.transform.position;
        Vector3 screenPos = _mapCamera.WorldToViewportPoint(trackingPos);
        screenPos.Scale(new Vector3(_renderTexture.rect.width, _renderTexture.rect.height, 1f));
        transform.position = screenPos;
    }
}
