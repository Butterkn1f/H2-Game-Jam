using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script SUCKS so bad it is so unoptimized and not safe but it is what it is no time
// warning u WILL vomit from reading this code

public class TutorialManager : Common.DesignPatterns.Singleton<TutorialManager>
{
    [SerializeField] List<TutorialPanel> _tutorialPanels;
    private int _currTutorialIndex = 0;
    private bool _isTutorialActive = false;

    void Start()
    {
        _isTutorialActive = LevelManager.Instance._currLevelIndex == 0;
    }

    public void BeginTutorial()
    {
        _tutorialPanels[_currTutorialIndex].FadeIn();
        MainGameManager.Instance.PauseGame(false);
        CustomerManager.Instance.PauseTimer(false);
    }

    public void AdvanceTutorial(int index)
    {
        if (!_isTutorialActive || _currTutorialIndex == index)
            return;

        _tutorialPanels[_currTutorialIndex].FadeOut();

        if (index < _tutorialPanels.Count)
        {
            _tutorialPanels[index].FadeIn();
        }
        else
        {
            MainGameManager.Instance.PauseGame(false);
            CustomerManager.Instance.PauseTimer(true);
            _isTutorialActive = false;
        }

    }
}
