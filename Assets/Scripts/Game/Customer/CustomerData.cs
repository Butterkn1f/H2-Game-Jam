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

    // Prefab for customer objects
    [Tooltip("Customer prefab to be instantiated")]
    [SerializeField] public GameObject CustomerPrefab; // Get a reference for the customer prefab (to be instantiated)

    // How long the customer will wait for 
    [SerializeField, Range(1, 60)] public float PatienceDuration = 15.0f;

}
