using Niantic.Lightship.Maps.Unity.Core;
using UnityEngine;
using LocationService = PlanetX.Map.LocationService;

public class MapInitializer : MonoBehaviour
{
    [SerializeField] private LightshipMap _map;
    
    void Start()
    {
        SetMapToCoordinates();
    }

    private void SetMapToCoordinates()
    {
        var oldCenter = _map.LatLngToScene(_map.MapCenter);
        var newCenter = _map.LatLngToScene(LocationService.Instance.coordinates);
        var offset = oldCenter - newCenter;
        _map.OffsetMapCenter(offset);
        _map.transform.position += offset;
            
        Debug.Log($"Offset: {offset}");
        Debug.Log($"Map set to coordinates: {LocationService.Instance.coordinates.Latitude}, {LocationService.Instance.coordinates.Longitude}");
    }
}
