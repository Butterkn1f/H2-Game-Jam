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
    public float WasteCost;

    [SerializeField, Range(0, 1)] private float _maxTipPercentage = 0.75f;
    private float _dishCost;

    public int NumStarsEarned;
    private float _twoStarCriteria;
    private float _threeStarCriteria;

    // Start is called before the first frame update
    void Start()
    {
        Profit = 0;
        AmountEarned = 0;
        Tips = 0;
        WasteCost = 0;
    }

    public void SetData(float dishCost, float twoStarCriteria, float threeStarCriteria)
    {
        _dishCost = dishCost;
        _twoStarCriteria = twoStarCriteria;
        _threeStarCriteria = threeStarCriteria;
    }

    public void CalculateStars()
    {
        if (Profit > 0)
        {
            NumStarsEarned += 1;
        }
        if (Profit >= _twoStarCriteria)
        {
            NumStarsEarned += 1;
        }
        if (Profit >= _threeStarCriteria)
        {
            NumStarsEarned += 1;
        }
    }

    public void AddMoney(float patienceMeter)
    {
        AmountEarned += _dishCost;

        // Will give anywhere from 0 - 75% tip based on patience meter
        float tips = _dishCost * (patienceMeter * _maxTipPercentage);
        Tips += tips;
    }

    public void ThrowAwayFood()
    {
        WasteCost += _dishCost;
    }

    // For tutorial
    public void OverrideWasteCost(float newWasteCost)
    {
        WasteCost = newWasteCost;
    }

    public void CalculateProfit()
    {
        Profit = (AmountEarned + Tips) - (WasteCost);
        CalculateStars();
    }
}
