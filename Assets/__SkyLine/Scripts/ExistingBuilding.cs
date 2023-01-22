using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistingBuilding : MonoBehaviour
{
    public int Date => _date;
    
    
    [Header("Building Data")]
    [SerializeField] private int _date;
    
    [Header("Component Refs")] 
    [SerializeField] private Renderer _renderer;

    private Material _visibleMaterial;
    private Material _occlusionMaterial;

    private void Start()
    {
        BuildingManager.Instance.Timeline.onDateChanged += SetVisibilityByDate;
        Init();
    }
    
    public void Init()
    {
        _visibleMaterial = BuildingManager.Instance.VisibleMat;
        _occlusionMaterial = BuildingManager.Instance.OcclusionMat;
        
        _renderer.enabled = false;
    }

    // should refactor this to just respond to events sent from Future building reference
    public void SetVisibilityByDate(int date)
    {
        if (!BuildingManager.Instance.ModelOn) return;
        
        if (date >= _date) _renderer.enabled = false;
        else _renderer.enabled = true;
    }
}
