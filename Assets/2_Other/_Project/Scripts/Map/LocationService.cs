using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Unity.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


namespace PlanetX.Map
{
    public class LocationService : Singleton<LocationService>
    {
        public bool Initialized => _intitialized;
        
        public UnityEvent onLocationServiceInitialized;
        
        [SerializeField] private float _desiredAccuracyInMeters = 10f;
        [SerializeField] private float _updateDistanceInMeters = 10f;

        [HideInInspector] public double latitude;
        [HideInInspector] public double longitude;
        [HideInInspector] public LatLng coordinates;
        [HideInInspector] public float heading;
        
        private bool _initialLocationSet = false;

        private bool _intitialized;

        public override void Awake()
        { 
            base.Awake();
            DontDestroyOnLoad(this);
        }
        
        IEnumerator Start()
        {
            // Waits for Unity Remote
#if UNITY_EDITOR
            yield return new WaitWhile(() => !UnityEditor.EditorApplication.isRemoteConnected);
            yield return new WaitForSecondsRealtime(5f);
#endif
            // Checks if the user has location service enabled
            if (!Input.location.isEnabledByUser)
                yield break;
            
            // Starts the location service
            Input.location.Start(_desiredAccuracyInMeters, _updateDistanceInMeters);
            Input.compass.enabled = true;

            // Waits until the location service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1f);
                maxWait--;
            }
            
            // Extra wait time for service to initialize in editor
#if UNITY_EDITOR
            int editorMaxWait = 15;
            while (UnityEngine.Input.location.status == LocationServiceStatus.Stopped && editorMaxWait > 0) {
                yield return new WaitForSecondsRealtime(1);
                editorMaxWait--;
            }
#endif
            
            // If the service didn't doesn't initialize during the set timeframe,
            // the location service use is cancelled.
            if (maxWait < 1)
            {
                Debug.Log("Location service timed out");
                yield break;
            }
            
            // If the connection fails this cancels the use of the location service
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to determine device location");
                yield break;
            }
            else
            {
                // If the connections succeeds, this retrieves the device's current location
                // and displays it in the console window
                var lastData = Input.location.lastData;
                Debug.Log($"Latitude: {lastData.latitude} \n" +
                          $"Longitude: {lastData.longitude} \n" +
                          $"Altitude: {lastData.altitude} \n" +
                          $"Horizontal Accuracy: {lastData.horizontalAccuracy} \n" +
                          $"Timestamp: {lastData.timestamp}");
            }
            
            onLocationServiceInitialized.Invoke();
            _intitialized = true;
            Debug.Log($"Location Service Initialized, Status: {Input.location.status}");
        }

        private void StopLocationService()
        {
            Input.location.Stop();
        }

        private void Update()
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            heading = Input.compass.trueHeading;
            coordinates = new LatLng(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }
    }
}
