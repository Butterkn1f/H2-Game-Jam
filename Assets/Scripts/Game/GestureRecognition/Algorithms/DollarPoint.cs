using System;
using UnityEngine;

[Serializable]
public struct DollarPoint
{
    public Vector2 Point;
    public int StrokeIndex;

    public DollarPoint(float pointX, float pointY, int strokeIndex = 0)
    {
        Point = new Vector2(pointX, pointY);
        StrokeIndex = strokeIndex;
    }
}