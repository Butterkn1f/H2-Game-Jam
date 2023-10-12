using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.DesignPatterns;
using UniRx.Extention;
using UnityEngine.UI;

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

    [SerializeField] private Image _backgroundImage;

    // Start is called before the first frame update
    void Start()
    {
        // Set background image
        // Should be in a different class
        _backgroundImage.sprite = LevelManager.Instance.CurrLevel.BackgroundImage;

        // Send the first customer
        _customerManager = CustomerManager.Instance;
        _customerManager.SetCustomerList(LevelManager.Instance.CurrLevel.LevelLocation.CustomerList);
        DishManager.Instance.InitializeIngredientButtons();
        ChatGetter.Instance.StartChat(LevelManager.Instance.CurrLevel.ChatID);

        // Get indiv components
        _frenzy = GetComponent<Frenzy>();
        _dayTimer = GetComponent<DayTimer>();

        GameState.SetValue(MainGameState.CHAT);
        AudioManager.Instance.PlayAudio(SoundUID.CHAT_AUDIO);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Huh");

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

    /// <summary>
    /// Trigger all the animations to be played 
    /// </summary>
    public void StartGameAnimation()
    {
        GameState.SetValue(MainGameState.GAME_START);
        AudioManager.Instance.PlayAudio(SoundUID.MAIN_GAME_AUDIO);

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
        // Todo: Change this to only run serve state
        if (GameState.GetValue() == MainGameState.NONE || GameState.GetValue() == MainGameState.CHAT || GameState.GetValue() == MainGameState.GAME_WAIT || GameState.GetValue() == MainGameState.GAME_OVER)
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
    GAME_START,
    GAME_PREPARE,
    GAME_COOK,
    GAME_SERVE, // Not used currently
    GAME_WAIT,
    GAME_OVER,
    DEBUG
}
