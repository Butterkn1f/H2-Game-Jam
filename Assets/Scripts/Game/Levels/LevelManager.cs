using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class contains the overall behaviors for the level 
/// This includes setting the whole level up, and other things (undecided)
/// </summary>
public class LevelManager : Common.DesignPatterns.Singleton<LevelManager>
{
    #region level-specific variables

    [SerializeField] public LevelData _currentLevelData;

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
        _backgroundImage.sprite = _currentLevelData.BackgroundImage;

        _customerManager = CustomerManager.Instance;
        _customerManager.SetCustomerList(_currentLevelData.LevelLocation.CustomerList);

        // TODO: Initialize dishes from level data here
        DishManager.Instance.InitializeIngredientButtons();

        ChatGetter.Instance.StartChat(_currentLevelData.ChatID);
    }

    
}
