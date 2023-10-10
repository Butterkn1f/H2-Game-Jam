using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

/// <summary>
/// This class manages all the aesthetic functions as well as the User interface for the customer.
/// To li lian: ideally, the order UI stuff should also appear here
/// </summary>
public class CustomerUI : MonoBehaviour
{
    [Serializable]
    private struct customerEmotion
    {
        public CustomerMood Mood;
        public Sprite Emoji;
    }

    [SerializeField] private List<customerEmotion> _moodEmojis;

    private bool _isActive;

    // UI elements
    [SerializeField] private GameObject _feelingBubble;
    [SerializeField] private Image _currentMoodImage;
    private CustomerMood _currentMood;

    [SerializeField] private GameObject _foodBubble;
    [SerializeField] private Image _orderImage;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CustomerBehavior>().PatiencePercentage.Value.Subscribe(patience => SetMoodImage(patience));
        _feelingBubble.GetComponent<RectTransform>().localScale = Vector3.zero;
        _foodBubble.GetComponent<RectTransform>().localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void IntroAnim()
    {
        Sequence introSequence = DOTween.Sequence();
        introSequence.Append(_feelingBubble.GetComponent<RectTransform>().DOScale(1.25f, 0.25f));
        introSequence.Join(_foodBubble.GetComponent<RectTransform>().DOScale(1.25f, 0.25f));
        introSequence.Append(_feelingBubble.GetComponent<RectTransform>().DOScale(1.0f, 0.5f));
        introSequence.Join(_foodBubble.GetComponent<RectTransform>().DOScale(1.0f, 0.5f));
        introSequence.AppendCallback(() => _isActive = true);
    }

    public void OutroAnim()
    {
        _isActive = false;

        Sequence introSequence = DOTween.Sequence();
        introSequence.Append(_feelingBubble.GetComponent<RectTransform>().DOScale(1.25f, 0.1f));
        introSequence.Join(_foodBubble.GetComponent<RectTransform>().DOScale(1.25f, 0.1f));
        introSequence.Append(_feelingBubble.GetComponent<RectTransform>().DOScale(0, 0.25f));
        introSequence.Join(_foodBubble.GetComponent<RectTransform>().DOScale(0, 0.25f));
    }

    private void SetMoodImage(float patiencePercentage)
    {
        if (!_isActive)
        {
            return;
        }

        CustomerMood _previousMood = _currentMood;

        if (patiencePercentage > 0.66)
        {
            _currentMood = CustomerMood.HAPPY;
        }
        else if (patiencePercentage > 0.33)
        {
            _currentMood = CustomerMood.NEUTRAL;
        }
        else if (patiencePercentage > 0.05)
        {
            _currentMood = CustomerMood.ANGRY;
        }
        else
        {
            _currentMood = CustomerMood.VERY_ANGRY;
        }

        if (_currentMood != _previousMood)
        {
            RectTransform _feelingBubbleRectTransform = _feelingBubble.GetComponent<RectTransform>();
            Sequence shakeSequence = DOTween.Sequence();

            // TODO: make this more impact
            shakeSequence.Append(_feelingBubbleRectTransform.DOScale(3.0f, 0.2f).SetEase(Ease.OutCubic));
            shakeSequence.Append(_feelingBubbleRectTransform.DOScale(1.0f, 0.2f).SetEase(Ease.InCubic));
        }

        _currentMoodImage.sprite = _moodEmojis.Where(x => x.Mood == _currentMood).Select(x => x.Emoji).FirstOrDefault();
        
    }
}

// The four moods the customer will have
// I don't know if this enum will even be used but I leave here just in case.
public enum CustomerMood
{
    HAPPY,
    NEUTRAL,
    ANGRY,
    VERY_ANGRY
}

