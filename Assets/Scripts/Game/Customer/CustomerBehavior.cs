using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Extention;

/// <summary>
/// This class manages the behaviors (no aesthetics) 
/// </summary>
public class CustomerBehavior : MonoBehaviour
{
    // Count timer??
    // Set if we want to count timer or not
    public bool CountTimer = false;
    private bool _activeBehaviour = false;
    private float _currentTimerDuration = 0;
    private float _maxTimerDuration = 0;
    public ReactiveProp<float> PatiencePercentage = new ReactiveProp<float>();

    // Set the current Timer
    public void SetTimer(float TimerMaxDuration)
    {
        _maxTimerDuration = TimerMaxDuration;
        _currentTimerDuration = _maxTimerDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentTimerDuration > 0)
        {
            _currentTimerDuration -= Time.deltaTime;
            _activeBehaviour = true;

            // Calculate the current mood the character should be displaying
            // Actually.. we should unirx this right.
            PatiencePercentage.SetValue(_currentTimerDuration / _maxTimerDuration);
        }
        else if (_activeBehaviour == true)
        {
            _activeBehaviour = false;

            // Tell the customer manager that the current customer should leave
            CustomerManager.Instance.LeaveCurrentCustomer();

            // Tell game that the customer left
            MainGameManager.Instance.BreakOrder();
        }
    }
}
