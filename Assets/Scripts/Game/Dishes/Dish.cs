using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dish
{
    public string Name;
    public Texture Sprite;
    public int MinIngredients;
    public int MaxIngredients;
    public List<string> AllowedIngredients;
}
