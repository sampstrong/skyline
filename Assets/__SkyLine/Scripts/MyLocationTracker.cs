using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Niantic.Lightship.Maps;
using UnityEngine;
using UnityEngine.PlayerLoop;
using LocationService = PlanetX.Map.LocationService;

public class MyLocationTracker : MonoBehaviour
{
    public LatLng Coordinates;

    private LatLng _coordinates;
    private AbstractMap _map;
    private bool _initialized;

    public void Init(AbstractMap map)
    {
        _map = map;
        _initialized = true;
        Debug.Log("MyLocationTracker Initialized");
    }

    void Update()
    {
        //if (!_initialized) return;
        //if (!_locationService.Initialized) return;
        LocationService.Instance.coordinates = _coordinates;
        
        Debug.Log($"Location: {LocationService.Instance.coordinates}");
        
        var mapCoords = new Vector2d(_coordinates.Latitude, _coordinates.Longitude);
        transform.localPosition = _map.GeoToWorldPosition(mapCoords, true);
    }
}
