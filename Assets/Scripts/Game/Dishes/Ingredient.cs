using UnityEngine;

[System.Serializable]
public class Ingredient
{
    public Sprite NormalSprite;
    public Sprite FrenzySprite;
    public IngredientType Type;
}

[System.Serializable]
public enum IngredientType
{
    Egg,
    Meat,
    Rice,
    Carrot
}
