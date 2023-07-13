using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps;
using PlanetX.Map;
using UnityEngine;

public class MapModelPopulator : BaseMapPopulator
{
    [SerializeField] private List<ModelAsset> _models;
    [SerializeField] private MapObjectCreator _mapObjectCreator;
    protected override void PopulateMap()
    {
        for (int i = 0; i < _models.Count; i++)
        {
            _mapObjectCreator.CreateNewMapObjectGroup( GetRandomCoords(), _models[i]);
        }
    }
}
