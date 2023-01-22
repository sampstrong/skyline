using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Unity.Core;
using PlanetX.Map;
using UnityEngine;

namespace PlanetX.Map
{
    public class Map3DObject : MapObject
    {
        [SerializeField] private float _verticalOffset = 100;
        
        private LightshipMap _map;
        private ModelAsset _model;
        private Renderer[] _renderers;

        public override void Initialize(LatLng coordinates, ModelAsset model = null)
        {
            // set appropriate scale
            
            _map = FindObjectOfType<LightshipMap>();
            _coordinates = coordinates;

            if (!model) return;
            _model = model;

            if (_model.modelOwnershipType == ModelAsset.ModelOwnershipType.OtherModel)
            {
                if (_model.modelInteractionType == ModelAsset.ModelInteractionType.Unseen)
                {
                    _renderers = GetComponentsInChildren<Renderer>();
                    foreach (var rend in _renderers)
                    {
                        rend.enabled = false;
                    }
                }
                if (_model.modelInteractionType == ModelAsset.ModelInteractionType.Seen)
                {
                    transform.localScale *= 0.5f;
                }
                else if (_model.modelInteractionType == ModelAsset.ModelInteractionType.Interacted)
                {
                    transform.localScale *= 0.75f;
                }
            }

        }
        
        protected override void Update()
        {
            UpdateMapObjectPosition();
        }
        
        protected override void UpdateMapObjectPosition()
        {
            var worldPosition = _map.LatLngToScene(_coordinates);
            transform.position = worldPosition + new Vector3(0, _verticalOffset, 0);
            
        }

        public override void ToggleVisibility(bool isVisible)
        {
            throw new NotImplementedException();
        }
    }
}
