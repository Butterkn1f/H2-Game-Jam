using UnityEngine;
using DG.Tweening;

public class IngredientItem : ITableItem
{
    [HideInInspector] public Ingredient Ingredient = null;

    public bool IsActive = false;

    public Sequence AnimateActivate(Ingredient ingredient)
    {
        Ingredient = ingredient;
        IsActive = true;
        _image.sprite = Frenzy.Instance.FrenzyEnabled.GetValue() ?
            ingredient.FrenzySprite : ingredient.NormalSprite;

        // Grow to a lighter color, then shrink back to original color and target color
        sequence = DOTween.Sequence();
        sequence.Append(_image.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.25f))
            .Join(_image.DOColor(targetColor, 0.15f))

            .Append(_image.rectTransform.DOScale(Vector3.one, 0.25f))
            .Join(_image.DOColor(targetColor * new Vector4(0.95f, 0.95f, 0.95f, 1), 0.25f))
            .AppendInterval(0.25f);

        return sequence;
    }

    public void Initialize()
    {
        _image.color = _initialColor * new Vector4(1, 1, 1, 0);
        BeginFloatAnimation();

        // Fade in animation
        FadeSequence = DOTween.Sequence();
        FadeSequence.Append(_image.DOFade(_initialColor.a, 0.25f));
    }

    public override void ToggleFrenzySprite(bool isFrenzy)
    {
        if (IsActive)
        {
            _image.sprite = isFrenzy ? Ingredient.FrenzySprite : Ingredient.NormalSprite;
        }
    }

    public override void TransitionOut()
    {
        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.SetParent(rectTransform.parent.parent, true);
        var pos = rectTransform.localPosition;
        rectTransform.anchorMin = new Vector2(0f, 0.5f);
        rectTransform.localPosition = pos;

        FadeSequence.Kill();
        FadeSequence = DOTween.Sequence();
        FadeSequence.Append(_image.DOFade(0, 0.25f))
            .OnComplete(() =>
            {
                KillAnimation();
                Destroy(gameObject);
            });
    }
}
