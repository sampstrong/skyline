using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Unity.Core;
using UnityEngine;

namespace PlanetX.Map
{
    public class MapPostProcessObject : Map2DObject
    {
        protected override void UpdateMapObjectPosition()
        {
            transform.position = _map.LatLngToScene(_coordinates);
            transform.position += new Vector3(0, _camera.transform.position.y - 1, 0);
        }
    }
}
