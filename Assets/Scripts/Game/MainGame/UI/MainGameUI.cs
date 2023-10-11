using System.Collections;
using System.Collections.Generic;
using Common.DesignPatterns;
using UnityEngine;

using UniRx;

public class MainGameUI : Singleton<MainGameUI>
{
    // UI elements
    private ResultsUI _resultsUI;
    private ChatUI _chatUI;

    // Temporary elements
    private MainGameState _currentGameState = MainGameState.NONE;
    
    // Start is called before the first frame update
    void Start()
    {
        _resultsUI = ResultsUI.Instance;
        _chatUI = ChatUI.Instance;

        // Subscribe to game state
        MainGameManager.Instance.GameState.Value.Subscribe(x => ChangeGameState(x));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeGameState(MainGameState newGameState)
    {
        switch (_currentGameState)
        {
            case MainGameState.CHAT:
                _chatUI.OutroResult();
                break;
            case MainGameState.MAIN_GAME:
                break;
            case MainGameState.GAME_OVER:
                break;
        }

        switch (newGameState)
        {
            case MainGameState.CHAT:
                _chatUI.SetUpChat();
                break;
            case MainGameState.MAIN_GAME:
                break;
            case MainGameState.GAME_OVER:
                _resultsUI.IntroResult();
                break;
        }

        _currentGameState = newGameState;
    }
}
