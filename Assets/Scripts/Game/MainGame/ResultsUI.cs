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

    public void IntroResult()
    {
        float opacity = _background.GetComponent<Image>().color.a;
        _background.GetComponent<Image>().color = Color.clear;
        _background.SetActive(true);
        _background.GetComponent<Image>().DOFade(opacity, 1.0f);
    }
}
