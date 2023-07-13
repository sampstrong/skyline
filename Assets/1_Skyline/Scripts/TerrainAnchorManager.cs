using System;
using System.Collections;
using System.Threading.Tasks;
using Buck;
using UnityEngine;
using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class TerrainAnchorManager : Singleton<TerrainAnchorManager>
{
    public ARGeospatialAnchor CurrentAnchor { get => _currentAnchor; }
    
    [SerializeField] private AREarthManager _earthManager;
    [SerializeField] private ARAnchorManager _anchorManager;
    [SerializeField] private TMP_InputField _latitudeInput;
    [SerializeField] private TMP_InputField _longitudeInput;
    [SerializeField] private TextMeshProUGUI _successText;
    
    private double latitude;
    private double longitude;
    private double altitudeAboveTerrain = 0;
    private Quaternion rotation = Quaternion.identity;

    private ARGeospatialAnchor _currentAnchor;
    
    public void SetAnchorLocationViaText()
    {
        if (_latitudeInput.text == null || _longitudeInput.text == null) return;
        
        var lat = Convert.ToDouble(_latitudeInput.text);
        var lon = Convert.ToDouble(_longitudeInput.text);
        
        SetAnchorLocation(lat, lon);
        
        _latitudeInput.text = null;
        _longitudeInput.text = null;
    }
    
    public void SetAnchorLocation(double lat, double lon)
    {
        latitude = lat;
        longitude = lon;
        
        Debug.Log($"Latitude: {latitude}, Longitude: {longitude}");
    }

    public async Task<ARGeospatialAnchor> CreateTerrainAnchor(GameObject placeable = null)
    {
        if (_earthManager.EarthState == EarthState.Enabled &&
            _earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            ARGeospatialAnchor terrainAnchor = _anchorManager.ResolveAnchorOnTerrain(
                // Locational values
                latitude,
                longitude,
                altitudeAboveTerrain,
                rotation
                );
                // This anchor can't be used immediately; check its TerrainAnchorState before rendering
                // content on this anchor.

                _currentAnchor = terrainAnchor;
                
                var state = await CheckTerrainAnchorState();

                if (state == TerrainAnchorState.Success)
                {
                    return _currentAnchor;
                }
        }

        return null;
    }


    private async Task<TerrainAnchorState> CheckTerrainAnchorState()
    {
        while (_currentAnchor.terrainAnchorState == TerrainAnchorState.TaskInProgress)
        {
            await Task.Yield();
        }
        
        switch (_currentAnchor.terrainAnchorState)
        {
            case TerrainAnchorState.Success:
                if (_currentAnchor.trackingState == TrackingState.Tracking)
                {
                    //StartCoroutine(ShowSuccessText());
                    //PlaceablesManager.Instance.PlaceObjectOnTerrainAnchor(_currentAnchor);
                    Debug.Log("Terrain Anchor State: Success");
                }
                break;
            case TerrainAnchorState.TaskInProgress:
                // ARCore is contacting the ARCore API to resolve the Terrain anchor's pose.
                // Display some waiting UI.
                Debug.Log("Terrain Anchor State: Task In Progress");
                //TryAgainLater();
                break;
            case TerrainAnchorState.ErrorUnsupportedLocation:
                // The requested anchor is in a location that isn't supported by the Geospatial API.
                Debug.Log("Terrain Anchor State: Error Unsupported Location");
                break;
            case TerrainAnchorState.ErrorNotAuthorized:
                // An error occurred while authorizing your app with the ARCore API. See
                // https://developers.google.com/ar/reference/unity-arf/namespace/Google/XR/ARCoreExtensions#terrainanchorstate_errornotauthorized
                // for troubleshooting steps.
                Debug.Log("Terrain Anchor State: Error Not Authorized");
                break;
            case TerrainAnchorState.ErrorInternal:
                // The Terrain anchor could not be resolved due to an internal error.
                Debug.Log("Terrain Anchor State: Error Internal");
                break;
            case TerrainAnchorState.None:
                // This Anchor isn't a Terrain anchor or it became invalid because the Geospatial Mode was
                // disabled.
                Debug.Log("Terrain Anchor State: None");
                break;
        }

        return _currentAnchor.terrainAnchorState;

    }


    private void TryAgainLater()
    {
        Invoke(nameof(CheckTerrainAnchorState), 1f);
    }

    private IEnumerator ShowSuccessText()
    {
        _successText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);
        
        _successText.gameObject.SetActive(false);
    }

}
