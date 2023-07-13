// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System;
using Niantic.Lightship.Maps.Unity.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Niantic.Lightship.Maps.Samples.OrthographicCamera
{
    /// <summary>
    /// A simple top-down camera controller. The camera supports panning with touch or
    /// mouse drags and zooming in and out using pinch gestures or the mouse wheel.
    /// </summary>
    public class TopDownCamera : MonoBehaviour
    {
        [SerializeField] private float _mouseScrollSpeed = 0.1f;
        [SerializeField] private float _pinchScrollSpeed = 0.002f;
        [SerializeField] private float _minimumMapRadius = 10.0f;
        [SerializeField] private Camera _camera;
        [SerializeField] private LightshipMap _map;

        private bool _isPinchPhase;
        private bool _isPanPhase;
        private float _lastPinchDistance;
        private Vector3 _lastWorldPosition;
        private float _mapRadius;

        private void Start()
        {
            _mapRadius = (float)_map.MapRadius;
            _camera.orthographicSize = _mapRadius;
        }

        private void Update()
        {
            // Mouse scroll wheel moved
            if (Input.mouseScrollDelta.y != 0)
            {
                var sizeDelta = Input.mouseScrollDelta.y * _mouseScrollSpeed * _mapRadius;
                var newMapRadius = Math.Max(_mapRadius - sizeDelta, _minimumMapRadius);

                _map.SetMapRadius(newMapRadius);
                _camera.orthographicSize = newMapRadius;
                _mapRadius = newMapRadius;
            }

            // UI element was pressed, so ignore all touch input this frame
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                return;
            }

            // Pinch logic
            if (Input.touchCount == 2)
            {
                Vector2 touch0, touch1;

                if (_isPinchPhase == false)
                {
                    // Pinch started so reset pan position
                    ResetPanTouch();

                    touch0 = Input.GetTouch(0).position;
                    touch1 = Input.GetTouch(1).position;
                    _lastPinchDistance = Vector2.Distance(touch0, touch1);

                    _isPinchPhase = true;
                }
                else
                {
                    touch0 = Input.GetTouch(0).position;
                    touch1 = Input.GetTouch(1).position;
                    float distance = Vector2.Distance(touch0, touch1);

                    var sizeDelta = (distance - _lastPinchDistance) * _pinchScrollSpeed * _mapRadius;
                    var newMapRadius = Math.Max(_mapRadius - sizeDelta, _minimumMapRadius);

                    _map.SetMapRadius(newMapRadius);
                    _camera.orthographicSize = newMapRadius;
                    _mapRadius = newMapRadius;

                    _lastPinchDistance = distance;
                }
            }
            // No pinch
            else
            {
                // Pinch so reset pan position
                if (_isPinchPhase && _isPanPhase && PlatformAgnosticInput.TouchCount == 1)
                    ResetPanTouch();

                _isPinchPhase = false;
            }

            // Pan camera by swiping
            if (PlatformAgnosticInput.TouchCount >= 1)
            {
                if (_isPanPhase == false)
                {
                    _isPanPhase = true;
                    ResetPanTouch();
                }
                else
                {
                    Vector3 currentInputPos = PlatformAgnosticInput.GetTouch(0).position;
                    currentInputPos.z = _camera.nearClipPlane;
                    var currentWorldPosition = _camera.ScreenToWorldPoint(currentInputPos);
                    currentWorldPosition.y = 0.0f;

                    var offset = currentWorldPosition - _lastWorldPosition;
                    _map.OffsetMapCenter(offset);
                    _map.transform.position += offset;
                    _lastWorldPosition = currentWorldPosition;
                }
            }
            else
            {
                _isPanPhase = false;
            }
        }

        private void ResetPanTouch()
        {
            Vector3 currentInputPos = PlatformAgnosticInput.GetTouch(0).position;
            var currentWorldPosition = _camera.ScreenToWorldPoint(currentInputPos);
            currentWorldPosition.y = 0.0f;

            _lastWorldPosition = currentWorldPosition;
        }
    }
}
