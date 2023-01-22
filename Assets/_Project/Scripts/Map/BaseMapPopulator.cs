using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Unity.Core;
using UnityEngine;
using UnityEngine.Assertions;
using LocationService = PlanetX.Map.LocationService;

public abstract class BaseMapPopulator : MonoBehaviour
{
    protected LightshipMap Map
    {
        get => _map;
    }

    protected LocationService LocationService
    {
        get => _locationService;
    }
    
    private LocationService _locationService;
    private LightshipMap _map;
    
    protected virtual void Start()
    {
        _map = FindObjectOfType<LightshipMap>();
        Assert.IsNotNull(_map, "No Lightship Map found in scene");
        
        _locationService = FindObjectOfType<LocationService>();
        Assert.IsNotNull(_locationService, "No Location Service found in scene");
        
        _locationService.onLocationServiceInitialized.AddListener(Init);
        Init();
    }

    protected virtual void Init()
    {
        PopulateMap();
    }
    
    protected virtual LatLng GetRandomCoords()
    {
        var randomLatitude = Random.Range((float)(_locationService.latitude - (2 / Map.MapRadius)),
            (float)(_locationService.latitude + (2 / Map.MapRadius)));
        var randomLongitude = Random.Range((float)(_locationService.longitude - (1 / Map.MapRadius)),
            (float)(_locationService.longitude + (1 / Map.MapRadius)));

        var randomCoords = new LatLng(randomLatitude, randomLongitude);
        return randomCoords;
    }

    /// <summary>
    /// Called after location service is initialized
    /// </summary>
    protected abstract void PopulateMap();
}
