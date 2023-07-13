using System;
using Buck;
using UnityEngine;

namespace _Project.Scripts.Models
{
    public class Model : PlaceableObject
    {
        public Sprite modelImage;
        public string modelName;
        public string modelDescription;

        protected bool _modelPlaced;

        protected virtual void Start()
        {
            //onPlaceableStateChanged += CheckIfPlaced;
            // SetupRenderers();
        }

        protected virtual void Update()
        {
            CheckIfBoxPlaced();
        }

        /// <summary>
        /// Overridden from PlaceableObject to include file name to load from.
        /// Allows loading from resources folder rather than predefined lists
        /// setup within components in the hierarchy
        /// </summary>
        /// <returns></returns>
        public override PlaceableObjectData GetData()
        {
            base.GetData();

            // model names must match the name of the prefab to load properly
            _placeableData.FileName = modelName;
            return _placeableData;
        }
        
        /// <summary>
        /// Triggers Model Placed Events in Model Manager
        /// Creates an Object Tracker for the model
        /// </summary>
        /// <param name="arg0"></param>
        // need to investigate why this isnt working as expected - maybe try swapping arg0 for _placeableState?
       //protected virtual void CheckIfPlaced(PlaceableState arg0)
       //{
       //    if (_modelPlaced) return;
       //    if (arg0 != PlaceableState.Finalized) return;

       //    Debug.Log("CheckIfPlaced() Triggered");
       //    
       //    ModelManager.Instance.TriggerOnModelPlaced();
       //    _modelPlaced = true;
       //    
       //    _placeableData.FileName = modelName;
       //    ObjectTrackerManager.Instance.CreateNewObjectTracker(this);
       //    
       //    // material check debugging
       //    var rends = GetComponentsInChildren<Renderer>();
       //    foreach (var rend in rends)
       //    {
       //        Debug.Log($"Renderers Enabled?: {rend.enabled}");
       //    }
       //    

       //}
       
        private void CheckIfBoxPlaced()
        {
            if (_modelPlaced) return;
            if (_placeableState == PlaceableState.Finalized)
            {
                ModelManager.Instance.TriggerOnModelPlaced();
                _modelPlaced = true;
            
                _placeableData.FileName = modelName;
                ObjectTrackerManager.Instance.CreateNewObjectTracker(this);
            }
        }
        
    }
}
