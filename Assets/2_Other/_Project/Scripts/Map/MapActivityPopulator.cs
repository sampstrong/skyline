using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Unity.Core;
using PlanetX.Map;
using Sirenix.OdinInspector;
using UnityEngine;
using LocationService = PlanetX.Map.LocationService;

public class MapActivityPopulator : BaseMapPopulator
{
    public enum PopulationType
    {
        SingleObject = 0,
        MultipleObjects
    }

    public PopulationType populationType;

    [HideIf("populationType", PopulationType.MultipleObjects)] [SerializeField]
    private MapObject _singleObject;

    [HideIf("populationType", PopulationType.MultipleObjects)] [SerializeField]
    private int _numberOfObjects;

    [HideIf("populationType", PopulationType.SingleObject)] [SerializeField]
    private List<MapObject> _objects;

    [SerializeField] private Canvas _canvas;

    [SerializeField] private float _rangeMultiplier = 1;


    protected override void PopulateMap()
    {
        if (populationType == PopulationType.SingleObject)
        {
            for (int i = 0; i < _numberOfObjects; i++)
            {
                var randomCoords = GetRandomCoords();
                CreateObject(_singleObject, randomCoords);
            }
        }
        else
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                var randomCoords = GetRandomCoords();
                CreateObject(_objects[i], randomCoords);
            }
        }
    }

    private void CreateObject(MapObject mapObject, LatLng randomCoords)
    {
        var obj = Instantiate(mapObject, _canvas.transform);
        obj.Initialize(randomCoords);
    }
    
    protected virtual LatLng GetRandomCoords()
    {
        var randomLatitude = Random.Range((float)(LocationService.latitude - (2 * _rangeMultiplier / Map.MapRadius)),
            (float)(LocationService.latitude + (2 * _rangeMultiplier / Map.MapRadius)));
        var randomLongitude = Random.Range((float)(LocationService.longitude - (1 * _rangeMultiplier / Map.MapRadius)),
            (float)(LocationService.longitude + (1 * _rangeMultiplier / Map.MapRadius)));

        var randomCoords = new LatLng(randomLatitude, randomLongitude);
        return randomCoords;
    }
}


