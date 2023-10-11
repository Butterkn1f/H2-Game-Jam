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

    private void SubscribeFrenzy()
    {
        Frenzy.Instance.FrenzyEnabled.Value.Subscribe(enabled =>
        {
            _image.sprite = enabled ? Ingredient.FrenzySprite : Ingredient.NormalSprite;
            
            if (!enabled && MainGameManager.Instance.GameState.GetValue() == MainGameState.GAME_PREPARE)
            {
                // if stopped frenzy and is currently in preparation state, enable buttons
                _button.interactable = true;
            }
        }).AddTo(this);
    }

    private void SubscribeGameState()
    {
        MainGameManager.Instance.GameState.Value.Subscribe(state =>
        {
            switch (state)
            {
                case MainGameState.GAME_PREPARE:
                    if (!Frenzy.Instance.FrenzyEnabled.GetValue())
                        _button.interactable = true;
                    break;

                case MainGameState.GAME_COOK:
                        _button.interactable = false;
                    break;

                default:
                    break;
            }
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
        SpawnedItem.SetParent(_potTransform, true);

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
        gameObject.SetActive(false);
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();

        _button.interactable = false;
        _button.onClick.AddListener(delegate () { AddIngredient(); });
        SubscribeFrenzy();
        SubscribeGameState();

        _image.sprite = ingredient.NormalSprite;
        Ingredient = ingredient;
        gameObject.SetActive(true);
    }

    public void AddIngredient()
    {
        if (DishManager.Instance.CheckAddIngredient(Ingredient))
        {
            AnimateFlyToPot();
        }
    }
}
