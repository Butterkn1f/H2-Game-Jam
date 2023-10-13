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
    public bool _isTutorialActive = false;

    void Start()
    {
        _isTutorialActive = LevelManager.Instance._currLevelIndex == 0;

        if (!_isTutorialActive)
            _tutorialPanels[0].gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void BeginTutorial()
    {
        if (!_isTutorialActive)
            return;

        _tutorialPanels[_currTutorialIndex].FadeIn();
        MainGameManager.Instance.PauseHardcode(true);
        CustomerManager.Instance.PauseTimer(true);
    }

    public void AdvanceTutorial(int index)
    {
        if (!_isTutorialActive || _currTutorialIndex == index)
            return;

        _tutorialPanels[_currTutorialIndex].FadeOut();

        if (index < _tutorialPanels.Count)
        {
            _tutorialPanels[index].FadeIn();
            _currTutorialIndex++;
        }
        else
        {
            MainGameManager.Instance.PauseHardcode(false);
            CustomerManager.Instance.PauseTimer(false);
            _isTutorialActive = false;
        }

    }

    public void StopTutorial()
    {
        MainGameManager.Instance.PauseHardcode(false);
        CustomerManager.Instance.PauseTimer(false);
        _tutorialPanels[_currTutorialIndex].FadeOut();
        _tutorialPanels[0].gameObject.transform.parent.gameObject.SetActive(false);
        _isTutorialActive = false;
    }
}
