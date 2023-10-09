using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
        else if (_activeBehaviour == true)
        {
            _activeBehaviour = false;
            // Add food throw away animation here

            // Tell the customer manager that the current customer should leave
            // It's so coupled im gna kms
            CustomerManager.Instance.LeaveCurrentCustomer();
        }
    }
}
