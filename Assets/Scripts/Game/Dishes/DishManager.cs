using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class DishManager : Common.DesignPatterns.Singleton<DishManager>
{
    #region Customizable Variables
    [SerializeField] private GameObject _dishItemPrefab;
    [SerializeField] private RectTransform _tableParentTransform;
    public List<Ingredient> Ingredients = new List<Ingredient>();
    public List<Dish> Dishes = new List<Dish>();
    #endregion

    private Dish _currDish = null;
    private int _currItemIndex = -1;
    private List<ITableItem> _items = new List<ITableItem>(); // TODO: Object pool this

    private void Start()
    {
        Frenzy.Instance.FrenzyEnabled.Value.Subscribe(enabled =>
        {
            foreach (var item in _items)
            {
                item.ToggleFrenzySprite(enabled);
            }

        }).AddTo(this);
    }

    private void InitializeDishItems()
    {
        // Add all allowed shapes first,
        // Then add random number of excess
        int itemQty = Random.Range(_currDish.MinShapes, _currDish.MaxShapes + 1);

        List<Shape> starterShapes = new List<Shape>();
        foreach(var shape in _currDish.AllowedShapes)
        {
            if (starterShapes.Count >= itemQty)
                break;

            starterShapes.Add(DrawManager.Instance.GetShapeFromType(shape));
        }

        List<Shape> shapes = new List<Shape>();
        for (int i = 0; i < (itemQty - starterShapes.Count); ++i)
        {
            shapes.Add(starterShapes[Random.Range(0, starterShapes.Count)]);
        }

        shapes.AddRange(starterShapes);
        // Shuffle shapes using fisher�yates algorithm
        for (int i = 0; i < shapes.Count; i++)
        {
            // Pick random Element
            int j = Random.Range(i, shapes.Count);

            // Swap Elements
            (shapes[j], shapes[i]) = (shapes[i], shapes[j]);
        }

        // Create dish items based on shapes
        for(int i = 0; i < shapes.Count; ++i)
        {
            var sign = (i % 2 == 0) ? 1 : -1;
            var floatOffset = Random.Range(10f, 30f) * sign;

            GameObject item = Instantiate(_dishItemPrefab, _tableParentTransform);
            DishItem dishItem = item.GetComponent<DishItem>();

            dishItem._image.rectTransform.anchoredPosition = new Vector2(0, floatOffset);
            dishItem.Initialize(shapes[i]);
            _items.Add(dishItem);
        }
    }

    public void ResetDish()
    {
        _tableParentTransform.gameObject.GetComponent<HorizontalLayoutGroup>().enabled = true;
        _currDish = null;
        _currItemIndex = -1;
    }

    private void MergeShapes()
    {
        _tableParentTransform.gameObject.GetComponent<HorizontalLayoutGroup>().enabled = false;

        var mergeSequence = DOTween.Sequence();
        if (Frenzy.Instance.FrenzyEnabled.GetValue())
            mergeSequence.timeScale = 2;

        foreach (RectTransform child in _tableParentTransform)
        {
            var pos = child.localPosition;
            child.anchorMax = child.anchorMin = new Vector2(0.5f, 0.5f);
            child.localPosition = pos;
            mergeSequence.Join(child.DOAnchorPosX(0, 0.5f));
        }

        mergeSequence.OnComplete(() =>
        {
            // TODO: poof!

            // Destroy dishes behind poof
            foreach (DishItem item in _items)
            {
                item.KillAnimation();
            }
            foreach (Transform child in _tableParentTransform)
            {
                Destroy(child.gameObject);
            }
            _items.Clear();

            // Instantiate current dish behind poof so that when poof disappears dish is there
            // TODO: Object pool this too
            GameObject dishObject = Instantiate(_dishItemPrefab, _tableParentTransform);
            DishItem dishItem = dishObject.GetComponent<DishItem>();
            dishItem.InitializeDish(_currDish);

            dishItem.ServeCustomerAnimation();
        });
    }

    public void RandomizeNewDish()
    {
        ResetDish();
        _currDish = Dishes[Random.Range(0, Dishes.Count)];
        CustomerManager.Instance.CurrentCustomerObject.GetComponent<CustomerUI>().OrderImage.sprite = _currDish.Sprite;

        InitializeDishItems();
        _currItemIndex = 0;
    }

    public void CheckDrawnShape(Shape shape)
    {
        if (shape == null && !Frenzy.Instance.FrenzyEnabled.GetValue())
            return;

        if (_currItemIndex >= 0 && _currItemIndex < _items.Count)
        {
            DishItem currItem = (DishItem)_items[_currItemIndex];
            if (shape == currItem.Shape || Frenzy.Instance.FrenzyEnabled.GetValue())
            {
                currItem.AnimateActivate();

                _currItemIndex++;
                if (_currItemIndex >= _items.Count)
                {
                    MergeShapes();
                }
            }
        }
    }
}
