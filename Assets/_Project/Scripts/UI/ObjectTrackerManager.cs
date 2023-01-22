using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Buck;
using UnityEngine;

public class ObjectTrackerManager : Singleton<ObjectTrackerManager>
{
    [SerializeField] private GameObject _objectTrackerPrefab;
    [SerializeField] private Canvas _objectTrackerCanvas;

    private List<ObjectTracker> _objectTrackers = new List<ObjectTracker>();
    
    public void CreateNewObjectTracker(PlaceableObject obj)
    {
        var objectTracker = Instantiate(_objectTrackerPrefab, _objectTrackerCanvas.transform).GetComponent<ObjectTracker>();
        objectTracker.Initialize(obj);
        _objectTrackers.Add(objectTracker);
    }

    public void ClearObjectIndicators()
    {
        if (_objectTrackers.Count <= 0) return;
        
        foreach (var tracker in _objectTrackers)
        {
            Destroy(tracker.gameObject);
        }
        
        _objectTrackers.Clear();
        
        Debug.Log($"Object Tracker Count: {_objectTrackers.Count}");
    }

    private void Update()
    {
        SortTrackers();
    }

    private void SortTrackers()
    {
        if (_objectTrackers.Count <= 1) return;
        
        // sort objects with objects with the furthest distance at the beginning of the list
        // we're using index to sort below, and sorting numbers that are lower will appear behind
        _objectTrackers = _objectTrackers.OrderByDescending(tracker => tracker.distance).ToList();
        
        // alternative sorting method
        // _objectTrackers.Sort((x, y) => 
        //     string.Compare(x.distance.ToString(), y.distance.ToString()));
        
        for (int i = 0; i < _objectTrackers.Count; i++)
        {
            _objectTrackers[i].sortingCanvas.sortingOrder = i;
        }
    }

    public void TurnOffOtherCallouts(GameObject calloutToStay)
    {
        foreach (var tracker in _objectTrackers)
        {
            if (tracker.callout == calloutToStay) continue;
            tracker.callout.SetActive(false);
        }
    }
}
