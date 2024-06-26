using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Extention;

/// <summary>
/// This class manages the main behaviors of the customers (no aesthetics) 
/// Currently it is mainly just for counting the patience, whether we want to add order behaviours in here is a separate thing
/// </summary>
public class CustomerBehavior : MonoBehaviour
{
    // Whether the customer currently has an active behaviour
    private bool _activeBehaviour = false;
    private bool _hasActivatedBehaviour = false;

    #region Customer Patience
    private float _currentTimerDuration = 0; // Timer counter
    private float _maxTimerDuration = 0; // Maximum Timer Duration
    
    // Reactive property showing the patience of the customer as a fraction
    // UI module subscribes to this and it becomes more loosely coupled
    public ReactiveProp<float> PatiencePercentage = new ReactiveProp<float>();
    #endregion

    /// <summary>
    /// This timer sets the timer information
    /// </summary>
    /// <param name="TimerMaxDuration">The time to count down from</param>
    public void SetTimer(float TimerMaxDuration)
    {
        _maxTimerDuration = TimerMaxDuration;
        _currentTimerDuration = _maxTimerDuration;

        if (!TutorialManager.Instance._isTutorialActive)
        {
            _activeBehaviour = true;
        }
        _hasActivatedBehaviour = true;
    }

    public void PauseTimer(bool IsPaused)
    {
        _activeBehaviour = !IsPaused;
        Debug.Log("Active behaviour: " + _activeBehaviour);
    }


    // Update is called once per frame
    void Update()
    {
        if (_activeBehaviour)
        {
            // Simple timer
            if (_currentTimerDuration > 0)
            {
                _currentTimerDuration -= Time.deltaTime;

                // Calculate the current mood the character should be displaying
                PatiencePercentage.SetValue(_currentTimerDuration / _maxTimerDuration);
            }
            // Create a switch
            else
            {
                if (!_hasActivatedBehaviour)
                {
                    return;
                }

                // Deactivate the update loop 
                _activeBehaviour = false;

                if (MainGameManager.Instance.isEOD())
                {
                    return;
                }

                // Tell game that the customer left due to lack of patience
                MainGameManager.Instance.BreakOrder();
            }
        }
    }
}
