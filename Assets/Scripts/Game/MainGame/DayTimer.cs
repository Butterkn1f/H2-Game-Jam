using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTimer : MonoBehaviour
{
    // Day Timer variables
    private bool _timerEnabled = false;
    [SerializeField, Range(10, 600)] private float DayDuration = 180;

    private float _timeCounter = 0;

    public void StartTimer()
    {
        _timerEnabled = true;
        _timeCounter = DayDuration;
    }

    public void EndTimer()
    {
        _timerEnabled = false;
        _timeCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timerEnabled)
        {
            if (_timeCounter > 0)
            {
                _timeCounter -= Time.deltaTime;
            }
            else
            {
                // Time is done, inform game manager to change states
                MainGameManager.Instance.EndGame();
            }
        }
    }
}
