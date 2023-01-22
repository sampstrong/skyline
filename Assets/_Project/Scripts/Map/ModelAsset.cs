using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAsset : MonoBehaviour
{
    public Sprite sprite;

    public enum ModelOwnershipType
    {
        None = 0,
        MyModel = 1,
        OtherModel = 2
    }

    public ModelOwnershipType modelOwnershipType;

    public enum ModelInteractionType
    {
        Unseen = 0,
        Seen = 1,
        Interacted = 2
    }

    public ModelInteractionType modelInteractionType;
}
