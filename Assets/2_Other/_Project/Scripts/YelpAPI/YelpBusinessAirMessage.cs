using System.Collections.Generic;
using Buck;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YelpBusinessAirMessage : YelpAirMessage
{
    [SerializeField] private TextMeshProUGUI _businessName;
    [SerializeField] private MeshRenderer _imageRenderer;
    [SerializeField] private Shader _imageShader;

    private YelpBusinessData _yelpBusinessData; 

    public void Initialize(YelpBusinessData data)
    {
        _yelpBusinessData = data;
        
        _businessName.text = data.name;
        _ratingImage.sprite = SetRatingImage(data.rating);
        _imageRenderer.material = CreateImageMaterial(data.texture);
        
        Debug.Log($"Yelp Business Initialized: {data.name}");
    }

    private Material CreateImageMaterial(Texture image)
    {
        Material mat = new Material(_imageShader);
        mat.mainTexture = image;
        return mat;
    }
}
