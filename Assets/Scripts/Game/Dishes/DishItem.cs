using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DishItem : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _initialColor;

    private Color targetColor = Color.white;
    private Sequence sequence = null;
    private Sequence floatSequence = null;

    public Shape Shape = null;
    private void AnimateActivate()
    {
        sequence = DOTween.Sequence();
        sequence.Append(_image.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.15f))
            .Join(_image.DOColor(targetColor * new Color(1.2f, 1.2f, 1.2f), 0.25f))

            .Append(_image.rectTransform.DOScale(Vector3.one, 0.25f))
            .Join(_image.DOColor(targetColor, 0.25f));
    }

    private void BeginFloatAnimation()
    {
        var floatOffset = Random.Range()
        floatSequence = DOTween.Sequence();
        floatSequence.Append(transform.DOBlendableMoveBy(new Vector3(0, 111, 0), 1.5f))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    public void Initialize(Shape shape)
    {
        Shape = shape;
        targetColor = shape.Color;
        _image.color = _initialColor;
        _image.sprite = shape.Sprite;
    }

    public void ActivateItem()
    {
        AnimateActivate();
    }

    public void KillAnimation()
    {
        sequence.Kill();
    }
}
