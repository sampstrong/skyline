using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetX.Map
{
    public interface IScalableMapObject
    {
        public float MinScaleFactor { get; set; }
        public float MaxScaleFactor { get; set; }
        
        
        /// <summary>
        /// Control the size of objects relative to object type and map radius.
        /// </summary>
        public void SetScale();
        
    }
}

