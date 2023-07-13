using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YelpBusinessData : IComparer<YelpBusinessData>
{
    public string id;
    public string name;
    public float rating;
    public string image_url;
    public Texture2D texture;
    public double latitude;
    public double longitude;
    public float distance;
    public List<YelpReviewData> reviews;


    public YelpBusinessData()
    {
    }


    public int Compare(YelpBusinessData x, YelpBusinessData y)
    {
        if (x != null && y != null) return x.distance.CompareTo(y.distance);
        else return 0;
    }
}
