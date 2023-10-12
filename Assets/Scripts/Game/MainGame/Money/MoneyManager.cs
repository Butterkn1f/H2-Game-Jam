using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.DesignPatterns;

public class MoneyManager : Singleton<MoneyManager> 
{
    // Money variables
    public float Profit;
    public float AmountEarned;
    public float Tips;
    public float IngredientFees;
    public float WasteCost;

    [SerializeField, Range(0, 1)] private float _maxTipPercentage = 0.75f;
    private float _dishCost;
    private float _ingredientCost;
    private float _restaurantFee;

    public float NumStarsEarned;

    // Start is called before the first frame update
    void Start()
    {
        Profit = 0;
        AmountEarned = 0;
        Tips = 0;
        IngredientFees = 0;
        WasteCost = 0;
    }

    public void SetLevelData(float dishCost, float restaurantFee, float ingredientCost)
    {
        _dishCost = dishCost;
        _restaurantFee = restaurantFee;
        _ingredientCost = ingredientCost;
    }

    public void SetStarsData(float twoStarCriteria, float threeStarCriteria)
    {
        if (Profit > 0)
        {
            NumStarsEarned += 1;
        }
        if (Profit >= twoStarCriteria)
        {
            NumStarsEarned += 1;
        }
        if (Profit >= twoStarCriteria)
        {
            NumStarsEarned += 1;
        }
    }

    public void SetRestaurantFees(float newRestaurantFee)
    {
        IngredientFees = newRestaurantFee;
    }

    public void AddMoney(float patienceMeter)
    {
        AmountEarned += _dishCost;

        // Will give anywhere from 0 - 75% tip based on patience meter
        float tips = _dishCost * (patienceMeter * 0.75f);
        Tips += tips;
        IngredientFees += _ingredientCost;
    }

    public void ThrowAwayFood()
    {
        WasteCost += _dishCost;
        IngredientFees += _ingredientCost;
    }

    // For tutorial
    public void OverrideWasteCost(float newWasteCost)
    {
        WasteCost = newWasteCost;
    }

    public void CalculateProfit()
    {
        Profit = (AmountEarned + Tips) - (IngredientFees + _restaurantFee + WasteCost);
    }
}
