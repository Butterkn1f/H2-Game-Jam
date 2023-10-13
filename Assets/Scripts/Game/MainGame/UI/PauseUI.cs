using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject _bell;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _pauseStuff;
    [SerializeField] private GameObject _settingsStuff;

    

    // Start is called before the first frame update
    void Start()
    {
        _pausePanel.SetActive(false);
        _pauseStuff.SetActive(false);
        _settingsStuff.SetActive(false);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBellAnim()
    {
        AudioManager.Instance.PlayAudio(SoundUID.BELL);
        Sequence seq = DOTween.Sequence();
        seq.Append(_bell.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 30), 0.25f).SetEase(Ease.OutCubic));
        seq.Append(_bell.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -30), 0.25f).SetEase(Ease.OutCubic));
        seq.Append(_bell.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.OutCubic));
    }

    public void IntroPausePanel()
    {
        if (TutorialManager.Instance._isTutorialActive)
        {
            return;
        }

        _pausePanel.GetComponent<Image>().color = Color.clear;
        _pausePanel.SetActive(true);
        _pauseStuff.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(_pausePanel.GetComponent<Image>().DOFade(0.8f, 1.0f));
        seq.SetUpdate(true);
    }

    public void OutroPausePanel()
    {
        _pausePanel.SetActive(false);
        _pauseStuff.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.Append(_pausePanel.GetComponent<Image>().DOFade(0f, 1.0f));
        seq.SetUpdate(true);
    }

    public void SetSettings()
    {
        _settingsStuff.SetActive(true);
        _pauseStuff.SetActive(false);
    }

    public void SetPause()
    {
        _settingsStuff.SetActive(false);
        _pauseStuff.SetActive(true);
    }

    
}
