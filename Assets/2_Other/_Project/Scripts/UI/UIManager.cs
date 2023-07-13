using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Models;
using Buck;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public UIState CurrentUIState { get => uIState; }

        public enum UIState
        {
            Main = 0,
            Overlay = 1,
            Other = 2
        }

        private UIState uIState;
        
        [SerializeField] private UIScreen _mainScreen;
        [SerializeField] private CanvasGroup[] _debugGroups;

        private List<UIScreen> _screenHistory = new List<UIScreen>();
        private UIScreen _currentOverlay;

        private void Start()
        {
            InitializeUI();
        }

        /// <summary>
        /// Sets default UI variables when the app is opened
        /// </summary>
        private void InitializeUI()
        {
            AddToScreenHistory(_mainScreen);
            SetUIState(_mainScreen);
        }

        /// <summary>
        /// Adds screens to the history for back button functionality
        /// </summary>
        /// <param name="screenToAddToHistory"></param>
        private void AddToScreenHistory(UIScreen screenToAddToHistory)
        {
            _screenHistory.Insert(0, screenToAddToHistory);
        }

    
        /// <summary>
        /// Toggle For Overlay UI Elements
        /// </summary>
        /// <param name="menuToToggle"></param>
        public void ToggleOverlay(UIScreen overlay)
        {
            if (!overlay.gameObject.activeSelf)
            {
                overlay.gameObject.SetActive(true);
                SetUIState(overlay);
                if (_currentOverlay && _currentOverlay != overlay) 
                    _currentOverlay.gameObject.SetActive(false);
                _currentOverlay = overlay;
            }
            else if (overlay.gameObject.activeSelf)
            {
                overlay.gameObject.SetActive(false);
                SetUIState(_screenHistory[0]);
                _currentOverlay = null;
            }
        }

        public void ToggleDebugging()
        {
            foreach (var group in _debugGroups)
            {
                if (group.alpha <= 0)
                {
                    group.alpha = 1;
                }
                else if (group.alpha >= 1)
                {
                    group.alpha = 0;
                }
            }
            
        }

        public void UpdateUI(UIScreen newScreen) //replace ChangeScreen() with this when ready
        {
            if (newScreen.animation)
                StartCoroutine(AnimationDelay(newScreen, newScreen.animation.clip.length));
            else
                ChangeScreen(newScreen);
        }
        
        private IEnumerator AnimationDelay(UIScreen newScreen, float animationTime)
        {
            // need to work through transition in vs out
            _screenHistory[0].animation.Play();
            newScreen.animation.Play();
            
            yield return new WaitForSeconds(animationTime);
            ChangeScreen(newScreen);
        }

        /// <summary>
        /// Changes from one screen to another
        /// </summary>
        /// <param name="newScreen"></param>
        public void ChangeScreen(UIScreen newScreen)
        {
            if (_currentOverlay) _currentOverlay.gameObject.SetActive(false);
            if (_screenHistory[0] != newScreen)
            {
                _screenHistory[0].gameObject.SetActive(false);
                AddToScreenHistory(newScreen);
            }
            
            newScreen.gameObject.SetActive(true);
            newScreen.SetToDefault();
            
            SetUIState(newScreen);
        }
        

        /// <summary>
        /// Go back to the most recent UI Screen
        /// Swap variables for current and previous UI screens
        /// </summary>
        public void GoBack()
        {
            _screenHistory[0].gameObject.SetActive(false);
            _screenHistory.Remove(_screenHistory[0]);
            _screenHistory[0].gameObject.SetActive(true);
            
            SetUIState(_screenHistory[0]);
        }

        /// <summary>
        /// Used to deactivate individual GameObjects via button click events
        /// </summary>
        /// <param name="objectToTurnOff"></param>
        public void TurnOff(GameObject objectToTurnOff)
        {
            objectToTurnOff.SetActive(false);
        }

        /// <summary>
        /// Used to activate individual GameObjects via button click events
        /// </summary>
        /// <param name="objectToTurnOn"></param>
        public void TurnOn(GameObject objectToTurnOn)
        {
            objectToTurnOn.SetActive(true);
        }

        public void PlaceManually()
        {
            ModelManager.Instance.BeginManualPlacement();
            ChangeScreen(_mainScreen);
        }

        private void SetUIState(UIScreen newScreen)
        {
            switch (newScreen.uiType)
            {
                case UIScreen.UIType.Main:
                    uIState = UIState.Main;
                    break;
                case UIScreen.UIType.Overlay:
                    uIState = UIState.Overlay;
                    break;
                case UIScreen.UIType.Other:
                    uIState = UIState.Other;
                    break;
            }
        }

        public void ResetInteractionState()
        {
            InteractionManager.Instance.CurrentInteractionState = InteractionState.None;
        }
        
        
    }
}
