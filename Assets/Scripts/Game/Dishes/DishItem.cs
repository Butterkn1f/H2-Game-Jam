using DG.Tweening;
using UnityEngine;

public class DishItem : ITableItem
{
    [SerializeField] private Sprite _frenzySprite;
    [SerializeField] private Sprite _rottenDish;
    [HideInInspector] public Shape Shape = null;

    public void ServeCustomerAnimation(bool isCorrect)
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

            if (isCorrect)
                MainGameManager.Instance.FinishOrder();
            else
                MainGameManager.Instance.BreakOrder();

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

                    if (isCorrect)
                        MainGameManager.Instance.FinishOrder();
                    else
                        MainGameManager.Instance.BreakOrder();
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

        // Fade in animation
        FadeSequence = DOTween.Sequence();
        FadeSequence.Append(_image.DOFade(_initialColor.a, 0.25f));
    }

    public void InitializeDish(Dish dish)
    {
        _image.rectTransform.localScale = new Vector3(2, 2, 2);
        _image.color = Color.white;
        _image.sprite = dish != null ? dish.Sprite : _rottenDish;
    }

    public override void ToggleFrenzySprite(bool isFrenzy)
    {
        _image.sprite = isFrenzy ? _frenzySprite : Shape.Sprite;
        targetColor = isFrenzy ? Color.white : Shape.Color;

        // Also update color if changing frenzy but already activated
        if (_image.color.a == 1)
        {
            _image.color = targetColor;
        }
    }
}
