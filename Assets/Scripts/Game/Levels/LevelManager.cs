using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class contains the overall behaviors for the level 
/// This includes setting the whole level up, and other things (undecided)
/// </summary>
// TODO: Change this to singleton persistent
public class LevelManager : Common.DesignPatterns.Singleton<LevelManager>
{
    #region level-specific variables
    [SerializeField] private List<LevelData> _levels;
    public LevelData CurrLevel => _levels[_currLevelIndex];
    #endregion

    #region level editable variables 

    [SerializeField] private Image _backgroundImage;
    private CustomerManager _customerManager;
    [SerializeField] private int _currLevelIndex = 0; //TEMP serializefield Level index, wil be changing in level manager later

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Set background image
        // Should be in a different class
        _backgroundImage.sprite = CurrLevel.BackgroundImage;

        _customerManager = CustomerManager.Instance;
        _customerManager.SetCustomerList(CurrLevel.LevelLocation.CustomerList);

        // TODO: Initialize dishes from level data here
        DishManager.Instance.InitializeIngredientButtons();

        // Money
        MoneyManager.Instance.SetLevelData(CurrLevel.DishCost, CurrLevel.RestaurantFees, CurrLevel.IngredientCost);

        ChatGetter.Instance.StartChat(CurrLevel.ChatID);
    }

    
}
