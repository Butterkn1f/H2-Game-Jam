using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class mainMenuSettings : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _settingsStuff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSettings()
    {
        _pausePanel.GetComponent<Image>().color = Color.clear;
        _pausePanel.SetActive(true);
        _settingsStuff.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(_pausePanel.GetComponent<Image>().DOFade(0.8f, 1.0f));
        seq.SetUpdate(true);
    }

    public void HideSettings()
    {
        _pausePanel.SetActive(false);
        _settingsStuff.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.Append(_pausePanel.GetComponent<Image>().DOFade(0f, 1.0f));
        seq.SetUpdate(true);
    }
}
