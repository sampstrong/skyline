using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Unity.Core;
using PlanetX.Map;
using UnityEngine;

public class Map2DMarker : MapObject
{
    public enum RenderSpace
    {
        ScreenSpace = 0,
        WorldSpace = 1
    }

    public RenderSpace renderSpace;
    
    private LightshipMap _map;
    private Camera _camera;
    
    public override void Initialize(LatLng coordinates, ModelAsset model = null)
    {
        SetCoordinates(coordinates);
        _map = FindObjectOfType<LightshipMap>();
        _camera = FindObjectOfType<Camera>();
    }

    public void SetCoordinates(LatLng coordinates)
    {
        _coordinates = coordinates;
    }

    protected override void UpdateMapObjectPosition()
    {
        if (renderSpace == RenderSpace.ScreenSpace)
        {
            var worldPosition = _map.LatLngToScene(_coordinates);
            var screenPosition = _camera.WorldToScreenPoint(worldPosition);
            screenPosition.z = 0;
            transform.position = screenPosition;    
        }
        else
        {
            transform.position = _map.LatLngToScene(_coordinates);
            transform.position += new Vector3(0, _camera.transform.position.y - 1, 0);
        }
    }

    public override void ToggleVisibility(bool isVisible)
    {
        if (isVisible)
        {
            gameObject.SetActive(true);
        }
        else gameObject.SetActive(false);
    }
}
