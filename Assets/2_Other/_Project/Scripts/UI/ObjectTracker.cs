using System.Collections;
using System.Globalization;
using Buck;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectTracker : MonoBehaviour
{
    public Canvas sortingCanvas;
    public GameObject callout;
    
    [HideInInspector] public float distance;
    
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private CanvasGroup _canvasGroup;

    private ObjectTrackerManager _objectTrackerManager;
    private Camera _camera;
    private PlaceableObject _placeableObject;
    private bool _offForReverseProjection;
    
    private IComparer _comparerImplementation;

    public void Initialize(PlaceableObject obj)
    {
        _camera = FindObjectOfType<Camera>();
        _objectTrackerManager = FindObjectOfType<ObjectTrackerManager>();
        _placeableObject = obj;
        _nameText.text = obj.PlaceableData.FileName;
    }
    
    void Update()
    {
        SetDistance();
        SetPosition();
        SetVisibility();
    }
    
    private void SetDistance()
    {
        distance = Vector3.Distance(_camera.transform.position, _placeableObject.transform.position);
        _distanceText.text = distance.ToString(format: "0", CultureInfo.CurrentCulture) + "m";
    }

    private void SetPosition()
    {
        transform.position = _camera.WorldToScreenPoint(_placeableObject.gameObject.transform.position);
    }

    private void SetVisibility()
    {
        // turn off when distance is too far
        if (distance > 250)
        {
            _canvasGroup.alpha = 0;
            return;
        }

        // turn off for negative z values (otherwise duplicates show up 180 degrees form original)
        _canvasGroup.alpha = transform.position.z < 0 ? 0 : 1;
    }

    public void ToggleCallout()
    {
        if (callout.activeSelf) callout.SetActive(false);
        else
        {
            callout.SetActive(true);
            _objectTrackerManager.TurnOffOtherCallouts(callout);
        }
    }
}
