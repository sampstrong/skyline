using UnityEngine;

public interface ISelectable
{
    /// <summary>
    /// Behavior when the object is selected
    /// </summary>
    public void Select();

    /// <summary>
    /// Behavior when the object is deselected
    /// </summary>
    public void Deselect();
}
