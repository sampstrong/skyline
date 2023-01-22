using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YelpBusinessDataPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _latitudeText;
    [SerializeField] private TextMeshProUGUI _longitudeText;
    [SerializeField] private Image _mainImage;
    [SerializeField] private Image _ratingImage;
    [SerializeField] private List<Sprite> _ratingSprites;

    [SerializeField] private GameObject _reviewPrefab;
    
    private YelpBusinessData _yelpBusinessData;

    private List<YelpReviewPanel> _reviewPanels = new List<YelpReviewPanel>();

    public void Initialize(YelpBusinessData businessData)
    {
        _yelpBusinessData = businessData;

        _nameText.text = businessData.name;
        _latitudeText.text = businessData.latitude.ToString(CultureInfo.CurrentCulture);
        _longitudeText.text = businessData.longitude.ToString(CultureInfo.CurrentCulture);
        _ratingImage.sprite = SetRatingImage(businessData.rating);
        SetImage(businessData);
        LoadReviews(businessData);
    }
    
    private Sprite SetRatingImage(float rating)
    {
        Sprite sprite;
        
        switch (rating)
        {
            case 0f:
                sprite = _ratingSprites[0];
                break;
            case 1f:
                sprite = _ratingSprites[1];
                break;
            case 1.5f:
                sprite = _ratingSprites[2];
                break;
            case 2f:
                sprite = _ratingSprites[3];
                break;
            case 2.5f:
                sprite = _ratingSprites[4];
                break;
            case 3f:
                sprite = _ratingSprites[5];
                break;
            case 3.5f:
                sprite = _ratingSprites[6];
                break;
            case 4f:
                sprite = _ratingSprites[7];
                break;
            case 4.5f:
                sprite = _ratingSprites[8];
                break;
            case 5f:
                sprite = _ratingSprites[9];
                break;
            default:
                sprite = _ratingSprites[0];
                Debug.Log("No Rating Found");
                break;
        }

        return sprite;
    }
    
    private void SetImage(YelpBusinessData businessData)
    {
        if (businessData.texture == null) return;
        
        _mainImage.sprite =
            Sprite.Create(businessData.texture, new Rect(0, 0, businessData.texture.width, businessData.texture.height), Vector2.zero);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(715, 450);
    }

    private void LoadReviews(YelpBusinessData businessData)
    {
        if (businessData.reviews.Count <= 0) return;
        
        foreach (var review in businessData.reviews)
        {
            var reviewObject = Instantiate(_reviewPrefab, gameObject.transform.parent).GetComponent<YelpReviewPanel>();
            var ratingSprite = SetRatingImage(review.rating);
            reviewObject.Initialize(review, ratingSprite);
            _reviewPanels.Add(reviewObject);
        }
    }

    private void OnDestroy()
    {
        if (_reviewPanels.Count <= 0) return;
        foreach (var review in _reviewPanels)
        {
            Destroy(review.gameObject);
        }
    }
}
