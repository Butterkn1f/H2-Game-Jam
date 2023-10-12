using System.Collections;
using System.Collections.Generic;
using Common.DesignPatterns;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : Singleton<ResultsUI>
{
    // Results screen gameobject variables
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _paper;

    // Result screen variables


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

        _paper.SetActive(false);
        _paper.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, _background.GetComponent<RectTransform>().rect.height * 2);
        _paper.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);

        float opacity = _background.GetComponent<Image>().color.a;
        _background.GetComponent<Image>().color = Color.clear;
        _paper.SetActive(true);
        _background.SetActive(true);

        seq.Append(_background.GetComponent<Image>().DOFade(opacity, 1.0f));
        seq.Append(_paper.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-30, 200), 1.5f));
        seq.Join(_paper.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 10), 1.75f));

        // Display all results

        // Display stars

        // Bun Bun jump up!

        // Buttons
    }
}
