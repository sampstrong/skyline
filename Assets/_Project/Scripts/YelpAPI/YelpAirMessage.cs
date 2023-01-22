using System.Collections;
using System.Collections.Generic;
using Buck;
using UnityEngine;
using UnityEngine.UI;

public class YelpAirMessage : MonoBehaviour
{
    [SerializeField] protected List<Sprite> _ratingSprites;
    [SerializeField] protected Image _ratingImage;

    protected virtual Sprite SetRatingImage(float rating)
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
    
    
}
