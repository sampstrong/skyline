using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableMapObject
{
    /// <summary>
    /// Called to create custom behavior when an object is seen
    /// </summary>
    public void OnObjectSeen();

    /// <summary>
    /// Called to create custom behavior when an object is interacted with
    /// </summary>
    public void OnObjectInteractedWith();
}
