using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.DesignPatterns;

public class MainGameManager : Singleton<MainGameManager>
{
    // Main Game
    private MainGameState _gameState;

    // Frenzy Manager
    private Frenzy _frenzy;

    // Customer Manager
    CustomerManager _customerManager;

    // Day Timer
    DayTimer _dayTimer;

    // Start is called before the first frame update
    void Start()
    {
        // Send the first customer
        _customerManager = CustomerManager.Instance;

        // Get indiv components
        _frenzy = GetComponent<Frenzy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            FinishOrder();
        }
    }

    /// <summary>
    /// Start the main game proper (the cooking portion)
    /// </summary>
    public void StartGame()
    {
        _dayTimer.StartTimer();

        _customerManager.SendNewCustomer();
    }

    public void EndGame()
    {
        _dayTimer.EndTimer();

        // Calculate money


        // Show results page
    }

    public void FinishOrder()
    {
        // Send the next customer in 
        _customerManager.LeaveCurrentCustomer();

        // Calculate tips and money

        // Get frenzy num
        _frenzy.AddFrenzyMeter();
    }

    public void BreakOrder()
    {
        // Get frenzy num
        _frenzy.BreakFrenzyMeter();

        // Calculate tips and money
    }

}

public enum MainGameState
{
    CHAT,
    MAIN_GAME,
    GAME_OVER
}
