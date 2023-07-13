using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YelpReviewAirMessage : YelpAirMessage
{
    [SerializeField] private TextMeshProUGUI _username;
    [SerializeField] private TextMeshProUGUI _reviewText;

    public void Initialize(YelpReviewData data)
    {
        _username.text = data.name;
        _reviewText.text = data.text;
        _ratingImage.sprite = SetRatingImage(data.rating);
    }
}
