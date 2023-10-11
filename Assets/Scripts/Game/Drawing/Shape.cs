using UnityEngine;

[System.Serializable]
public class Shape
{
    public ShapeType Type;
    public Color Color;
    public Sprite Sprite;
}

[System.Serializable]
public enum ShapeType
{
    Circle,
    Horizontal,
    Up,
    Down
}
