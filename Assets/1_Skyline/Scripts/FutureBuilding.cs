using Niantic.Lightship.Maps;
using Sirenix.OdinInspector;
using UnityEngine;

public class FutureBuilding : MonoBehaviour, ITimelineObject
{
    public string Name => _name;
    public int Date => _date;
    public float Height => _height;
    public LatLng Coordinates => _coordinates;
    public VisibilityState CurrentVisibilityState => _visibilityState;

    public enum VisibilityState
    {
        Invisible = 0,
        Visible = 1,
        Selected = 2
    }

    private VisibilityState _visibilityState;
    
    [Header("Building Data")]
    [SerializeField] private string _name;
    [SerializeField] private int _date;
    [SerializeField] private float _height;
    [SerializeField] private double _latitude;
    [SerializeField] private double _longitude;

    [Header("Component Refs")] 
    [SerializeField] private Renderer _renderer;
    
    private LatLng _coordinates;
    private Material _selectedMat;
    private Material _unselectedMat;
    private Vector3 _pinPosition;
    private float _pinOffset = 100f;
    
    void Start()
    {
        _coordinates = new LatLng(_latitude, _longitude);
        _pinPosition = gameObject.transform.position + new Vector3(0, (_height + _pinOffset), 0);
        
        // set date above 2030 if unknown
        if (_date == 0) _date = 2031;
        
        BuildingManager.Instance.Timeline.onDateChanged += SetVisibilityByDate;
        Init();
    }

    public void Init()
    {
        // Set Up Materials
        _selectedMat = BuildingManager.Instance.SelectedMat;
        _unselectedMat = BuildingManager.Instance.UnselectedMat;
        
        // initialize visibility
        SetVisibilityByDate(BuildingManager.Instance.Timeline.TimelineDate);
        
        // set location?
    }
    
    
    [Button]
    public void Select()
    {
        // Deselect other buildings first
        BuildingManager.Instance.DeselectAllBuildings();
        
        // Set select parameters for this building
        SetVisibilityState(VisibilityState.Selected);
        BuildingManager.Instance.LocationPin.gameObject.SetActive(true);
        BuildingManager.Instance.LocationPin.MoveToPosition(_pinPosition);
    }

    public void Deselect()
    {
        SetVisibilityState(VisibilityState.Visible);
        BuildingManager.Instance.LocationPin.gameObject.SetActive(false);
    }

    public void SetVisibilityByDate(int date)
    {
        if (date >= _date) SetVisibilityState(VisibilityState.Visible);
        else SetVisibilityState(VisibilityState.Invisible);
    }

    private void SetVisibilityState(VisibilityState state)
    {
        _visibilityState = state;

        switch (state)
        {
            case VisibilityState.Invisible:
                _renderer.enabled = false;
                break;
            case VisibilityState.Visible:
                _renderer.enabled = true;
                _renderer.material = _unselectedMat;
                break;
            case VisibilityState.Selected:
                _renderer.enabled = true;
                _renderer.material = _selectedMat;
                break;
            default:
                break;
        }
    }

    
    
    
}
