using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Samples.OrthographicCamera;
using Niantic.Lightship.Maps.Unity.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PlanetX.Map
{
    public class Map2DObject : MapObject, IScalableMapObject
    {
        
        public float MinScaleFactor { get => _minScaleFactor; set => _minScaleFactor = value; }
        public float MaxScaleFactor { get => _maxScaleFactor; set => _maxScaleFactor = value; }

        public Image image;
        public Canvas canvas;
        
        public enum LevelOfDetail
        {
            High = 0,
            Mid = 1,
            Low = 2
        }

        public LevelOfDetail levelOfDetail;

        [SerializeField] private List<GameObject> _unseenBubbleObjects;

        public enum RenderType
        {
            Normal = 0,
            PostProcess = 1
        }

        public RenderType renderType;

        private ModelAsset _model;
        
        private float _minScaleFactor = 0.5f;
        private float _maxScaleFactor = 1.5f;
        
        [HideIf("renderType", RenderType.Normal)]
        [SerializeField] private GameObject _waveImage;
        

        protected LightshipMap _map;
        protected Camera _camera;

        public override void Initialize(LatLng coordinates, ModelAsset model = null)
        {
            _coordinates = coordinates;
            _map = FindObjectOfType<LightshipMap>();
            _camera = FindObjectOfType<Camera>();

            if (renderType == RenderType.PostProcess)
            {
                canvas.worldCamera = _camera;
            }

            if (!model) return;
            _model = model;

            
            // need to refactor the below code into a state machine to clean up
            
            if (_model.modelOwnershipType == ModelAsset.ModelOwnershipType.MyModel)
            {
                if (levelOfDetail == LevelOfDetail.Mid) image.sprite = MapObjectCreator.commonObjects.MyMidLODSprite;
                if (levelOfDetail == LevelOfDetail.Low) image.sprite = MapObjectCreator.commonObjects.MyLowLODSprite;
                
                if (renderType == RenderType.PostProcess)
                {
                    image.material = MapObjectCreator.commonObjects.MyGlowMaterial;
                    if (levelOfDetail == LevelOfDetail.High)
                    {
                        _waveImage.SetActive(true);
                    }
                }
            }
            
            if (_model.modelOwnershipType == ModelAsset.ModelOwnershipType.OtherModel)
            {
                _minScaleFactor *= 0.5f;
                _maxScaleFactor *= 0.5f;

                if (levelOfDetail == LevelOfDetail.Mid) image.sprite = MapObjectCreator.commonObjects.OtherMidLODSprite;
                if (levelOfDetail == LevelOfDetail.Low) image.sprite = MapObjectCreator.commonObjects.OtherLowLODSprite;

                if (renderType == RenderType.PostProcess)
                {
                    image.material = MapObjectCreator.commonObjects.OtherGlowMaterial;
                    if (levelOfDetail is LevelOfDetail.High)
                    {
                        _waveImage.SetActive(false);
                    }
                }

                if (levelOfDetail == LevelOfDetail.High &&
                    _model.modelInteractionType == ModelAsset.ModelInteractionType.Unseen)
                {
                    foreach (var obj in _unseenBubbleObjects)
                    {
                        obj.SetActive(true);
                    }

                    image.color = Color.black;
                }
            }
            
            if (levelOfDetail is LevelOfDetail.Mid or LevelOfDetail.Low) return;
            image.sprite = model.sprite;
            
            // instead of individually holding coordinates, could pass the MapObjectGroup and reference
            // its coordinates instead
            // set a MapObjectGroup parameter as null by default, then check if null before assigning
        }

        protected override void Update()
        {
            base.Update();
            SetScale();
        }

        protected override void UpdateMapObjectPosition()
        {
            if (renderType == RenderType.Normal)
            {
                var worldPosition = _map.LatLngToScene(_coordinates);
                var screenPosition = _camera.WorldToScreenPoint(worldPosition);
                screenPosition.z = 0;
                transform.position = screenPosition;
            }
            else
            {
                transform.position = _map.LatLngToScene(_coordinates);
                transform.position += new Vector3(0, _camera.transform.position.y - 1, 0);
            }
        }
        

        public void SetScale()
        {
            if (levelOfDetail is LevelOfDetail.Mid or LevelOfDetail.Low) return;
            if (!_model) return;
            
            if (_model.modelOwnershipType == ModelAsset.ModelOwnershipType.MyModel)
            {
                transform.localScale = Vector3.one * 500 / (float)_map.MapRadius;    
            }
            else if (_model.modelOwnershipType == ModelAsset.ModelOwnershipType.OtherModel)
            {
                transform.localScale = Vector3.one * 250 / (float)_map.MapRadius; 
            }
            
            
            if (transform.localScale.y > _maxScaleFactor)
            {
                transform.localScale = Vector3.one * _maxScaleFactor;
            }
            else if (transform.localScale.y < _minScaleFactor)
            {
                transform.localScale = Vector3.one * _minScaleFactor;
            }
        }

        public override void ToggleVisibility(bool isVisible)
        {
            if (isVisible)
            {
                gameObject.SetActive(true);
                // play pop-in animation?
                // can use coroutine + Random.Value
            }
            else gameObject.SetActive(false);
        }
    }
}
