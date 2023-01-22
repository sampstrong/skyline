using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YelpReviewPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _reviewText;
    [SerializeField] private Image _ratingImage;
    
    public void Initialize(YelpReviewData reviewData, Sprite ratingSprite)
    {
        _nameText.text = reviewData.name;
        _reviewText.text = reviewData.text;
        _ratingImage.sprite = ratingSprite;
    }
}
