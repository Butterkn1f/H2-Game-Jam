using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.DesignPatterns;
using UniRx.Extention;
using UnityEngine.UI;
using DG.Tweening;

public class MainGameManager : Singleton<MainGameManager>
{
    // Main Game
    public ReactiveProp<MainGameState> GameState = new ReactiveProp<MainGameState>();

    // Frenzy Manager
    private Frenzy _frenzy;

    // Customer Manager
    CustomerManager _customerManager;

    // Money Manager
    MoneyManager _moneyManager;

    // Day Timer
    DayTimer _dayTimer;

    [SerializeField] private Image _backgroundImage;

    private float currentAnimSpeed;
    public ReactiveProp<bool> PausedGame = new ReactiveProp<bool>();

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
        _moneyManager = MoneyManager.Instance;
        _moneyManager.SetData(LevelManager.Instance.CurrLevel.DishCost, LevelManager.Instance.CurrLevel.TwoStarMarker, LevelManager.Instance.CurrLevel.ThreeStarMarker);

        // Get indiv components
        _frenzy = GetComponent<Frenzy>();
        _dayTimer = GetComponent<DayTimer>();

        GameState.SetValue(MainGameState.CHAT);
        AudioManager.Instance.PlayAudio(SoundUID.CHAT_AUDIO);

        PausedGame.SetValue(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            FinishOrder();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            PauseGame(false);
        }
    }

    public bool isEOD()
    {
        return _dayTimer.IsEOD();
    }

    /// <summary>
    /// Start the main game proper (the cooking portion)
    /// </summary>
    public void StartGame()
    {
        _dayTimer.StartTimer();
        _customerManager.SendNewCustomer();
        TutorialManager.Instance.BeginTutorial();
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
        _customerManager.LeaveCurrentCustomer(true, false);

        // Calculate money
        _moneyManager.CalculateProfit();

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

        if (isEOD())
        {
            return;
        }

        // Calculate tips and money
        _moneyManager.AddMoney(_customerManager.CurrentCustomerObject.GetComponent<CustomerBehavior>().PatiencePercentage.GetValue());

        // Send the next customer in 
        _customerManager.LeaveCurrentCustomer(false);
        _customerManager.SetSuccessfulOrder();

        // Get frenzy num
        _frenzy.AddFrenzyMeter();
        AudioManager.Instance.PlayAudio(SoundUID.CORRECT_ORDER);
    }

    /// <summary>
    /// This function is called when the player misses the timing for the order 
    /// </summary>
    public void BreakOrder()
    {
        // Tell the customer manager that the current customer should leave 
        // Next customer will come in 
        CustomerManager.Instance.LeaveCurrentCustomer(false);

        // Get frenzy num
        _frenzy.BreakFrenzyMeter();

        // Calculate tips and money
        _moneyManager.ThrowAwayFood();

        AudioManager.Instance.PlayAudio(SoundUID.WRONG_ORDER);
    }

    public void PauseGame(bool pauseCharaAnimations = true)
    {
        if (PausedGame.GetValue())
        {
            PausedGame.SetValue(false);
        }
        else
        {
            PausedGame.SetValue(true);
            
        }

        _customerManager.PauseTimer(PausedGame.GetValue());
        _dayTimer.PauseTimer(PausedGame.GetValue());

        if (pauseCharaAnimations)
        {
            if (PausedGame.GetValue() == false)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
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
