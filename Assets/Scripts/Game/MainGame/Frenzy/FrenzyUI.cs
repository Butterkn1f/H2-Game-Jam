using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using DG.Tweening;
using TMPro;

/// <summary>
/// This class is in charge of the Frenzy Mode UI // Frenzy Bar
/// </summary>
public class FrenzyUI : MonoBehaviour
{
    // Frenzy Bar Variables
    [SerializeField] private Slider _frenzyBar;

    // Frenzy Mode Variables
    [SerializeField] private GameObject _frenzyBackground;
    [SerializeField] private GameObject _frenzyText;
    [SerializeField] private GameObject _bunnyZoomImage;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Frenzy>()._frenzyPercentage.Value.Subscribe(x => UpdateFrenzyBar(x));

        _frenzyBackground.SetActive(false);
        _frenzyText.SetActive(false);
    }

    private void UpdateFrenzyBar(float newFrenzyBar)
    {
        _frenzyBar.DOValue(newFrenzyBar, 0.25f);
    }

    private void FrenzyBackgroundIntro()
    {
        RectTransform rectTransform = _frenzyBackground.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, - (_frenzyBackground.gameObject.GetComponentInParent<RectTransform>().rect.height * 2));
        _frenzyBackground.SetActive(true);
        rectTransform.DOAnchorPosY(0, 1.0f).SetEase(Ease.OutCubic);
    }

    private void FrenzyBackgroundOutro()
    {
        RectTransform rectTransform = _frenzyBackground.GetComponent<RectTransform>();
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPosY((_frenzyBackground.gameObject.GetComponentInParent<RectTransform>().rect.height * 2), 1.0f).SetEase(Ease.OutCubic));
        seq.AppendCallback(() => _frenzyBackground.SetActive(false));
    }

    private void FrenzyTextIntro()
    {
        RectTransform rectTransform = _frenzyText.GetComponent<RectTransform>();
        TextMeshProUGUI text = _frenzyText.GetComponent<TextMeshProUGUI>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -200);
        text.alpha = 0;

        _frenzyText.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPosY(0, 1.0f).SetEase(Ease.OutCubic));
        seq.Join(text.DOFade(1, 1.0f));
        seq.AppendInterval(0.5f);
        seq.Append(rectTransform.DOAnchorPosY(200, 1.0f).SetEase(Ease.OutCubic));
        seq.Join(text.DOFade(0, 1.0f));
        seq.AppendCallback(() => _frenzyText.SetActive(false));
    }

    public void StartFrenzy()
    {
        FrenzyBackgroundIntro();
        FrenzyTextIntro();
    }

    public void EndFrenzy()
    {
        FrenzyBackgroundOutro();
    }
}
