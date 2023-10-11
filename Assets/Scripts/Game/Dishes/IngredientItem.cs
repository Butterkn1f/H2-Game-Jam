using UnityEngine;
using DG.Tweening;

public class IngredientItem : ITableItem
{
    [HideInInspector] public Ingredient Ingredient = null;

    public override Sequence AnimateActivate()
    {
        // Grow to a lighter color, then shrink back to original color and target color
        sequence = DOTween.Sequence();
        sequence.Append(_image.rectTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.25f))
            .Join(_image.DOColor(targetColor, 0.15f))

            .Append(_image.rectTransform.DOScale(Vector3.one, 0.25f))
            .Join(_image.DOColor(targetColor * 0.9f, 0.25f));

        return sequence;
    }

    public void Initialize(Ingredient ingredient)
    {
        Ingredient = ingredient;
        _image.color = _initialColor;
        _image.sprite = Frenzy.Instance.FrenzyEnabled.GetValue() ? 
            ingredient.FrenzySprite : ingredient.NormalSprite;
        BeginFloatAnimation();
    }

    public override void ToggleFrenzySprite(bool isFrenzy)
    {
        _image.sprite = isFrenzy ? Ingredient.FrenzySprite : Ingredient.NormalSprite;
    }
}
