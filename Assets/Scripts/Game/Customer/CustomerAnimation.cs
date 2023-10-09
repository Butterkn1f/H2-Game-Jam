using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// This class controls the animation for the customer (to be added into itself)
/// </summary>
public class CustomerAnimation : MonoBehaviour
{
    private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        // Set up initial positioning
        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x + GetComponentInParent<RectTransform>().rect.width * 2, _rectTransform.anchoredPosition.y);
    }

    public void PlayIntroAnimation()
    {
        _rectTransform.DOMoveX(0, 1.0f).SetEase(Ease.OutCubic);
    }

    public void PlayOutroAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_rectTransform.DOAnchorPos(new Vector2(_rectTransform.anchoredPosition.x - GetComponentInParent<RectTransform>().rect.width * 2, _rectTransform.anchoredPosition.y), 1.0f).SetEase(Ease.InCubic));
        // Delete the object afterwards
        seq.AppendCallback(() => { DOTween.Kill(this); Destroy(this.gameObject); } );
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayOutroAnimation();
        }
    }
}
