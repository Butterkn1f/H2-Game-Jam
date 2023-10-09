using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class acts as a data container for the location places
/// </summary>
[CreateAssetMenu(fileName = "New Location", menuName = "Other Objects/Location Data", order = 2)]
public class LocationData : ScriptableObject
{
    [Tooltip("Name of location")]
    public string LocationName = "Singapore";
    
    // Which customers are native to the location
    public List<CustomerData> CustomerList;

    // Location specific backgrounds
    public Sprite BackgroundImage;
}
