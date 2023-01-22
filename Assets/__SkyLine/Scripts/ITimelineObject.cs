
using Niantic.Lightship.Maps;

public interface ITimelineObject
{
    public string Name { get; }
    public int Date { get; }
    public float Height { get; }
    public LatLng Coordinates { get; }

    /// <summary>
    /// Behavior to toggle the visibility of the object based on
    /// it's date on a timeline
    /// </summary>
    public void SetVisibilityByDate(int date);
    
    /// <summary>
    /// Behavior when the object is selected
    /// </summary>
    public void Select();

    /// <summary>
    /// Behavior when the object is deselected
    /// </summary>
    public void Deselect();
}


