using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Unity.Core;
using UnityEngine;

namespace PlanetX.Map
{
    public abstract class MapObject : MonoBehaviour
    {
        public LatLng Coordinates
        {
            get => _coordinates;
            set => _coordinates = value;
        }
        
        protected LatLng _coordinates;
        
        protected virtual void Update()
        {
            UpdateMapObjectPosition();
        }
        
        /// <summary>
        /// Use to set initial state of object
        /// </summary>
        /// <param name="coordinates"></param>
        public abstract void Initialize(LatLng coordinates, ModelAsset model = null);
        

        /// <summary>
        /// Updates position on screen based on coordinates
        /// Method will vary depending on if the object is in
        /// screen space or world space
        /// </summary>
        protected abstract void UpdateMapObjectPosition();

        
        /// <summary>
        /// Toggles the visibility for the object to allow for different objects at
        /// different levels of detail.
        /// </summary>
        public abstract void ToggleVisibility(bool isVisible);

        

    }
}
