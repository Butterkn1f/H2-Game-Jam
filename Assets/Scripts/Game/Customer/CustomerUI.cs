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
    /// <summary>
    /// A simple data container containing a mood emoji pair (for patience)
    /// </summary>
    [Serializable]
    private struct customerEmotion
    {
        public CustomerMood Mood;
        public Sprite Emoji;
    }

    // Total list of mood emoji pairs
    [SerializeField] private List<customerEmotion> _moodEmojis;

    private bool _isActive; // Toggles the update of the character animation
    public float IntroAnimationDuration = 0.75f; 

    // UI elements
    [SerializeField] private GameObject _feelingBubble;
    [SerializeField] private Image _patienceMeter;
    [SerializeField] private Image _currentMoodImage;
    private CustomerMood _currentMood;

    [SerializeField] private GameObject _foodBubble;
    [SerializeField] private CanvasGroup _ingredientsParent;
    [SerializeField] private List<Image> _ingredients;
    public Image OrderImage;
    private Sequence sequence;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CustomerBehavior>().PatiencePercentage.Value.Subscribe(patience => SetMoodImage(patience));
        _feelingBubble.GetComponent<RectTransform>().localScale = Vector3.zero;
        _foodBubble.GetComponent<RectTransform>().localScale = Vector3.zero;
    }

    public void OnPointerDownFoodBubble()
    {
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(OrderImage.DOFade(0, 0.25f))
            .Append(_ingredientsParent.DOFade(1, 0.25f));

        TutorialManager.Instance.AdvanceTutorial(3);

    }

    public void OnPointerUpFoodBubble()
    {
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(_ingredientsParent.DOFade(0, 0.1f))
            .Append(OrderImage.DOFade(1, 0.1f));
    }

    public void InitializeFoodBubble(Dish dish)
    {
        OrderImage.sprite = dish.Sprite;

        foreach (var ingType in dish.Ingredients)
        {
            var ingredient = LevelManager.Instance.CurrLevel.Ingredients.Find(ing => ing.Type == ingType);
            var ingUI = _ingredients.Find(ing => ing.gameObject.activeSelf == false);

            if (ingredient != null && ingUI != null)
            {
                ingUI.sprite = ingredient.NormalSprite;
                ingUI.gameObject.SetActive(true);
            }
        }
    }

    // Introduction animation
    public void IntroAnim()
    {
        Sequence introSequence = DOTween.Sequence();
        introSequence.Append(_feelingBubble.GetComponent<RectTransform>().DOScale(1.25f, 0.25f));
        introSequence.Join(_foodBubble.GetComponent<RectTransform>().DOScale(1.25f, 0.25f));
        introSequence.Append(_feelingBubble.GetComponent<RectTransform>().DOScale(1.0f, 0.5f));
        introSequence.Join(_foodBubble.GetComponent<RectTransform>().DOScale(1.0f, 0.5f));
        introSequence.AppendCallback(() => _isActive = true); // Set behaviour is true after animation starts

        // TODO: set floating animation on the bubble
    }

    // Outro animation
    public void OutroFoodAnim()
    {
        _isActive = false;

        Sequence introSequence = DOTween.Sequence();
        introSequence.Append(_foodBubble.GetComponent<RectTransform>().DOScale(1.25f, 0.1f));
        introSequence.Append(_foodBubble.GetComponent<RectTransform>().DOScale(0, 0.25f));
    }

    public void OutroEmotionAnim()
    {
        _isActive = false;

        Sequence introSequence = DOTween.Sequence();
        introSequence.Append(_feelingBubble.GetComponent<RectTransform>().DOScale(1.25f, 0.1f));
        introSequence.Append(_feelingBubble.GetComponent<RectTransform>().DOScale(0, 0.25f));
    }

    private void SetMoodImage(float patiencePercentage)
    {
        if (!_isActive)
        {
            return;
        }

        CustomerMood _previousMood = _currentMood;

        _patienceMeter.fillAmount = patiencePercentage;

        if (patiencePercentage > 0.66)
        {
            _currentMood = CustomerMood.HAPPY;
            _patienceMeter.color = new Color(0.42f,0.89f,0.24f);
        }
        else if (patiencePercentage > 0.33)
        {
            _currentMood = CustomerMood.NEUTRAL;
            _patienceMeter.color = new Color(0.95f, 0.69f, 0.26f);
        }
        else if (patiencePercentage > 0.05)
        {
            _currentMood = CustomerMood.ANGRY;
            _patienceMeter.color = new Color(0.98f, 0.2f, 0.01f);
        }
        else
        {
            _currentMood = CustomerMood.VERY_ANGRY;
            _patienceMeter.color = new Color(0.98f, 0.2f, 0.01f);
        }

        if (_currentMood != _previousMood)
        {
            RectTransform _feelingBubbleRectTransform = _feelingBubble.GetComponent<RectTransform>();
            Sequence shakeSequence = DOTween.Sequence();

            // TODO: make this more impact
            shakeSequence.Append(_feelingBubbleRectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic));
            shakeSequence.Append(_feelingBubbleRectTransform.DOScale(1.0f, 0.2f).SetEase(Ease.InCubic));
        }

        _currentMoodImage.sprite = _moodEmojis.Where(x => x.Mood == _currentMood).Select(x => x.Emoji).FirstOrDefault();
        
    }

    public void SetHeart()
    {
        _currentMood = CustomerMood.LOVE;
        _currentMoodImage.sprite = _moodEmojis.Where(x => x.Mood == _currentMood).Select(x => x.Emoji).FirstOrDefault();
        RectTransform _feelingBubbleRectTransform = _feelingBubble.GetComponent<RectTransform>();
        Sequence shakeSequence = DOTween.Sequence();
        _patienceMeter.gameObject.SetActive(false);

        // TODO: make this more impact
        shakeSequence.Append(_feelingBubbleRectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic));
        shakeSequence.Append(_feelingBubbleRectTransform.DOScale(1.0f, 0.2f).SetEase(Ease.InCubic));
    }

    public void SetAngry()
    {
        _currentMood = CustomerMood.VERY_ANGRY;
        _currentMoodImage.sprite = _moodEmojis.Where(x => x.Mood == _currentMood).Select(x => x.Emoji).FirstOrDefault();
        RectTransform _feelingBubbleRectTransform = _feelingBubble.GetComponent<RectTransform>();
        Sequence shakeSequence = DOTween.Sequence();
        _patienceMeter.gameObject.SetActive(false);

        // TODO: make this more impact
        shakeSequence.Append(_feelingBubbleRectTransform.DOScale(1.5f, 0.2f).SetEase(Ease.OutCubic));
        shakeSequence.Append(_feelingBubbleRectTransform.DOScale(1.0f, 0.2f).SetEase(Ease.InCubic));
    }
}

// The four moods the customer will have
// I don't know if this enum will even be used but I leave here just in case.
public enum CustomerMood
{
    HAPPY,
    NEUTRAL,
    ANGRY,
    VERY_ANGRY,
    LOVE
}

