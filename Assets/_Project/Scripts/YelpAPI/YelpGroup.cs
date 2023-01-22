using System.Collections;
using System.Collections.Generic;
using Buck;
using UnityEngine;

public class YelpGroup : PlaceableObject
{
    [HideInInspector] public bool controlledBillboardingOn = false;
    
    [SerializeField] private YelpBusinessAirMessage _businessAirMessage;
    [SerializeField] private List<YelpReviewAirMessage> _reviewAirMessage;
    [SerializeField] private BaseFacingObject _billboard;

    private Camera _camera;

    private void Start()
    {
        _camera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        ToggleBillboard();
    }

    public void Initialize(YelpBusinessData data)
    {
        Debug.Log("Yelp Group Initialize Invoked");
        _businessAirMessage.gameObject.SetActive(true);
        _businessAirMessage.Initialize(data);
        SetPlaceableData(data);

        if (data.reviews.Count <= 0) return;
        for (int i = 0; i < data.reviews.Count; i++)
        {
            _reviewAirMessage[i].gameObject.SetActive(true);
            _reviewAirMessage[i].Initialize(data.reviews[i]);

            MoveUp(_businessAirMessage.gameObject);
            
            if (i - 1 < 0) continue;
            MoveUp(_reviewAirMessage[i - 1].gameObject);
            
            if (i - 2 < 0) continue;
            MoveUp(_reviewAirMessage[i - 2].gameObject);
        }
    }
    
    private void SetPlaceableData(YelpBusinessData data)
    {
        if (_placeableData == null) _placeableData = new PlaceableObjectData(0, new Pose());
        _placeableData.FileName = data.name;
    }

    private void MoveUp(GameObject obj)
    {
        Debug.Log($"Object Moved Up: {obj.name}");
        var currentPos = obj.transform.localPosition;
        obj.transform.localPosition = new Vector3(currentPos.x,
                                                currentPos.y + 1.6f,
                                                  currentPos.z);
    }

    private void ToggleBillboard()
    {
        if (!controlledBillboardingOn) return;
        
        var threshold = 20f;
        var distance = Vector3.Distance(transform.position, _camera.transform.position);

        _billboard.enabled = distance < threshold ? false : true;
    }
}
