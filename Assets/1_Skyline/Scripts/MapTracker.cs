using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps;
using UnityEngine;

public class MapTracker : MonoBehaviour
{
    public LatLng Coordinates => _building.Coordinates;
    
    private ITimelineObject _building;
    
    public void Init(ITimelineObject building)
    {
        _building = building;
    }
}
