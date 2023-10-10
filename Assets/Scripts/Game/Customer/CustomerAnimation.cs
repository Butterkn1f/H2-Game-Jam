using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// This class controls the animations for the customer
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class CustomerAnimation : MonoBehaviour
{
    private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x + GetComponentInParent<RectTransform>().rect.width * 2, _rectTransform.anchoredPosition.y);
    }

    /// <summary>
    /// This function plays the introduction animation for the character (sliding in)
    /// </summary>
    public void PlayIntroAnimation()
    {
        _rectTransform = GetComponent<RectTransform>();

        // Set up initial positioning (just in case)
        _rectTransform.anchoredPosition = new Vector2(GetComponentInParent<RectTransform>().rect.width * 2, _rectTransform.anchoredPosition.y);

        // Slide to the left
        _rectTransform.DOMoveX(0, 1.0f).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// This function plays the outro animation for the character (sliding out)
    /// </summary>
    public void PlayOutroAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_rectTransform.DOAnchorPos(new Vector2(_rectTransform.anchoredPosition.x - GetComponentInParent<RectTransform>().rect.width * 2, _rectTransform.anchoredPosition.y), 1.0f).SetEase(Ease.InCubic));

        // Deactivate the gameobject
        seq.AppendCallback(() => { this.gameObject.SetActive(false); });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => { Destroy(this.gameObject); } );
    }
}
