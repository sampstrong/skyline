using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Models;
using _Project.Scripts.UI;
using Buck;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class MysteryBoxManager : Singleton<MysteryBoxManager>
{
    [HideInInspector] public UnityEvent<MysteryBox> onBoxOpened;
    [HideInInspector] public UnityEvent onBoxAllowanceReached;
    [HideInInspector] public UnityEvent onMyCollectiblesClicked;
    
    [HideInInspector] public bool boxAllowanceReached;
    private int _boxesOpenedToday = 0;

    [Header("Mystery Box Parameters")]
    [SerializeField] private MysteryBox _mysteryBox;
    [SerializeField] private int _openBoxesAllowed = 5;
    
    [Header("Misc UI")]
    [SerializeField] private UIScreen _myCollectablesButton;
    [SerializeField] private TextMeshProUGUI _currentPrizeText;
    [SerializeField] private GameObject _currentPrizeCanvas;
    
    private float _prizeDestroyTime = 2f;
    
    void Start()
    {
        ModelManager.Instance.onModelTapped.AddListener(RegisterBoxHit);
        ModelManager.Instance.onModelTapped.AddListener(RegisterPrizeHit);
        ModelManager.Instance.onCurrentModelUpdated.AddListener(SetCurrentPrizeText);
        ModelManager.Instance.onManualPlacementStarted.AddListener(ShowCurrentPrizeCanvas);
        ModelManager.Instance.onModelPlaced.AddListener(HideCurrentPrizeCanvas);
        ModelManager.Instance.onModelsCleared.AddListener(ClearPrizes);
        
        onBoxOpened.AddListener(IncrementBoxesOpened);
        onBoxOpened.AddListener(ShowMyCollectablesPopUp);
    }
    
    /// <summary>
    /// Sets the current prize for the Mystery Box and sets the current model to the mystery box.
    /// Uses current prize values to populate current model values
    /// </summary>
    /// <param name="prize"></param>
    public void SetPrize(Prize prize)
    {
        _mysteryBox.currentPrize = prize;
        _mysteryBox.modelImage = prize.prizeImage;
        _mysteryBox.modelName = prize.prizeName;
        _mysteryBox.modelDescription = prize.prizeDescription;
        
        ModelManager.Instance.SetCurrentModel(_mysteryBox);
        
        // Debug.Log($"Prize Set: {prize.prizeName}");
    }

    /// <summary>
    /// Triggered when a mystery box is tapped on. Triggers onBoxOpened event
    /// for things like instantiating prizes, changing air message text, etc.
    /// Sets the current mystery box.
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="time"></param>
    private void RegisterBoxHit(RaycastHit hit, float time)
    {
        if (boxAllowanceReached) return;
        
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (hit.transform.TryGetComponent(out MysteryBox box))
            {
                if (!box.isInteractable) return;
                if (box.boxOpened) return;
                
                box.OpenBox();
                onBoxOpened.Invoke(box);
            }
        }
    }
    

    private void RegisterPrizeHit(RaycastHit hit, float time)
    {
        // Debug.Log("Prize Hit Registered");
        
        if (hit.transform.TryGetComponent(out Prize prize))
        {
            Debug.Log($"Destroy Timer: {time}");

            if (time > _prizeDestroyTime)
            {
                Destroy(prize.transform.parent.gameObject);

                Debug.Log("Prize Destroyed");

                ModelManager.Instance.ResetTimer();
            }
        }
    }
    
    
    private void IncrementBoxesOpened(MysteryBox box)
    {
        _boxesOpenedToday++;
        
        if (_boxesOpenedToday >= _openBoxesAllowed)
        {
            onBoxAllowanceReached.Invoke();
            boxAllowanceReached = true;
        }
    }
    
    private void ShowMyCollectablesPopUp(MysteryBox box)
    {
        UIManager.Instance.ToggleOverlay(_myCollectablesButton);
    }

    public void ClickMyCollectables(UIScreen newScreen)
    {
        UIManager.Instance.ChangeScreen(newScreen);
        _myCollectablesButton.gameObject.SetActive(false);
        onMyCollectiblesClicked.Invoke();
    }
    
    private void SetCurrentPrizeText(Model newModel)
    {
        // Debug.Log("Current Prize Text Set");
        
        _currentPrizeText.text = newModel.modelName;
    }

    private void ShowCurrentPrizeCanvas()
    {
        _currentPrizeCanvas.SetActive(true);
    }

    private void HideCurrentPrizeCanvas()
    {
        _currentPrizeCanvas.SetActive(false);
    }

    private void ClearPrizes()
    {
        Prize[] prizes = FindObjectsOfType<Prize>();

        foreach (var prize in prizes)
        {
            Destroy(prize.gameObject);
        }
    }

    
}
