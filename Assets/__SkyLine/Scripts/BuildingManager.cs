using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buck;
using Mapbox.Examples;
using Mapbox.Unity.Map;
using UnityEngine;
using UnityEngine.UI;
using LocationService = PlanetX.Map.LocationService;

public class BuildingManager : Singleton<BuildingManager>
{
    public Material UnselectedMat => _unselectedMat;
    public Material SelectedMat => _selectedMat;

    public Material OcclusionMat => _occlusionMat;
    public Material VisibleMat => _normalMat;
    public Timeline Timeline => _timeline;
    public LocationPin LocationPin => _locationPin;

    public bool ModelOn => _modelOn;
    
    [SerializeField] private GameObject _cityPrefab;
    [SerializeField] private double _originLatitude;
    [SerializeField] private double _originLongitude;
    [SerializeField] private Toggle _materialToggle;
    [SerializeField] private Toggle _rendererToggle;
    [SerializeField] private Material _occlusionMat;
    [SerializeField] private Material _normalMat;

    [Header("Selection Materials")] 
    [SerializeField] private Material _unselectedMat;
    [SerializeField] private Material _selectedMat;

    [Header("References")] 
    [SerializeField] private Timeline _timeline;
    [SerializeField] private LocationPin _locationPinPrefab;
    [SerializeField] private SpawnOnMap _mapSpawner;
    [SerializeField] private AbstractMap _map;
    

    [Header("Map Objects")] 
    [SerializeField] private FutureBuildingMapObject _mapObjectPrefab;
    [SerializeField] private Canvas _mapObjectCanvas;
    [SerializeField] private MyLocationTracker _myLocationTrackerPrefab;
    [SerializeField] private MyLocationMapObject _myLocationMapObject;

    private List<Renderer> _occlusionModelRenderers = new List<Renderer>();
    private LocationPin _locationPin;
    private OcclusionModel _occlusionModel;
    private GameObject _currentModel;
    private bool _sceneInitialized = false;
    private bool _modelOn;

    private List<ITimelineObject> _futureBuildings = new List<ITimelineObject>();
    private List<ITimelineObject> _buildingMapObjects = new List<ITimelineObject>();

    private void Start()
    {
        GeospatialManager.Instance.ErrorStateChanged.AddListener(Init);
        _materialToggle.onValueChanged.AddListener(ToggleMaterials);
        _rendererToggle.onValueChanged.AddListener(ToggleRenderers);
    }

    private async void Init(ErrorState state, string message)
    {
        if (_sceneInitialized) GeospatialManager.Instance.ErrorStateChanged.RemoveListener(Init);
        if (_sceneInitialized) return;
        if (state == ErrorState.NoError)
        {
            Debug.Log("Building Manager Initialized");
            
            var model = await PlaceOcclusionModel();
            SetUpTimelineObjects();
            SetUpRendererList();
            //InitMyLocation();
        
            _locationPin = Instantiate(_locationPinPrefab);
            _locationPin.gameObject.SetActive(false);

            _sceneInitialized = true;
        }
    }

    public async Task<OcclusionModel> PlaceOcclusionModel()
    {
        TerrainAnchorManager.Instance.SetAnchorLocation(_originLatitude, _originLongitude);
        _currentModel = await PlaceablesManager.Instance.PlaceObjectOnTerrainAnchor(false, _cityPrefab);
        _occlusionModel = _currentModel.GetComponent<OcclusionModel>();
        
        Debug.Log($"Occlusion Model: {_currentModel.name} placed");
        return _occlusionModel;
    }
    
    private void SetUpTimelineObjects()
    {
        // set list of buildings in scene - needs to happen after model is placed
        var buildings = FindObjectsOfType<FutureBuilding>();
        
        Debug.Log($"Future Buildings Count: {buildings.Length}");
        
        foreach (var building in buildings)
        {
            _futureBuildings.Add(building);
        }
        
        Debug.Log($"TimelineObject Count: {_futureBuildings.Count}");

        // spawn a map object for each building
        foreach (var building in _futureBuildings)
        {
            var tracker = _mapSpawner.SpawnMapTracker(building);
            var mapObject = Instantiate(_mapObjectPrefab, _mapObjectCanvas.transform);
            mapObject.Init(building, tracker);
            _buildingMapObjects.Add(mapObject);
        }
    }

    private void SetUpRendererList()
    {
        var renderers = _occlusionModel.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (renderer.GetComponentInParent<FutureBuilding>()) continue;
            _occlusionModelRenderers.Add(renderer);
            renderer.enabled = false;
        }
    }

    private void InitMyLocation()
    {
        var tracker = Instantiate(_myLocationTrackerPrefab);
        tracker.Init(_map);
        var myLocation = Instantiate(_myLocationMapObject, _mapObjectCanvas.transform);
        myLocation.Init(tracker);
    }
    

    private void ToggleMaterials(bool toggleOn)
    {
        Debug.Log($"Toggle Materials Triggered, Current Material: {_occlusionModel.CurrentMaterial.name}," +
                                                         $"Occlusion Mat: {_occlusionMat.name}");
        
        if (toggleOn) _occlusionModel.SetOcclusionMaterial(_occlusionMat);
        else _occlusionModel.SetOcclusionMaterial(_normalMat);
    }

    private void ToggleRenderers(bool toggleValue)
    {
        foreach (var renderer in _occlusionModelRenderers)
        {
            renderer.enabled = toggleValue;
        }

        _modelOn = toggleValue;
    }


    public void DeselectAllBuildings()
    {
        foreach (var building in _futureBuildings)
        {
            building.Deselect();
        }
    }

    public void DeselectAllMapObjects()
    {
        foreach (var mapObject in _buildingMapObjects)
        {
            mapObject.Deselect();
        }
    }

    public void ToggleMenu(GameObject menu)
    {
        if (menu.activeSelf) menu.SetActive(false);
        else menu.SetActive(true);
    }
}
