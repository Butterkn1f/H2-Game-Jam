using System.Collections;
using System.Collections.Generic;
using Common.DesignPatterns;
using UnityEngine;

using UniRx;

public class MainGameUI : Singleton<MainGameUI>
{
    // UI elements
    [SerializeField] private ResultsUI _resultsUI;
    [SerializeField] private MainGameAnimation _gameAnimation;
    [SerializeField] private ChatUI _chatUI;

    // Temporary elements
    private MainGameState _currentGameState = MainGameState.NONE;
    
    // Start is called before the first frame update
    void Start()
    {
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
            case MainGameState.GAME_START:
                // TO Timothy: Any other way of doing this?
                //_gameAnimation.TruckOutroSequence(1.0f);
                break;
            case MainGameState.GAME_OVER:
                break;
            default:
                break;
        }

        switch (newGameState)
        {
            case MainGameState.CHAT:
                _chatUI.SetUpChat();
                break;
            case MainGameState.GAME_START:
                _gameAnimation.TruckIntroSequence(1.0f);
                break;
            case MainGameState.GAME_OVER:
                _gameAnimation.TruckOutroSequence(1.0f);
                _resultsUI.IntroResult(_gameAnimation.OutroSeqDuration);
                break;
            default:
                break;
        }

        _currentGameState = newGameState;
    }
}
