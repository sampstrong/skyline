using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using _Project.Scripts.Models;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Samples.OrthographicCamera;
using Niantic.Lightship.Maps.Unity.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PlanetX.Map
{
    public class MapObjectManager : Singleton<MapObjectManager>
    {
        public UnityEvent<LevelOfDetail> onLODChanged;
        public UnityEvent<Vector3> onPositionChosen;

        public LevelOfDetail CurrentLevelOfDetail { get => _levelOfDetail; set => _levelOfDetail = value; }
        public MapInteractionState CurrentMapInteractionState { get => _mapInteractionState; set => _mapInteractionState = value; }
    
        public enum LevelOfDetail
        {
            High = 0,
            Mid = 1,
            Low = 2
        }

        public enum MapInteractionState
        {
            Viewing = 0,
            Placing = 1
        }
        
        private LevelOfDetail _levelOfDetail;
        [SerializeField] private MapInteractionState _mapInteractionState;

        [Header("Map Objects Parameters")]
        [SerializeField] private MapObjectCreator _mapObjectCreator;
        [SerializeField] private double _highMidLODThreshold = 650;
        [SerializeField] private double _lowMidLODThreshold = 1000;

        [Header("Position & Placement")] 
        [SerializeField] private GameObject _placementPanel;
        [SerializeField] private TextMeshProUGUI _coordinatesText;
        [SerializeField] private MapObject _selectedLocationIcon;
        [SerializeField] private GameObject _myLocationPrefab;
        [SerializeField] private ModelAsset _defaultModelAsset;
        
        
        [Header("Utilities")]
        [SerializeField] private Camera _mapCamera;
        [SerializeField] private LightshipMap _map;
        
        // My Position
        private Map2DMarker[] _myLocationObjects;
        private LatLng _myPosition;

        // Selecting a Position
        private LatLng _currentCoordinates;
        private float _touchTimer = 0;
        private float _timeToSelect = 1f;
        private Vector2 _initialTouchPosition;
        private float _touchDistanceThreshold = 10f;
        private double _lastMapRadius;
        private bool _coordinatesChosen = false;

        private bool _locationServiceInitialized = false;

        private void Start()
        {
            _selectedLocationIcon.ToggleVisibility(false);
            _placementPanel.SetActive(false);

            onPositionChosen.AddListener(SetCoordinates);
            LocationService.Instance.onLocationServiceInitialized.AddListener(Init);
            Init();
        }

        private void Init()
        {
            _locationServiceInitialized = true;
            InitializeMyLocation();
            LoadMapObjects();
        }

        private void InitializeMyLocation()
        {
            _myLocationObjects = _myLocationPrefab.GetComponentsInChildren<Map2DMarker>();
            foreach (var obj in _myLocationObjects)
            {
                obj.ToggleVisibility(true);
                obj.Initialize(LocationService.Instance.coordinates);
            }
            
            Debug.Log("Location Initialized");
        }

        /// <summary>
        /// Loads Map Objects from UserDataManager's list of PlacedModels
        /// </summary>
        [Button]
        private void LoadMapObjects()
        {
            Debug.Log("Load Map Objects Invoked");

            var dataManager = MapDataManager.Instance;
            
            if (dataManager.CoordinatesDict.Count <= 0) return;
            
            Debug.Log($"User Data Manager Coordinates Count: {dataManager.CoordinatesDict.Count}");

            foreach (var kvp in dataManager.CoordinatesDict)
            {
                var coordinates = kvp.Value;

                // replace with Resources.Load to load model based on kvp.key/name, then pull sprite data
                var splitKey = kvp.Key.Split(":");
                var filePath = splitKey[0];
                var modelAsset = Resources.Load<GameObject>(filePath).GetComponent<ModelAsset>();
                
                Debug.Log($"File Path: {filePath}, Sprite Name: {modelAsset.sprite.name}");
                
                _mapObjectCreator.CreateNewMapObjectGroup(coordinates, modelAsset);
                
                Debug.Log($"Map Object created at: {coordinates.Latitude}, {coordinates.Longitude}");
            }
            
            Debug.Log("All Map Objects Loaded");
        }

        void Update()
        {
            if (!_locationServiceInitialized) return;
            
            UpdateMyPosition();
            UpdateLevelOfDetail();

            if (_mapInteractionState == MapInteractionState.Viewing) return;
            ChoosePositionOnHold();
        }

        private void UpdateMyPosition()
        {
            foreach (var obj in _myLocationObjects)
            {
                obj.SetCoordinates(LocationService.Instance.coordinates);

                if (obj.renderSpace == Map2DMarker.RenderSpace.ScreenSpace)
                {
                    obj.transform.rotation = GetScreenSpaceHeading(LocationService.Instance.heading);
                }
            }
        }
        
        private Quaternion GetScreenSpaceHeading(float heading)
        {
            var screenHeading = Quaternion.Euler(0, 0, -heading);
            return screenHeading;
        }

        // should separate these input methods out into a new class - MapInputManager
        private void ChoosePositionOnHold()
        {
            if (PlatformAgnosticInput.TouchCount == 1)
            {
                var touch = PlatformAgnosticInput.GetTouch(0);
                if (CheckForTouchChanged(touch)) _touchTimer = 0;
                else _touchTimer += Time.deltaTime;
                
                if (_touchTimer >= _timeToSelect)
                {
                    var currentTouchPosition = touch.position;
                    var worldPosition = _mapCamera.ScreenToWorldPoint(currentTouchPosition);
                    worldPosition.y = 0;
                    
                    onPositionChosen.Invoke(worldPosition);
                    _touchTimer = 0;
                }
            }
        }

        // should separate these input methods out into a new class - MapInputManager
        private bool CheckForTouchChanged(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _initialTouchPosition = touch.position;
            }

            Vector2 currentTouchPosition = touch.position;
            float distance = Vector2.Distance(_initialTouchPosition, currentTouchPosition);

            if (touch.phase == TouchPhase.Ended) return true;
            if (distance > _touchDistanceThreshold) return true;
            return false;
        }

        // should separate these input methods out into a new class - MapInputManager
        private void SetCoordinates(Vector3 worldPosition)
        {
            if (!_placementPanel.activeSelf) _placementPanel.SetActive(true);
            
            var unitConverter = new MapUnitConverter(_map.MapCenter);
            
            _currentCoordinates = unitConverter.SceneToLatLng(worldPosition);
            _selectedLocationIcon.ToggleVisibility(true);
            _selectedLocationIcon.Initialize(_currentCoordinates);
                    
            _coordinatesChosen = true;
            _coordinatesText.text = $"{_currentCoordinates.Latitude}, \n" +
                                    $"{_currentCoordinates.Longitude}";
        }
        
        // should separate these input methods out into a new class - MapInputManager
        public void ResetCoordinates()
        {
            _currentCoordinates = new LatLng();
            _coordinatesText.text = "NO COORDINATES";
            _coordinatesChosen = false;
            _selectedLocationIcon.ToggleVisibility(false);
        }
        
        // should separate these input methods out into a new class - MapInputManager
        public void PlaceObject(GameObject obj)
        {
            var model = obj.GetComponent<ModelAsset>();
            
            if (!_coordinatesChosen) return;
            
            _mapObjectCreator.CreateNewMapObjectGroup(_currentCoordinates, model);
            _selectedLocationIcon.ToggleVisibility(false);
            
            // place terrain anchor
        }

        private void UpdateLevelOfDetail()
        {
            if (_map.MapRadius > _highMidLODThreshold && _lastMapRadius < _highMidLODThreshold)
            {
                _levelOfDetail = LevelOfDetail.Mid;
                onLODChanged.Invoke(_levelOfDetail);
            }
            else if (_map.MapRadius < _highMidLODThreshold && _lastMapRadius > _highMidLODThreshold)
            {
                _levelOfDetail = LevelOfDetail.High;
                onLODChanged.Invoke(_levelOfDetail);
            }

            if (_map.MapRadius > _lowMidLODThreshold && _lastMapRadius < _lowMidLODThreshold)
            {
                _levelOfDetail = LevelOfDetail.Low;
                onLODChanged.Invoke(_levelOfDetail);
            }
            else if (_map.MapRadius < _lowMidLODThreshold && _lastMapRadius > _lowMidLODThreshold)
            {
                _levelOfDetail = LevelOfDetail.Mid;
                onLODChanged.Invoke(_levelOfDetail);
            }

            _lastMapRadius = _map.MapRadius;
        }
    }
}

