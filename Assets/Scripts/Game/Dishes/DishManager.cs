using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System.Linq;

public class DishManager : Common.DesignPatterns.Singleton<DishManager>
{
    #region Customizable Variables
    [SerializeField] private GameObject _dishItemPrefab;
    [SerializeField] private GameObject _ingredientItemPrefab;
    [SerializeField] private RectTransform _tableParentTransform;
    [SerializeField] private List<IngredientButton> _ingredientButtons;
    #endregion

    private Dish _currDish = null;
    private int _currItemIndex = -1;
    private List<ITableItem> _items = new List<ITableItem>(); // TODO: Object pool this

    private LevelData _currLevel;

    private void Start()
    {
        _currLevel = LevelManager.Instance.CurrLevel;

        SubscribeGameState();
        Frenzy.Instance.FrenzyEnabled.Value.Subscribe(enabled =>
        {
            foreach (var item in _items)
            {
                item.ToggleFrenzySprite(enabled);
            }

        }).AddTo(this);
    }

    private void SubscribeGameState()
    {
        MainGameManager.Instance.GameState.Value.Subscribe(state =>
        {
            switch(state)
            {
                case MainGameState.GAME_PREPARE:
                    RandomizeNewDish();
                    break;

                case MainGameState.GAME_COOK:
                    InitializeDishItems();
                    break;

                case MainGameState.GAME_WAIT:
                    ResetDish();
                    break;

                default:
                    break;
            }
        }).AddTo(this);
    }

    private void InitializeIngredients()
    {
        for(int i = 0; i < _currDish.Ingredients.Count; ++i)
        {
            var ingredient = _currLevel.Ingredients.Find(ing => ing.Type == _currDish.Ingredients[i]);
            var sign = (i % 2 == 0) ? 1 : -1;
            var floatOffset = Random.Range(10f, 30f) * sign;

            GameObject item = Instantiate(_ingredientItemPrefab, _tableParentTransform);
            IngredientItem ingItem = item.GetComponent<IngredientItem>();

            ingItem._image.rectTransform.anchoredPosition = new Vector2(0, floatOffset);
            ingItem.Initialize(ingredient);
            _items.Add(ingItem);
        }
    }

    private void InitializeDishItems()
    {
        ClearTableItems();
        _currItemIndex = 0;

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
        ClearTableItems();

        _tableParentTransform.gameObject.GetComponent<HorizontalLayoutGroup>().enabled = true;
        _currDish = null;
        _currItemIndex = -1;
    }

    private void ClearTableItems()
    {
        foreach (var item in _items)
        {
            item.TransitionOut();
        }
        _items.Clear();
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
            ClearTableItems();

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
        _currDish = _currLevel.Dishes[Random.Range(0, _currLevel.Dishes.Count)];
        CustomerManager.Instance.CurrentCustomerObject.GetComponent<CustomerUI>().OrderImage.sprite = _currDish.Sprite;

        InitializeIngredients();
    }

    public void CheckDrawnShape(Shape shape)
    {
        if (shape == null && !Frenzy.Instance.FrenzyEnabled.GetValue())
            return;

        if (Frenzy.Instance.FrenzyEnabled.GetValue() && MainGameManager.Instance.GameState.GetValue() == MainGameState.GAME_PREPARE)
        {
            // Adding ingredients, slowly increment addition
            IngredientItem item = (IngredientItem)_items.Find(i =>
            !(i as IngredientItem).IsActive);

            if (item != null)
            {
                item.AnimateActivate();
                // Check whether all items in list are active
                if (!_items.Any(item => (item as IngredientItem).IsActive == false))
                {
                    // If so, switch to next state
                    MainGameManager.Instance.GameState.SetValue(MainGameState.GAME_COOK);
                }
            }
        }

        else if (_currItemIndex >= 0 && _currItemIndex < _items.Count)
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

    public bool CheckAddIngredient(Ingredient ingredient)
    {
        if (_currDish.Ingredients.Contains(ingredient.Type))
        {
            IngredientItem item = (IngredientItem)_items.Find(i => 
            (i as IngredientItem).Ingredient.Type == ingredient.Type);

            if (item != null)
            {
                item.AnimateActivate();

                // Check whether all items in list are active
                if (!_items.Any(item => (item as IngredientItem).IsActive == false))
                {
                    // If so, switch to next state
                    MainGameManager.Instance.GameState.SetValue(MainGameState.GAME_COOK);
                }
                return true;
            }
        }

        return false;
    }

    public void InitializeIngredientButtons()
    {
        for (int i = 0; i < _currLevel.Ingredients.Count; ++i)
        {
            _ingredientButtons[i].Initialize(_currLevel.Ingredients[i]);
        }

        for (int j = _currLevel.Ingredients.Count; j < _ingredientButtons.Count; ++j)
        {
            // For remaining indexes, set button to inactive
            _ingredientButtons[j].gameObject.SetActive(false);
        }
    }
}
