using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dish
{
    public string Name;
    public Sprite Sprite;
    public int MinShapes;
    public int MaxShapes;
    public List<IngredientType> Ingredients;
    public List<ShapeType> AllowedShapes;
}
