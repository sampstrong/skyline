using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps.Samples.OrthographicCamera;
using PlanetX.Map;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapObjectGroup : MonoBehaviour, IInteractableMapObject
{
    public List<MapObject> HighLODObjects { get => _highLODObjects; set => _highLODObjects = value; }
    public List<MapObject> MidLODObjects { get => _midLODObjects; set => _midLODObjects = value; }
    public List<MapObject> LowLODObjects { get => _lowLODObjects; set => _lowLODObjects = value; }

    public MapObjectGroupData GroupData { get => _groupData; set => _groupData = value; }

    // Maybe make all of these variables within group data and just reference group data on init?
    private List<MapObject> _highLODObjects = new List<MapObject>();
    private List<MapObject> _midLODObjects = new List<MapObject>();
    private List<MapObject> _lowLODObjects = new List<MapObject>();
    private List<MapObject> _map3DObjects = new List<MapObject>();
    
    private MapObjectGroupData _groupData;
    
    private void Start()
    {
        //Initialize();
        MapObjectManager.Instance.onLODChanged.AddListener(SetLOD);
    }

    public void Initialize(List<MapObject> highLODObjects, List<MapObject> midLODObjects, List<MapObject> lowLODObjects, List<MapObject> map3DObjects)
    {
        _highLODObjects = highLODObjects;
        _midLODObjects = midLODObjects;
        _lowLODObjects = lowLODObjects;
        _map3DObjects = map3DObjects;
        SetLOD(MapObjectManager.Instance.CurrentLevelOfDetail);
    }

    private void SetLOD(MapObjectManager.LevelOfDetail levelOfDetail)
    {
        switch (levelOfDetail)
        {
            case MapObjectManager.LevelOfDetail.High:
                ToggleLODVisibility(_highLODObjects, true);
                ToggleLODVisibility(_midLODObjects, false);
                ToggleLODVisibility(_lowLODObjects, false);
                break;
            case MapObjectManager.LevelOfDetail.Mid:
                ToggleLODVisibility(_highLODObjects, false);
                ToggleLODVisibility(_midLODObjects, true);
                ToggleLODVisibility(_lowLODObjects, false);
                break;
            case MapObjectManager.LevelOfDetail.Low:
                ToggleLODVisibility(_highLODObjects, false);
                ToggleLODVisibility(_midLODObjects, false);
                ToggleLODVisibility(_lowLODObjects, true);
                break;
            default:
                ToggleLODVisibility(_highLODObjects, true);
                ToggleLODVisibility(_midLODObjects, false);
                ToggleLODVisibility(_lowLODObjects, false);
                break;
        }
    }

    private void ToggleLODVisibility(List<MapObject> objects, bool isVisible)
    {
        foreach (var obj in objects)
        {
            obj.ToggleVisibility(isVisible);
        }
    }

    public void OnObjectSeen()
    {
        // change bubble size
        // chang icon
    }

    public void OnObjectInteractedWith()
    {
        // change bubble size
        // add emojis
    }
}
