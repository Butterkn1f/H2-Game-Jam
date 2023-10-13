using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using DG.Tweening;
using TMPro;
using UnityEditor;

/// <summary>
/// This class is in charge of the Frenzy Mode UI // Frenzy Bar
/// </summary>
public class FrenzyUI : MonoBehaviour
{
    // Frenzy Bar Variables
    [SerializeField] private Slider _frenzyBar;

    // Frenzy Mode Variables
    [SerializeField] private GameObject _frenzyBackground;
    [SerializeField] private GameObject _frenzyMoneyDrop;
    [SerializeField] private GameObject _frenzyText;
    [SerializeField] private GameObject _bunnyZoomImage; // The bunny image to pop up

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Frenzy>()._frenzyPercentage.Value.Subscribe(x => UpdateFrenzyBar(x));

        _frenzyBackground.SetActive(false);
        _frenzyMoneyDrop.SetActive(false);
        _frenzyText.SetActive(false);
        _bunnyZoomImage.SetActive(false);
    }

    private void UpdateFrenzyBar(float newFrenzyBar)
    {
        _frenzyBar.DOValue(newFrenzyBar, 0.25f);
    }

    private void FrenzyBackgroundIntro(float _frenzyDuration)
    {
        RectTransform moneyRectTransform = _frenzyMoneyDrop.GetComponent<RectTransform>();
        RectTransform bgRectTransform = _frenzyBackground.GetComponent<RectTransform>();
        moneyRectTransform.anchoredPosition = new Vector2(moneyRectTransform.anchoredPosition.x, (_frenzyBackground.gameObject.GetComponentInParent<RectTransform>().rect.height * 2));
        bgRectTransform.anchoredPosition = new Vector2(bgRectTransform.anchoredPosition.x,  (_frenzyBackground.gameObject.GetComponentInParent<RectTransform>().rect.height));
        _frenzyMoneyDrop.SetActive(true);
        _frenzyBackground.SetActive(true);
        moneyRectTransform.DOAnchorPosY(-(_frenzyBackground.gameObject.GetComponentInParent<RectTransform>().rect.height), _frenzyDuration + 10).SetEase(Ease.Linear);

        Sequence seq = DOTween.Sequence();
        seq.Append(bgRectTransform.DOAnchorPosY(500, 0.5f).SetEase(Ease.OutCubic));
        seq.Append(bgRectTransform.DOAnchorPosY(-450, _frenzyDuration + 10).SetEase(Ease.Linear));

    }

    private void FrenzyBackgroundOutro()
    {
        DOTween.Kill(_frenzyMoneyDrop);
        RectTransform rectTransform = _frenzyBackground.GetComponent<RectTransform>();
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPosY(- (_frenzyBackground.gameObject.GetComponentInParent<RectTransform>().rect.height * 2), 1.0f).SetEase(Ease.OutCubic));
        seq.Join(_frenzyMoneyDrop.GetComponent<RectTransform>().DOAnchorPosY(-(_frenzyBackground.gameObject.GetComponentInParent<RectTransform>().rect.height * 2),  1).SetEase(Ease.OutCubic));
        seq.AppendCallback(() => _frenzyBackground.SetActive(false));
        seq.AppendCallback(() => _frenzyMoneyDrop.SetActive(false));

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

    private void BunnyIntro()
    {
        RectTransform rectTransform = _bunnyZoomImage.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,  - (_bunnyZoomImage.gameObject.GetComponentInParent<RectTransform>().rect.height * 2));

        _bunnyZoomImage.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPosY(350, 1.0f).SetEase(Ease.OutCubic));
        seq.AppendCallback(() => _bunnyZoomImage.GetComponent<Animator>().SetTrigger("Frenzy"));
        seq.AppendCallback(() => _bunnyZoomImage.GetComponent<Animator>().ResetTrigger("Frenzy"));
        seq.AppendInterval(1.0f);
        seq.Append(rectTransform.DOAnchorPosY(-(_bunnyZoomImage.gameObject.GetComponentInParent<RectTransform>().rect.height * 2), 1.0f).SetEase(Ease.OutCubic));
        seq.AppendCallback(() => _bunnyZoomImage.SetActive(false));
    }

    public void StartFrenzy(float frenzyDuration)
    {
        FrenzyBackgroundIntro(frenzyDuration);
        FrenzyTextIntro();
        BunnyIntro();
    }

    public void EndFrenzy()
    {
        FrenzyBackgroundOutro();
    }
}
