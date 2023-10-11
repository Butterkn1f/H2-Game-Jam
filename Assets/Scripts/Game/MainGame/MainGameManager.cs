using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.DesignPatterns;
using UniRx.Extention;

public class MainGameManager : Singleton<MainGameManager>
{
    // Main Game
    public ReactiveProp<MainGameState> GameState = new ReactiveProp<MainGameState>();

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
        _dayTimer = GetComponent<DayTimer>();

        GameState.SetValue(MainGameState.CHAT);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            FinishOrder();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
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

    /// <summary>
    /// Trigger all the animations to be played 
    /// </summary>
    public void StartGameAnimation()
    {
        GameState.SetValue(MainGameState.MAIN_GAME);
        
    }

    public void EndGame()
    {
        _dayTimer.EndTimer();

        // End frenzy, if any
        _frenzy.BreakFrenzy();
        _frenzy.BreakFrenzyMeter();

        // Customer should leave
        _customerManager.LeaveCurrentCustomer(false);

        // Calculate money

        // Trigger animations
        GameState.SetValue(MainGameState.GAME_OVER);
    }

    /// <summary>
    /// This function is called when the player finishes an order
    /// TO LI LIAN: this is impt for ya..
    /// </summary>
    public void FinishOrder()
    {
        if (GameState.GetValue() != MainGameState.MAIN_GAME)
        {
            return;
        }

        // Send the next customer in 
        _customerManager.LeaveCurrentCustomer();

        // Calculate tips and money

        // Get frenzy num
        _frenzy.AddFrenzyMeter();
    }

    /// <summary>
    /// This function is called when the player misses the timing for the order 
    /// </summary>
    public void BreakOrder()
    {
        // Get frenzy num
        _frenzy.BreakFrenzyMeter();

        // Calculate tips and money
    }

}

public enum MainGameState
{
    NONE,
    CHAT,
    MAIN_GAME,
    GAME_OVER
}
