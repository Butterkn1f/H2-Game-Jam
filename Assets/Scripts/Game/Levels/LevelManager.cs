using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class contains the overall behaviors for the level 
/// This includes setting the whole level up, and other things (undecided)
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region level-specific variables

    [SerializeField] private LevelData _currentLevelData;

    #endregion

    #region level editable variables 

    [SerializeField] private Image _backgroundImage;
    private CustomerManager _customerManager;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Set background image
        // Should be in a different class
        _backgroundImage.sprite = _currentLevelData.LevelLocation.BackgroundImage;
        // Make sure it doesnt get squashed
        _backgroundImage.SetNativeSize();
        

        _customerManager = GetComponent<CustomerManager>();
        _customerManager.SetCustomerList(_currentLevelData.LevelLocation.CustomerList);
        _customerManager.SendNewCustomer();
        Debug.Log("What.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
