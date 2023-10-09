using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains an overall list of customer data
/// </summary>
[CreateAssetMenu(fileName = "New Customer", menuName = "Other Objects/Customer Data", order = 1)]
public class CustomerData : ScriptableObject
{
    // Name of customer 
    // I'm going to use this as a unique identifier for the characters sue me (i am lazy to add a enum)
    public string Name;

    // Character Sprite
    public Sprite CharacterSprite;

    public float PatienceDuration = 15.0f;

}
