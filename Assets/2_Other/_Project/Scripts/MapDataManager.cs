using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Models;
using Buck;
using Niantic.Lightship.Maps;
using Niantic.Platform.Debugging;
using SimpleJSON;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapDataManager : Singleton<MapDataManager>
{
    // List of placed models to count towards planet area
    public List<Model> PlacedModels => _placedModels;
    public List<LatLng> ModelCoordinates => _modelCoordinates;
    public Dictionary<string, LatLng> CoordinatesDict => _coordinatesDict;

    private TextMeshProUGUI _planetAreaText;

    private MapData _mapData;

    // data
    private float _currentPlanetArea;
    private List<Model> _placedModels = new List<Model>();
    private List<LatLng> _modelCoordinates = new List<LatLng>();
    private Dictionary<string, LatLng> _coordinatesDict = new Dictionary<string, LatLng>();

    private bool _dataLoaded = false;

    private string _testModel = "Waving Astronaut";
    private string _mapDataKey = "UserDataKey";

    private float _bubbleDiameter = 250f;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        LoadData();
        InitMainScene();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene == SceneManager.GetSceneByName("OrthographicMap")) SetPlanetAreaText();
        else InitMainScene();
    }

    private void InitMainScene()
    {
        PlaceablesManager.Instance.AllGroupsLoaded.AddListener(OnAllGroupsLoaded);
        PlaceablesManager.Instance.ObjectPlaced.AddListener(OnObjectPlaced);
    }


    /// <summary>
    /// Invoked once all PlaceablesGroups have loaded so data can be pulled from them
    /// </summary>
    /// <param name="arg0"></param>
    private void OnAllGroupsLoaded(List<PlaceablesGroup> arg0)
    {
        SetPlacedObjectsList();
    }

    /// <summary>
    /// Invoked When an object has been placed
    /// Adds data from that object to the appropriate places
    /// </summary>
    /// <param name="obj"></param>
    private void OnObjectPlaced(GameObject obj)
    {
        var model = obj.GetComponent<Model>();
        Assert.IsNotNull(model, "Placed Object does not contain a Model component. Map Data will not be saved");

        // get coordinates from model and add data to local dictionary
        LatLng coordinates = GetCoordinates(model);
        string modelName = model.modelName + ":" + model.GetInstanceID();
        
        Debug.Log($"Model Name: {modelName}");
        
        _coordinatesDict.Add(modelName, coordinates);

        // update local area data and list of placed objects
        UpdatePlanetArea(model);
        AddObjectToList(model);

        // save local data from memory to storage
        SaveData();
    }
    
    private LatLng GetCoordinates(Model model)
    {
        Debug.Log("Get Coordinates Triggered");

        // calc distance in degrees per meter for lat and long
        double degreesPerMeterLat = 0.000009; // same as 1 / 111,111
        double degreesPerMeterLong = 1 / (111111 * Mathf.Cos((float)model.Group.GroupData.Latitude));

        // get lat and long values based on local position offset from group
        // subtracted and swapped - not sure why, bu this seems to work
        // should we be using z instead of y for longitude?
        double latitude = model.Group.GroupData.Latitude - (model.transform.localPosition.x * degreesPerMeterLat);
        double longitude = model.Group.GroupData.Longitude - (model.transform.localPosition.z * degreesPerMeterLong);

        // create set of coordinates and add them to the list
        LatLng coordinates = new LatLng(latitude, longitude);
        
        Debug.Log($"Coordinates: {coordinates}");

        return coordinates;
    }
    
    /// <summary>
    /// Updates the tally for total area a user has claimed for their planet
    /// Calculation is a rough estimation only - not exact area
    /// </summary>
    /// <param name="arg0"></param>
    private void UpdatePlanetArea(Model model)
    {
        Debug.Log("Update Planet Area Invoked");

        // placeholder for now to exclude mystery boxes
        if (model.modelName != _testModel) return;

        Vector3 newModelPos = model.transform.position;

        // calculate area of a circle = pi r squared
        float newArea;
        newArea = (Mathf.PI * Mathf.Pow((_bubbleDiameter / 2), 2));

        Debug.Log($"Other Placed Models: {_placedModels.Count}");

        foreach (var placedModel in _placedModels)
        {
            // skip calculation against self
            if (model == placedModel) continue;

            Debug.Log($"Other model position: {placedModel.transform.position}");

            // get distance to other models in list
            float distance = Vector3.Distance(placedModel.transform.position, newModelPos);

            // if overlapping another bubble, rescale new area proportional to the amount of overlap
            if (distance < _bubbleDiameter)
            {
                newArea *= (distance / _bubbleDiameter);
            }
        }

        Debug.Log($"New Area: {newArea}");

        _currentPlanetArea += newArea;

        Debug.Log($"Planet Area: {_currentPlanetArea}");
    }


    /// <summary>
    /// Creates list of Models from all PlaceablesGroups
    /// </summary>
    private void SetPlacedObjectsList(GameObject arg0 = null)
    {
        _placedModels.Clear();

        foreach (var group in PlaceablesManager.Instance.PlaceablesGroups)
        {
            foreach (var obj in group.Placeables)
            {
                if (!obj.TryGetComponent(out Model model)) return;

                // placeholder for map testing
                if (model.modelName == _testModel)
                {
                    AddObjectToList(model);
                }
            }
        }
    }

    private void AddObjectToList(Model model)
    {
        _placedModels.Add(model);
        Debug.Log($"Model: {model.name} added to Placed Objects List");
    }


    private void SetPlanetAreaText()
    {
        Debug.Log("Set Planet Area Text Invoked");

        _planetAreaText = GameObject.Find("AreaTallyText").GetComponent<TextMeshProUGUI>();
        _planetAreaText.text = _currentPlanetArea.ToString("0");

        Debug.Log($"Planet Area Text Set: {_currentPlanetArea}");
    }

    /// <summary>
    /// Same the current user data to player prefs to recall in future sessions
    /// </summary>
    private void SaveData()
    {
        // can bulk save coordinates as a string using this is it doesnt work directly saving dictionary
        var coordinates = Newtonsoft.Json.JsonConvert.SerializeObject(_coordinatesDict);

        MapData mapData = new MapData(_currentPlanetArea, coordinates);
        PlayerPrefs.SetString(_mapDataKey, JsonUtility.ToJson(mapData));

        Debug.Log($"Map data to JSON: {JsonUtility.ToJson(mapData)}");

        PlayerPrefs.Save();

        Debug.Log("User Map Data Saved");
    }

    /// <summary>
    /// Loads user data from previous session
    /// </summary>
    private void LoadData()
    {
        if (!PlayerPrefs.HasKey(_mapDataKey))
        {
            Debug.Log("No map data to load");
            _dataLoaded = true;
            return;
        }

        MapData mapData = JsonUtility.FromJson<MapData>(PlayerPrefs.GetString(_mapDataKey));
        _mapData = mapData;
        _currentPlanetArea = _mapData.PlanetArea;
        _coordinatesDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, LatLng>>(_mapData.Coordinates);

        foreach (var kvp in _coordinatesDict)
        {
            Debug.Log($"Loaded: {kvp.Key}, {kvp.Value}");
        }

        Debug.Log("All User Map Data Loaded");
        _dataLoaded = true;
    }
    
    public void ClearData()
    {
        _coordinatesDict.Clear();
        _placedModels.Clear();
        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        if (!_dataLoaded) return;
        SaveData();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveData();
    }

    
}
   
