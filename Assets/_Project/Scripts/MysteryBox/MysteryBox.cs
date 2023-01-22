using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Models;
using _Project.Scripts.UI;
using Buck;
using UnityEngine;


public class MysteryBox : Model
{
    [HideInInspector] public Prize currentPrize;
    [HideInInspector] public bool isInteractable;
    [HideInInspector] public bool boxOpened;

    [Header("Data Persistance")] 
    [SerializeField] private string _fileName = "Mystery Box";
    
    [Header("Interaction")]
    [SerializeField] private float _interactableThreshold = 3.5f;
    [SerializeField] private Collider _boxCollider;
    
    
    [SerializeField] private Transform _prizeTransform;
    [SerializeField] private GameObject _openParticles;
    [SerializeField] private float _particleTime = 0.9f;
    
    [Header("Air Messages")] 
    [SerializeField] private AirMessage _airMessage;
    [Space(20)]
    [TextArea(5, 10)] 
    [SerializeField] private string _unopenedMessage;
    [Space(20)]
    [TextArea(5, 10)] 
    [SerializeField] private string _allowanceReachedMessage;
    [Space(20)]
    [TextArea(5, 10)] 
    [SerializeField] private string _finalMessage;
    
    private Camera _mainCamera;
    private Renderer[] _boxRenderers;
    private GameObject _currentMessage;
    private float _destroyDelay = 1.5f;
    
    // private bool _boxPlaced;
    
    
    protected override void Start()
    {
        base.Start();
        
        _mainCamera = FindObjectOfType<Camera>();
        _boxRenderers = GetComponentsInChildren<Renderer>();

        MysteryBoxManager.Instance.onBoxAllowanceReached.AddListener(ShowAllowanceReachedMessage);
        MysteryBoxManager.Instance.onMyCollectiblesClicked.AddListener(ShowFinalMessage);
        
        if (MysteryBoxManager.Instance.boxAllowanceReached) _airMessage.UpdateText(_allowanceReachedMessage);
        else _airMessage.UpdateText(_unopenedMessage);
    }

    protected override void Update()
    {
        base.Update();
        
        CheckIfInteractable();
        // CheckIfBoxPlaced();
    }
    
    
    /// <summary>
    /// Overridden from PlaceableObject to include load prize name
    /// </summary>
    /// <param name="data"></param>
    /// <param name="groupAnchor"></param>
    public override void Restore(PlaceableObjectData data, PlaceablesGroup groupAnchor)
    {
        base.Restore(data, groupAnchor);

        currentPrize = Resources.Load<GameObject>(data.PrizeName).GetComponent<Prize>();
        SetPrizeData(currentPrize);
    }
    
    /// <summary>
    /// Overridden from PlaceableObject to include prize name to load.
    /// Allows loading from resources folder rather than predefined lists
    /// setup within components in the hierarchy
    /// </summary>
    /// <returns></returns>
    public override PlaceableObjectData GetData()
    {
        base.GetData();
        
        // Debug.Log("Mysterybox Get Data triggered");

        // here we could do an if/else statement to keep the _plaeableData.FileName
        // as prizeName if box has been opened, so the prize will be instantiated on 
        // new session rather than another mysterybox containing that prize
        // for this to continue to work, prizes need to inherit from model/placeableObject
        // or else they wont be saved the next time around
        _placeableData.FileName = _fileName;
        _placeableData.PrizeName = currentPrize.prizeName;

        return _placeableData;
    }

    public void SetPrizeData(Prize prize)
    {
        currentPrize = prize;
        modelImage = prize.prizeImage;
        modelName = prize.prizeName;
        modelDescription = prize.prizeDescription;
        
        Debug.Log($"Prize Set: {currentPrize.gameObject.name}");
    }
    
    private void CheckIfInteractable()
    {
        float distance = Vector3.Distance(transform.position, _mainCamera.transform.position);

        if (distance < _interactableThreshold) isInteractable = true;
        else isInteractable = false;
        
    }

   // private void CheckIfBoxPlaced()
   // {
   //     if (_boxPlaced) return;
   //     if (_placeableState == PlaceableState.Finalized)
   //     {
   //         ModelManager.Instance.TriggerOnModelPlaced();
   //         _boxPlaced = true;
   //         
   //         _placeableData.FileName = modelName;
   //         ObjectTrackerManager.Instance.CreateNewObjectTracker(this);
   //     }
   // }
    
    public void OpenBox()
    {
        if (!isInteractable) return;
        Debug.Log("Box Opened");
        
        boxOpened = true;
        HideBox();
        InstantiateParticles();
        StartCoroutine(InstantiatePrize());
    }

    private void HideBox()
    {
        _boxCollider.enabled = false;
        
        foreach (var rend in _boxRenderers)
        {
            rend.enabled = false;
        }
    }

    private void InstantiateParticles()
    {
        GameObject particles = Instantiate(_openParticles.gameObject, _prizeTransform.position, Quaternion.identity);
        particles.transform.parent = gameObject.transform;
        particles.GetComponent<ParticleSystem>().Play();
        Destroy(particles, _particleTime);
    }

    private IEnumerator InstantiatePrize()
    {
        yield return new WaitForSeconds(_particleTime);
        _airMessage.UpdateText(currentPrize.customAirMessage);
        GameObject thisPrize = Instantiate(currentPrize.gameObject, _prizeTransform.position, Quaternion.identity);
        thisPrize.transform.parent = transform;
        
        
        Debug.Log($"Prize Instantiated: {currentPrize}");
    }

    private void ShowAllowanceReachedMessage()
    {
        StartCoroutine(MessageDelay(_allowanceReachedMessage));
    }
    
    private void ShowFinalMessage()
    {
        if (!boxOpened) return;
        
        StartCoroutine(MessageDelay(_finalMessage));
    }
    
    private IEnumerator MessageDelay(string message)
    {
        yield return new WaitForSeconds(_particleTime);
        
        _airMessage.UpdateText(message);
        Debug.Log("Air Message Updated");
    }
    
   
    
}
