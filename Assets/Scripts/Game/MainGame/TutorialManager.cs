using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script SUCKS so bad it is so unoptimized and not safe but it is what it is no time
// warning u WILL vomit from reading this code

public class TutorialManager : Common.DesignPatterns.Singleton<TutorialManager>
{
    [SerializeField] List<GameObject> _tutorialPanels;
    private int _currTutorialIndex = 0;
    private bool _isTutorialActive = false;

    void Start()
    {
        _isTutorialActive = LevelManager.Instance._currLevelIndex == 0;
    }

    public void BeginTutorial()
    {
        _tutorialPanels[_currTutorialIndex].SetActive(true);
        // TODO: Integrate stopping customer patience etc here
    }

    public void AdvanceTutorial(int? index = null)
    {
        if (!index.HasValue)
        {
            index = _currTutorialIndex + 1;
        }


    }
}
