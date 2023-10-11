using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MainGameAnimation : MonoBehaviour
{
    // Variables
    [SerializeField] private GameObject _shutter;
    [SerializeField] private GameObject _roof;
    [SerializeField] private GameObject _dayOverText;
    [SerializeField] private GameObject _dayStartText;

    private float _defaultShutterYPos;
    private float _defaultTextStartPos;
    private float _defaultRoofingYPos;

    public float IntroSeqDuration = 4.0f;
    public float OutroSeqDuration = 4.5f;

    // Start is called before the first frame update
    void Start()
    {
        _defaultShutterYPos = _shutter.GetComponent<RectTransform>().anchoredPosition.y;
        _defaultRoofingYPos = _roof.GetComponent<RectTransform>().anchoredPosition.y;
        _defaultTextStartPos = _dayStartText.GetComponent<RectTransform>().anchoredPosition.y;

        _dayOverText.SetActive(false);
        _dayStartText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _roof.SetActive(true);
        _shutter.SetActive(true);
    }

    // Intro Sequence
    public void TruckIntroSequence(float delay = 0)
    {
        RectTransform rectTransform = _dayStartText.GetComponent<RectTransform>();
        TextMeshProUGUI text = _dayStartText.GetComponent<TextMeshProUGUI>();
        _dayStartText.GetComponent<RectTransform>().anchoredPosition = new Vector2(_dayStartText.GetComponent<RectTransform>().anchoredPosition.x, _defaultTextStartPos - 200);
        _dayStartText.GetComponent<TextMeshProUGUI>().alpha = 0;

        _dayStartText.SetActive(true);
        _roof.SetActive(true);
        _shutter.SetActive(true);

        Sequence introSeq = DOTween.Sequence();
        introSeq.PrependInterval(delay);
        introSeq.Append(_shutter.GetComponent<RectTransform>().DOAnchorPosY(400, 1.0f));
        introSeq.Append(_roof.GetComponent<RectTransform>().DOAnchorPosY(-30, 1.0f));

        introSeq.Append(_dayStartText.GetComponent<RectTransform>().DOAnchorPosY(_defaultTextStartPos, 1.0f).SetEase(Ease.OutCubic));
        introSeq.Join(_dayStartText.GetComponent<TextMeshProUGUI>().DOFade(1, 1.0f));
        introSeq.AppendInterval(0.5f);
        introSeq.Append(_dayStartText.GetComponent<RectTransform>().DOAnchorPosY(_defaultTextStartPos + 200, 1.0f).SetEase(Ease.OutCubic));
        introSeq.Join(_dayStartText.GetComponent<TextMeshProUGUI>().DOFade(0, 1.0f));
        introSeq.AppendCallback(() => _dayStartText.SetActive(false));
        introSeq.AppendCallback(() => MainGameManager.Instance.StartGame());
    }

    // Outro Sequence
    public void TruckOutroSequence(float delay = 0)
    {
        _dayOverText.GetComponent<RectTransform>().anchoredPosition = new Vector2(_dayOverText.GetComponent<RectTransform>().anchoredPosition.x, _defaultTextStartPos - 200);
        _dayOverText.GetComponent<TextMeshProUGUI>().alpha = 0;

        _dayOverText.SetActive(true);
        _roof.SetActive(true);
        _shutter.SetActive(true);

        Sequence outroSeq = DOTween.Sequence();
        outroSeq.PrependInterval(delay);
        
        outroSeq.Append(_dayOverText.GetComponent<RectTransform>().DOAnchorPosY(_defaultTextStartPos, 1.0f).SetEase(Ease.OutCubic));
        outroSeq.Join(_dayOverText.GetComponent<TextMeshProUGUI>().DOFade(1, 1.0f));
        outroSeq.AppendInterval(0.5f);
        outroSeq.Append(_dayOverText.GetComponent<RectTransform>().DOAnchorPosY(_defaultTextStartPos + 200, 1.0f).SetEase(Ease.OutCubic));
        outroSeq.Join(_dayOverText.GetComponent<TextMeshProUGUI>().DOFade(0, 1.0f));
        outroSeq.AppendCallback(() => _dayStartText.SetActive(false));

        outroSeq.Append(_roof.GetComponent<RectTransform>().DOAnchorPosY(_defaultRoofingYPos, 1.0f));
        outroSeq.Append(_shutter.GetComponent<RectTransform>().DOAnchorPosY(_defaultShutterYPos, 1.0f));

    }
}
