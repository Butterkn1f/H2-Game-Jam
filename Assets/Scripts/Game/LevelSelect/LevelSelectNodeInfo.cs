using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]

public class LevelSelectNodeInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMPro.TextMeshProUGUI _levelTitle;
    [SerializeField] private List<Image> _stars = new List<Image>();
    [SerializeField] private Button _playBtn;
    [SerializeField] private GameObject _infosParent;
    [SerializeField] private Sprite _starEnabledSprite;
    [SerializeField] private Sprite _starEmptySprite;
    private Truck _truck;

    private RectTransform _rectTransform;

    public Sequence sequence;

    private void Start()
    {
        gameObject.SetActive(false);
        _rectTransform = GetComponent<RectTransform>();
        _truck = GameObject.FindGameObjectWithTag("Truck").GetComponent<Truck>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_truck != null)
            _truck.IsOverInfo = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_truck != null)
            _truck.IsOverInfo = false;
    }

    private void OnDisable()
    {
        if (_truck != null)
            _truck.IsOverInfo = false;
    }

    public void Initialize(LevelData level)
    {
        _levelTitle.text = level.LevelName;
        for (int i = 0; i < _stars.Count; ++i)
        {
            _stars[i].sprite = i < level.StarsEarned ? _starEnabledSprite : _starEmptySprite;
        }

        _playBtn.onClick.AddListener(() => OnPressPlay(level.LevelNumber - 1));
    }

    public void AnimatePopup()
    {
        _rectTransform.localScale = new Vector3(0.8f, 0.5f, 1);
        _infosParent.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.5f, 1);
        _infosParent.GetComponent<CanvasGroup>().alpha = 0;

        gameObject.SetActive(true);
        sequence.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(_rectTransform.DOScale(1f, 1.5f).SetEase(Ease.OutElastic))
            .Join(_infosParent.GetComponent<CanvasGroup>().DOFade(1f, 1f))
            .Join(_infosParent.GetComponent<RectTransform>().DOScale(1f, 1f).SetEase(Ease.OutElastic));
    }

    public void OnPressPlay(int index)
    {
        LevelManager.Instance.SetCurrLevel(index);
    }
}
