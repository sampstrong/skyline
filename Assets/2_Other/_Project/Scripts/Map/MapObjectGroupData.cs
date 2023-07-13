using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Niantic.Lightship.Maps;
using UnityEngine;

public class MapObjectGroupData
{
    public LatLng Coordinates;
    public string Username;
    
    
    public MapObjectGroupData(LatLng coordinates, string username)
    {
        Coordinates = coordinates;
        Username = username;
    }
}
