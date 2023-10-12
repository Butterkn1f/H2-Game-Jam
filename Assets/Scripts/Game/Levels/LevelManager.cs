using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class contains the overall behaviors for the level 
/// This includes setting the whole level up, and other things (undecided)
/// </summary>
public class LevelManager : Common.DesignPatterns.SingletonPersistent<LevelManager>
{
    #region level-specific variables
    [SerializeField] private List<LevelData> _levels;
    public LevelData CurrLevel => _levels[_currLevelIndex];
    #endregion

    #region level editable variables 
    private CustomerManager _customerManager;
    public int _currLevelIndex = 0;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
    }

    public int GetLastUnlockedLevelIndex()
    {
        int lastUnlocked = _levels.Count - 1;
        for (int i = 0; i < _levels.Count; ++i)
        {
            if (!_levels[i].IsUnlocked)
            {
                lastUnlocked = i - 1;
                break;
            }
        }

        return lastUnlocked;
    }

    public void SetCurrLevel(int index)
    {
        _currLevelIndex = index;
        SceneManager.LoadScene("GameScene");
    }

    public LevelData GetLevelAtIndex(int index)
    {
        return _levels[index];
    }
}
