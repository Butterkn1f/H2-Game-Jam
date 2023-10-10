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

    public Shape Shape = null;
    private void AnimateActivate()
    {
        sequence = DOTween.Sequence();
        sequence.Append(_image.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.15f))
            .Join(_image.DOColor(targetColor * new Color(1.2f, 1.2f, 1.2f), 0.25f))

            .Append(_image.rectTransform.DOScale(Vector3.one, 0.25f))
            .Join(_image.DOColor(targetColor, 0.25f));
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
