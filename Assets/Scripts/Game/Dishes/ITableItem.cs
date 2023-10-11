using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class ITableItem : MonoBehaviour
{
    public Image _image;
    [SerializeField] protected Color _initialColor;

    protected Color targetColor = Color.white;
    protected Sequence sequence = null;
    protected Sequence floatSequence = null;
    protected Sequence FadeSequence = null;

    public virtual Sequence AnimateActivate()
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

    public void KillAnimation()
    {
        floatSequence.Kill();
        sequence.Kill();
        FadeSequence.Kill();
    }

    public abstract void ToggleFrenzySprite(bool isFrenzy);

    public virtual void TransitionOut()
    {
        KillAnimation();
        Destroy(gameObject);
    }
}
