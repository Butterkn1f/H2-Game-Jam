using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]

public class IngredientButton : MonoBehaviour
{
    [SerializeField] private GameObject _ghostItemPrefab;
    [SerializeField] private RectTransform _potTransform;

    private Button _button;
    private Image _image;

    private RectTransform SpawnedItem = null;
    private Sequence _itemSequence;
    [HideInInspector] public Ingredient Ingredient = null;

    void Start()
    {
        //gameObject.SetActive(false);
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();

        //_button.enabled = false;
        _button.onClick.AddListener(delegate () { AddIngredient(); });
        //SubscribeFrenzy();
    }

    private void SubscribeFrenzy()
    {
        Frenzy.Instance.FrenzyEnabled.Value.Subscribe(enabled =>
        {
            _image.sprite = enabled ? Ingredient.FrenzySprite : Ingredient.NormalSprite;
        }).AddTo(this);
    }

    private void AnimateFlyToPot()
    {
        _itemSequence?.Kill();
        if (SpawnedItem != null)
        {
            Destroy(SpawnedItem.gameObject);
            SpawnedItem = null;
        }

        RectTransform ownTransform = GetComponent<RectTransform>();
        GameObject obj = Instantiate(_ghostItemPrefab, ownTransform.parent);
        SpawnedItem = obj.GetComponent<RectTransform>();
        SpawnedItem.localPosition = ownTransform.localPosition;
        SpawnedItem.SetParent(ownTransform.parent.parent, true);

        SpawnedItem.sizeDelta = new Vector2(ownTransform.rect.width, ownTransform.rect.height);
        obj.GetComponent<Image>().sprite = _image.sprite;

        _itemSequence = DOTween.Sequence();
        _itemSequence.Append(SpawnedItem.DOAnchorPos(_potTransform.anchoredPosition, 0.5f))
            .Join(obj.GetComponent<Image>().DOFade(0, 0.5f))
            .OnComplete(() =>
            {
                Destroy(SpawnedItem.gameObject);
                SpawnedItem = null;
            });
    }

    public void Initialize(Ingredient ingredient)
    {
        _image.sprite = ingredient.NormalSprite;
        Ingredient = ingredient;
        _button.enabled = true;
        gameObject.SetActive(true);
    }

    public void AddIngredient()
    {
        AnimateFlyToPot();
    }
}
