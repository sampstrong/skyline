using System;
using System.Collections.Generic;
using _Project.Scripts.UI;
using Buck;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Models
{
    public class ModelManager : Singleton<ModelManager>
    {
        [HideInInspector] public UnityEvent onModelPlaced;
        [HideInInspector] public UnityEvent<RaycastHit, float> onModelTapped;
        [HideInInspector] public UnityEvent onTapRealeased;
        [HideInInspector] public UnityEvent onModelsCleared;
        [HideInInspector] public UnityEvent<Model> onCurrentModelUpdated;
        [HideInInspector] public UnityEvent onManualPlacementStarted;

        [HideInInspector] public Model currentModel;

        private float _timeTouched = 0;
        private Dictionary<Guid, Model> modelsInScene = new Dictionary<Guid, Model>(); // need to implement

        private void Start()
        {
            // Replace onModelPlaced with PlaceablesManager ObjectPlaced event
            // PlaceablesManager.Instance.ObjectPlaced.AddListener(EndPlacementState);
            
            InteractionManager.Instance.CurrentInteractionState = InteractionState.None;
            onModelPlaced.AddListener(EndPlacementState);
        }
        

        private void Update()
        {
            if (InteractionManager.Instance.CurrentInteractionState != InteractionState.None) return;
            if (UIManager.Instance.CurrentUIState != UIManager.UIState.Main) return;
            
            CheckForTap();
        }
        
        /// <summary>
        /// Sets the current model, which contains data for Name, Description, Image, and GameObject
        /// Used to update relevant UI and for model placement
        /// </summary>
        /// <param name="newModel"></param>
        public void SetCurrentModel(Model newModel)
        {
            currentModel = newModel;
            PlaceablesManager.Instance.PrefabsList[0] = currentModel.gameObject;
            PlaceablesManager.Instance.SetPrefabIndex(0);
            onCurrentModelUpdated.Invoke(currentModel);
            
            Debug.Log($"Current Model: {PlaceablesManager.Instance.PrefabsList[0]}");
        }

        public void BeginManualPlacement()
        {
            // Debug.Log("Manual Placement Started");

            InteractionManager.Instance.CurrentInteractionState = InteractionState.Placing;
            onManualPlacementStarted.Invoke();
        }

        public void TriggerOnModelPlaced()
        {
            onModelPlaced.Invoke();
        }

        private void EndPlacementState()
        {
            InteractionManager.Instance.CurrentInteractionState = InteractionState.None;
            // Debug.Log("Placement State Ended");
        }
        
        private void CheckForTap()
        {
            if (Input.touchCount > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //if (!hit.transform.TryGetComponent(out Model model)) return;
                    _timeTouched += Time.deltaTime;
                    
                    onModelTapped.Invoke(hit, _timeTouched);
                    
                    //Debug.Log($"Tapped Model: {model.name}");
                }
                
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    ResetTimer();
                    onTapRealeased.Invoke();
                }
            }
        }

        public void ResetTimer()
        {
            _timeTouched = 0f;
        }
        
        
        /// <summary>
        /// Placeholder
        /// Need to refactor
        /// </summary>
        public void ClearModels()
        {
            Debug.Log("Clear Objects Triggered");
            
            onModelsCleared.Invoke();
        
            Model[] models = FindObjectsOfType<Model>();

            foreach (var model in models)
            {
                Destroy(model.gameObject);
            }
            

            PlaceablesManager.Instance.ClearData();
            ObjectTrackerManager.Instance.ClearObjectIndicators();
            MapDataManager.Instance.ClearData();
            
            //PlaceablesManager.Instance.SaveImmediate();
        
            Debug.Log("Objects Cleared & Data Saved");
        }
        
        
    }
   
}
