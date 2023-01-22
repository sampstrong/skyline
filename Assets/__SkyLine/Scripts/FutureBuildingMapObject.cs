using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Niantic.Lightship.Maps;
using TMPro;
using UnityEngine;

public class FutureBuildingMapObject : MonoBehaviour, ITimelineObject
{
    public ITimelineObject Building
    {
        get => _building;
        set => _building = value;
    }
    public string Name => _building.Name;
    public int Date => _building.Date;
    public float Height => _building.Height;
    public LatLng Coordinates => _building.Coordinates;

    public enum VisibilityState
    {
        Invisible = 0,
        Visible = 1,
        Selected = 2
    }

    private VisibilityState _visibilityState;
    
    [SerializeField] private GameObject _positionMarker;
    [SerializeField] private GameObject _selectedPositionMarker;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _heightText;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;


    private Camera _mapCamera;
    private RectTransform _renderTexture;
    private ITimelineObject _building;
    private MapTracker _tracker;

    private bool _initialized;
    private bool _selected;

    public void Init(ITimelineObject building, MapTracker tracker)
    {
        _mapCamera = GameObject.FindWithTag("MapCamera").GetComponent<Camera>();
        _renderTexture = FindObjectOfType<RenderTextureMap>().GetComponent<RectTransform>();
        _building = building;
        _tracker = tracker;
        
        SetVisibilityState(VisibilityState.Visible);
        PopulateInfoPanel();
        BuildingManager.Instance.Timeline.onDateChanged += SetVisibilityByDate;

        _initialized = true;
    }

    private void PopulateInfoPanel()
    {
        _nameText.text = Name;
        _heightText.text = Height.ToString(CultureInfo.CurrentCulture);
    }

    private void Update()
    {
        FollowMapTracker();

        HideOffMap();
        
    }

    private void HideOffMap()
    {
        if (_rectTransform.position.y <= 805) _canvasGroup.alpha = 1;
        else _canvasGroup.alpha = 0;
    }

    private void FollowMapTracker()
    {
        if (!_initialized) return;
        var trackingPos = _tracker.transform.position;
        Vector3 screenPos = _mapCamera.WorldToViewportPoint(trackingPos);
        screenPos.Scale(new Vector3(_renderTexture.rect.width, _renderTexture.rect.height, 1f));
        transform.position = screenPos;
    }

    public void ToggleSelected()
    {
        Debug.Log("Toggle Selected Triggered");
        if (_selected) Deselect();
        else Select();
    }
    
    public void Select()
    {
        Debug.Log("Map Object Selected");
        
        BuildingManager.Instance.DeselectAllMapObjects();
        SetVisibilityState(VisibilityState.Selected);
        _building.Select();
        _selected = true;
    }

    public void Deselect()
    {
        Debug.Log("Map Object Deselected");
        
        SetVisibilityState(VisibilityState.Visible);
        _building.Deselect();
        _selected = false;
    }

    public void SetVisibilityByDate(int date)
    {
        if (date >= _building.Date) SetVisibilityState(VisibilityState.Visible);
        else SetVisibilityState(VisibilityState.Invisible);
    }

    private void SetVisibilityState(VisibilityState state)
    {
        _visibilityState = state;

        switch (state)
        {
            case VisibilityState.Invisible:
                _positionMarker.SetActive(false);
                _selectedPositionMarker.SetActive(false);
                break;
            case VisibilityState.Visible:
                _positionMarker.SetActive(true);
                _selectedPositionMarker.SetActive(false);
                break;
            case VisibilityState.Selected:
                _positionMarker.SetActive(true);
                _selectedPositionMarker.SetActive(true);
                break;
            default:
                break;
        }
    }
    
}
