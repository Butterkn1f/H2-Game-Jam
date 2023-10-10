using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DishItem : MonoBehaviour
{
    [SerializeField] public Image _image;
    [SerializeField] private Color _initialColor;

    private Color targetColor = Color.white;
    private Sequence sequence = null;
    private Sequence floatSequence = null;

    public Shape Shape = null;

    private void AnimateActivate()
    {
        // Grow to a lighter color, then shrink back to original color and target color
        sequence = DOTween.Sequence();
        sequence.Append(_image.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.25f))
            .Join(_image.DOColor(targetColor * new Color(3f, 3f, 3f), 0.15f))

            .Append(_image.rectTransform.DOScale(Vector3.one, 0.25f))
            .Join(_image.DOColor(targetColor, 0.25f));
    }

    public void BeginFloatAnimation()
    {
        // Initialize with offset, then for float just float towards 0
        floatSequence = DOTween.Sequence();
        floatSequence.Append(_image.rectTransform.DOAnchorPosY(0, 1.5f))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void Initialize(Shape shape)
    {
        Shape = shape;
        targetColor = shape.Color;
        _image.color = _initialColor;
        _image.sprite = shape.Sprite;
        BeginFloatAnimation();
    }

    public void ActivateItem()
    {
        AnimateActivate();
    }

    public void KillAnimation()
    {
        floatSequence.Kill();
        sequence.Kill();
    }
}
