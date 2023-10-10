using System.Collections;
using System.Collections.Generic;
using UniRx.Extention;
using UnityEngine;

public class Frenzy : MonoBehaviour
{
    // Normal frenzy variables
    public ReactiveProp<float> _frenzyPercentage = new ReactiveProp<float>(); // How close the player is to getting a frenzy in percentage
    [SerializeField, Range(0, 2)] private float _frenzyBarDecreaseRate = 1; // How fast / slow the frenzy bar decreases

    [SerializeField, Range(1, 20)] private float _frenzyDuration = 10; // How long the frenzy mode duration is

    // Frenzy mode variables
    private bool _frenzyEnabled = false;
    private float _frenzyModeTimer;

    // Start is called before the first frame update
    void Start()
    {
        _frenzyPercentage.SetValue(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_frenzyEnabled)
        {
            if (_frenzyPercentage.GetValue() > 0)
            {
                // Slowly decrease the frenzy decrease rate
                _frenzyPercentage.SetValue(_frenzyPercentage.GetValue() - _frenzyBarDecreaseRate * Time.deltaTime);
            }
        }
        else
        {
            if (_frenzyModeTimer > 0)
            {
                _frenzyModeTimer -= Time.deltaTime;
                _frenzyPercentage.SetValue(_frenzyModeTimer / _frenzyDuration);
            }
            else
            {
                // End the frenzy mode
                _frenzyEnabled = false;
                _frenzyModeTimer = 0;
                _frenzyPercentage.SetValue(0);
                GetComponent<FrenzyUI>().EndFrenzy();

            }
        }
    }

    

    public void AddFrenzyMeter(float amountToAdd = 0.25f)
    {
        if (_frenzyEnabled)
        {
            // Don't add when the bar is already full 
            return;
        }

        _frenzyPercentage.SetValue(_frenzyPercentage.GetValue() + 0.25f);
        
        if (_frenzyPercentage.GetValue() >= 1)
        {
            _frenzyEnabled = true;
            GetComponent<FrenzyUI>().StartFrenzy();
            _frenzyModeTimer = _frenzyDuration;
        }
    }

    public void BreakFrenzy()
    {
        if (_frenzyEnabled)
        {
            // End the frenzy mode
            _frenzyEnabled = false;
            _frenzyModeTimer = 0;
            _frenzyPercentage.SetValue(0);
            GetComponent<FrenzyUI>().EndFrenzy();
        }

    }

    public void BreakFrenzyMeter()
    {
        _frenzyPercentage.SetValue(0);
    }
}
