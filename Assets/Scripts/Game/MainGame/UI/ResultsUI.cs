using System.Collections;
using System.Collections.Generic;
using Common.DesignPatterns;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : Singleton<ResultsUI>
{
    // Results screen gameobject variables
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _paper;

    // Result screen variables
    [SerializeField] private TextMeshProUGUI _earnedAmount;
    [SerializeField] private TextMeshProUGUI _tips;
    [SerializeField] private TextMeshProUGUI _wasted;
    [SerializeField] private TextMeshProUGUI _profit;

    // GameObjects
    [SerializeField] private GameObject _earnedAmountObj;
    [SerializeField] private GameObject _tipsObj;
    [SerializeField] private GameObject _wastedObj;
    [SerializeField] private GameObject _profitObj;
    [SerializeField] private GameObject _starsObj;

    [SerializeField] private GameObject _star1;
    [SerializeField] private GameObject _star2;
    [SerializeField] private GameObject _star3;

    [SerializeField] private GameObject _toast;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IntroResult(float delay = 0)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(delay);

        _earnedAmountObj.SetActive(false);
        _tipsObj.SetActive(false);
        _wastedObj.SetActive(false);
        _profitObj.SetActive(false);

        _starsObj.SetActive(false);

        _star1.SetActive(false);
        _star2.SetActive(false);
        _star3.SetActive(false);

        _toast.SetActive(false);

        _paper.SetActive(false);
        _paper.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, _background.GetComponent<RectTransform>().rect.height * 2);
        _paper.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);

        float opacity = _background.GetComponent<Image>().color.a;
        _background.GetComponent<Image>().color = Color.clear;
        _paper.SetActive(true);
        _background.SetActive(true);

        seq.Append(_background.GetComponent<Image>().DOFade(opacity, 1.0f));
        seq.Append(_paper.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-30, 200), 1.5f));
        seq.Join(_paper.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 5), 1.75f));

        // Set Results
        _earnedAmount.text = "$ " + MoneyManager.Instance.AmountEarned.ToString("0.00");
        _tips.text = "$ " + MoneyManager.Instance.Tips.ToString("0.00");
        _wasted.text = "- $ " + MoneyManager.Instance.WasteCost.ToString("0.00");
        _profit.text = "$ " + MoneyManager.Instance.Profit.ToString("0.00");

        // Display all results
        _earnedAmountObj.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        _tipsObj.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        _wastedObj.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        _profitObj.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        _starsObj.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);

        seq.AppendCallback(() => _earnedAmountObj.SetActive(true));
        seq.Append(_earnedAmountObj.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f));
        seq.AppendCallback(() => _tipsObj.SetActive(true));
        seq.Append(_tipsObj.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f));
        seq.AppendCallback(() => _wastedObj.SetActive(true));
        seq.Append(_wastedObj.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f));
        seq.AppendCallback(() => _profitObj.SetActive(true));
        seq.Append(_profitObj.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f));
        seq.AppendCallback(() => _starsObj.SetActive(true));
        seq.Append(_starsObj.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f));

        // Display stars
        int numStars = MoneyManager.Instance.NumStarsEarned;
        Debug.Log(numStars);
        _star1.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        _star2.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);
        _star3.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 1);

        if (numStars >= 1)
        {
            seq.AppendCallback(() => _star1.SetActive(true));
            seq.Append(_star1.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f));
        }
        if (numStars >= 2)
        {
            seq.AppendCallback(() => _star2.SetActive(true));
            seq.Append(_star2.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f));
        }
        if (numStars >= 3)
        {
            seq.AppendCallback(() => _star3.SetActive(true));
            seq.Append(_star3.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f));
        }

        // Bun Bun jump up!
        _toast.GetComponent<RectTransform>().anchoredPosition = new Vector2(_toast.GetComponent<RectTransform>().anchoredPosition.x, -550 - 500);
        _toast.GetComponent<Image>().color = new Color (1,1,1, 0);

        _toast.SetActive(true);

        seq.Append(_toast.GetComponent<RectTransform>().DOAnchorPosY(-550, 0.5f));
        seq.Join(_toast.GetComponent<Image>().DOFade(1, 0.5f));

        switch (numStars)
        {
            case 0:
                _toast.GetComponent<Animator>().CrossFade("Toast_sad", 0, 0);
                break;
            case 1:
            case 2:
                _toast.GetComponent<Animator>().CrossFade("Toast_Ok", 0, 0);
                break;
            case 3:
                _toast.GetComponent<Animator>().CrossFade("Toast_Happy", 0, 0);
                break;
            default:
                break;
        }

        // Buttons
    }
}
