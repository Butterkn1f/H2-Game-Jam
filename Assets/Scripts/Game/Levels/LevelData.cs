using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This needs to be edited thanks

#region Level Data Scriptable Object
/// <summary>
/// This class contains the overall data container for the level 
/// NOT DONE !! PLS CHANGE IF NECESSARY
/// </summary>
[CreateAssetMenu(fileName = "New Level Data", menuName = "Levels/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    // Overall level information
    // Level ID?
    public int LevelNumber = 1;
    public string LevelName = "Level 1"; // Change/delete if necessary

    public Sprite BackgroundImage;

    // Add location data structure here
    [Tooltip("Current Level Location Data (Scriptable Object)")]
    public LocationData LevelLocation;

    // Add pre-level talking dialogue
    [Tooltip("Start of level chat ID")]
    public string ChatID;

    [Header("Cooking")]
    [Tooltip("Available ingredients in this level")]
    public List<Ingredient> Ingredients = new List<Ingredient>();
    [Tooltip("Available dishes in this level")]
    public List<Dish> Dishes = new List<Dish>();


    // Other information (delete if needed)
    [Tooltip("Amount to be removed when calculating revenue")]
    public float IngredientCost = 0;

    // Due to lack of time, will be saving values here.
    // super super super bad but :(
    [Header("Saved Values")]
    public bool IsUnlocked = false;
    [HideInInspector] public int StarsEarned = 0;
}
#endregion

#region Level List Scriptable Object
/// <summary>
/// This class contains the list of all the levels
/// </summary>
[CreateAssetMenu(fileName = "New Level List", menuName = "Levels/Level List", order = 2)]
public class LevelList : ScriptableObject
{
    [Tooltip("List of all levels")]
    public LevelData[] LevelData;
}
#endregion