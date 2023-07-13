using System;

[Serializable]
public class MapData
{
    public float PlanetArea;
    public string Coordinates;

    public MapData(float planetArea, string coordinates)
    {
        PlanetArea = planetArea;
        Coordinates = coordinates;
    }
}
