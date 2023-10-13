using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof (CanvasGroup))]
public class TutorialPanel : MonoBehaviour
{
    private CanvasGroup _overlayCanvas;

    void Start()
    {
        _overlayCanvas = GetComponent<CanvasGroup>();
        _overlayCanvas.alpha = 0;
        gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        _overlayCanvas.DOFade(1, 0.5f);
    }

    public void FadeOut()
    {
        _overlayCanvas.DOFade(0, 0.5f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
