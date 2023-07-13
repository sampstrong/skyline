using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Models;
using _Project.Scripts.UI;
using UnityEngine;

public class Prize : BaseFacingObject
{
    [Header("Prize Info")]
    public Sprite prizeImage;
    public string prizeName;
    public string prizeDescription;
    
    [Header("Air Message")]
    [TextArea(5, 10)] 
    public string customAirMessage;
}
