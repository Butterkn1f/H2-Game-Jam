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
    [SerializeField] private CanvasGroup _wokSoup;
    [SerializeField] private CanvasGroup _wokLid;
    #endregion

    private Dish _currDish = null;
    private int _currItemIndex = -1;
    private List<ITableItem> _items = new List<ITableItem>(); // TODO: Object pool this
    private List<IngredientType> _addedIngredients = new List<IngredientType>();

    private void Start()
    {
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
                    _wokLid.DOFade(1, 0.5f);
                    break;

                case MainGameState.GAME_WAIT:
                    TutorialManager.Instance.AdvanceTutorial(5);
                    ResetDish();
                    _wokSoup.DOFade(0, 0.25f);
                    _wokLid.DOFade(0, 0.25f);
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
            var sign = (i % 2 == 0) ? 1 : -1;
            var floatOffset = Random.Range(10f, 30f) * sign;

            GameObject item = Instantiate(_ingredientItemPrefab, _tableParentTransform);
            IngredientItem ingItem = item.GetComponent<IngredientItem>();

            ingItem._image.rectTransform.anchoredPosition = new Vector2(0, floatOffset);
            ingItem.Initialize();
            _items.Add(ingItem);
        }
    }

    private void InitializeDishItems()
    {
        TutorialManager.Instance.AdvanceTutorial(4);
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
        _addedIngredients.Clear();

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
            bool isCorrect = CheckCorrectIngredients();

            dishItem.InitializeDish(isCorrect ? _currDish : null);

            dishItem.ServeCustomerAnimation(isCorrect);
        });
    }

    private bool CheckCorrectIngredients()
    {
        _addedIngredients.Sort();
        _currDish.Ingredients.Sort();

        for (int i = 0; i < _addedIngredients.Count; i++)
        {
            if (_addedIngredients[i] != _currDish.Ingredients[i])
            {
                return false;
            }
        }

        return true;
    }

    public void RandomizeNewDish()
    {
        _currDish = LevelManager.Instance.CurrLevel.Dishes[Random.Range(0, LevelManager.Instance.CurrLevel.Dishes.Count)];
        CustomerManager.Instance.CurrentCustomerObject.GetComponent<CustomerUI>().InitializeFoodBubble(_currDish);

        InitializeIngredients();
    }

    public void CheckDrawnShape(Shape shape)
    {
        if (shape == null && !Frenzy.Instance.FrenzyEnabled.GetValue())
            return;

        if (Frenzy.Instance.FrenzyEnabled.GetValue() && MainGameManager.Instance.GameState.GetValue() == MainGameState.GAME_PREPARE)
        {
            // Adding ingredients, slowly increment addition
            IngredientItem item = (IngredientItem)_items.Find(i =>!(i as IngredientItem).IsActive);

            IngredientType unaddedIngredient = _currDish.Ingredients.Find(i => !_addedIngredients.Contains(i));
            var ingredient = LevelManager.Instance.CurrLevel.Ingredients.Find(ing => ing.Type == unaddedIngredient);

            if (item != null && ingredient != null)
            {
                _addedIngredients.Add(ingredient.Type);
                item.AnimateActivate(ingredient).OnComplete(() =>
                {
                    // Check whether all items in list are active
                    if (!_items.Any(item => (item as IngredientItem).IsActive == false))
                    {
                        // If so, switch to next state
                        MainGameManager.Instance.GameState.SetValue(MainGameState.GAME_COOK);
                    }
                });
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
        if (!_addedIngredients.Contains(ingredient.Type))
        {
            foreach(IngredientItem item in _items)
            {
                if (!item.IsActive)
                {
                    if (_addedIngredients.Count == 0)
                    {
                        _wokSoup.DOFade(1, 0.5f);
                    }

                    _addedIngredients.Add(ingredient.Type);
                    item.AnimateActivate(ingredient).OnComplete(() =>
                    {
                        // Check whether all items in list are active
                        if (!_items.Any(item => (item as IngredientItem).IsActive == false))
                        {
                            // If so, switch to next state
                            MainGameManager.Instance.GameState.SetValue(MainGameState.GAME_COOK);
                        }
                    });

                    break;
                }
            }
            return true;
        }


        return false;

/*        if (_currDish.Ingredients.Contains(ingredient.Type))
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

        return false;*/
    }

    public void InitializeIngredientButtons()
    {
        for (int i = 0; i < LevelManager.Instance.CurrLevel.Ingredients.Count; ++i)
        {
            _ingredientButtons[i].Initialize(LevelManager.Instance.CurrLevel.Ingredients[i]);
        }

        for (int j = LevelManager.Instance.CurrLevel.Ingredients.Count; j < _ingredientButtons.Count; ++j)
        {
            // For remaining indexes, set button to inactive
            _ingredientButtons[j].gameObject.SetActive(false);
        }
    }
}
