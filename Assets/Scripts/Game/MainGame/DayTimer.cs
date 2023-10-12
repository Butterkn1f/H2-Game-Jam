using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTimer : MonoBehaviour
{
    // Simple timer
    // Day Timer variables
    private bool _timerEnabled = false;
    [SerializeField, Range(10, 600)] private float DayDuration = 180;

    private float _timeCounter = 0;

    // clock
    [SerializeField] private GameObject _clockHand;

    public void StartTimer()
    {
        _timerEnabled = true;
        _timeCounter = DayDuration;
        _clockHand.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 90);
        _clockHand.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -90), DayDuration).SetEase(Ease.Linear);
    }

    public void EndTimer()
    {
        _timerEnabled = false;
        _timeCounter = 0;
    }

    public bool IsEOD()
    {
        return (_timeCounter <= 0);
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
