using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buck;
using Google.XR.ARCoreExtensions;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YelpDataLoader : MonoBehaviour
{
    [SerializeField] private AREarthManager _earthManager;
    [SerializeField] private GameObject _placeablePrefab;
    [SerializeField] private Toggle _controlledBillboardingToggle;

    private List<YelpGroup> _yelpObjects = new List<YelpGroup>();

    [SerializeField] private float _radiusInMeters = 250f;
    private int _maxNumberOfResults = 20;

    private void Start()
    {
        _controlledBillboardingToggle.onValueChanged.AddListener(ToggleControlledBillboarding);
    }

    public async void GetBusinessData()
    {
        //var keyword = _textInput.text;
        var url = SetBusinessURL();
        var businessesJson = await YelpWebRequest.GetWebData(url);
        
        LoadBusinessResults(businessesJson);
    }

    private string SetBusinessURL(string keyword = null)
    {
        // get current location and swap for test coordinates
        // double testLat = 41.0510806;
        // double testLong = -73.7672382;

        var latitude = _earthManager.CameraGeospatialPose.Latitude;
        var longitude = _earthManager.CameraGeospatialPose.Longitude;

        string url;
        
        if (keyword == null)
        {
            url = $"https://api.yelp.com/v3/businesses/search?latitude={latitude}&longitude={longitude}&radius={_radiusInMeters}&sort_by=distance&limit={_maxNumberOfResults}";
        }
        else
        {
            url = $"https://api.yelp.com/v3/businesses/search?term={keyword}&latitude={latitude}&longitude={longitude}&radius={_radiusInMeters}&sort_by=distance&limit={_maxNumberOfResults}";
        }
        
        return url;
    }

    private async void LoadBusinessResults(JSONNode json)
    {
        // Remove old results
        if (_yelpObjects.Count > 0)
        {
            foreach (var result in _yelpObjects)
            {
                Destroy(result.gameObject);
            }
        }
        
        // populate results based on keyword
        var businesses = json["businesses"];
        
        for (int i = 0; i < businesses.Count; ++i)
        {
            var coordinates = businesses[i]["coordinates"];
            
            var yelpData = new YelpBusinessData
            {
                id = businesses[i]["id"],
                name = businesses[i]["name"],
                rating = businesses[i]["rating"],
                image_url = businesses[i]["image_url"],
                latitude = coordinates["latitude"],
                longitude = coordinates["longitude"]
            };
            
            
            yelpData.reviews = await LoadReviews(yelpData);
            yelpData.texture = await LoadImage(yelpData);
            var obj = await CreateObject(yelpData);
            _yelpObjects.Add(obj);
            ObjectTrackerManager.Instance.CreateNewObjectTracker(obj);
        }
    }

    private static async Task<Texture2D> LoadImage(YelpBusinessData yelpBusinessData)
    {
        var image = await ImageWebRequest.GetWebImage(yelpBusinessData.image_url);
        yelpBusinessData.texture = image;
        return image;
    }
    
    private static async Task<List<YelpReviewData>> LoadReviews(YelpBusinessData yelpBusinessData)
    {
        string reviewsURL = $"https://api.yelp.com/v3/businesses/{yelpBusinessData.id}/reviews";
        var reviewsJson = await YelpWebRequest.GetWebData(reviewsURL);
        List<YelpReviewData> yelpReviewData = new List<YelpReviewData>();
        var reviews = reviewsJson["reviews"];
        for (int j = 0; j < reviews.Count; j++)
        {
            var user = reviews[j]["user"];
            var reviewData = new YelpReviewData
            {
                text = reviews[j]["text"],
                rating = reviews[j]["rating"],
                name = user["name"]
            };

            yelpReviewData.Add(reviewData);
        }

        return yelpReviewData;
    }
    
    private async Task<YelpGroup> CreateObject(YelpBusinessData businessData)
    {
        TerrainAnchorManager.Instance.SetAnchorLocation(businessData.latitude, businessData.longitude);
        var obj = await PlaceablesManager.Instance.PlaceObjectOnTerrainAnchor(false, _placeablePrefab);
        var yelpGroup = obj.GetComponent<YelpGroup>();
        yelpGroup.Initialize(businessData);

        return yelpGroup;
    }

    private void ToggleControlledBillboarding(bool toggleValue)
    {
        Debug.Log($"Controlled Billboarding toggled: {toggleValue}");
        
        foreach (var obj in _yelpObjects)
        {
            obj.controlledBillboardingOn = toggleValue;
        }
    }

}
