using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DishItem : MonoBehaviour
{
    public Image _image;
    [SerializeField] private Color _initialColor;
    [SerializeField] private Sprite _frenzySprite;

    private Color targetColor = Color.white;
    private Sequence sequence = null;
    private Sequence floatSequence = null;

    public Shape Shape = null;

    public Sequence AnimateActivate()
    {
        // Grow to a lighter color, then shrink back to original color and target color
        sequence = DOTween.Sequence();
        sequence.Append(_image.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.25f))
            .Join(_image.DOColor(targetColor * new Color(3f, 3f, 3f), 0.15f))

            .Append(_image.rectTransform.DOScale(Vector3.one, 0.25f))
            .Join(_image.DOColor(targetColor, 0.25f));

        return sequence;
    }

    public void BeginFloatAnimation()
    {
        // Initialize with offset, then for float just float towards 0
        floatSequence = DOTween.Sequence();
        floatSequence.Append(_image.rectTransform.DOAnchorPosY(0, 1.5f))
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void ServeCustomerAnimation()
    {
        var dishSequence = DOTween.Sequence();

        if (Frenzy.Instance.FrenzyEnabled.GetValue())
        {
            // Throw food diagonally, immediately slide in new customer
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.SetParent(rectTransform.parent.parent, true);
            var pos = rectTransform.localPosition;
            rectTransform.anchorMin = new Vector2(0f, 0.5f);
            rectTransform.localPosition = pos;

            MainGameManager.Instance.FinishOrder();

            dishSequence.Append(rectTransform.DOAnchorPos(new Vector2(-200, 500), 0.8f))
                .Join(_image.DOFade(0, 0.8f).SetEase(Ease.InCirc))
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
        else
        {
            // Serve customer up, wait for serving to complete before sliding in new customer
            dishSequence.AppendInterval(0.1f);
            dishSequence.Append(_image.rectTransform.DOAnchorPosY(300, 0.5f))
                .Join(_image.DOFade(0, 0.4f))
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                    MainGameManager.Instance.FinishOrder();
                });
        }
    }

    public void Initialize(Shape shape)
    {
        Shape = shape;
        _image.color = _initialColor;
        targetColor = Frenzy.Instance.FrenzyEnabled.GetValue() ? Color.white : shape.Color;
        _image.sprite = Frenzy.Instance.FrenzyEnabled.GetValue() ? _frenzySprite : shape.Sprite;
        BeginFloatAnimation();
    }

    public void InitializeDish(Dish dish)
    {
        _image.rectTransform.localScale = new Vector3(2, 2, 2);
        _image.color = Color.white;
        _image.sprite = dish.Sprite;
    }

    public void ToggleFrenzySprite(bool isFrenzy)
    {
        _image.sprite = isFrenzy ? _frenzySprite : Shape.Sprite;
        targetColor = isFrenzy ? Color.white : Shape.Color;

        // Also update color if changing frenzy but already activated
        if (_image.color.a == 1)
        {
            _image.color = targetColor;
        }
    }

    public void KillAnimation()
    {
        floatSequence.Kill();
        sequence.Kill();
    }
}
