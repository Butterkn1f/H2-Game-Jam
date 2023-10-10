using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DishManager : Common.DesignPatterns.Singleton<DishManager>
{
    #region Customizable Variables
    [SerializeField] private GameObject _dishItemPrefab;
    [SerializeField] private RectTransform _tableParentTransform;
    [SerializeField] private TextMeshProUGUI _text;
    public List<Dish> Dishes = new List<Dish>();
    #endregion

    private Dish _currDish = null;
    private int _currItemIndex = -1;
    private List<DishItem> _items = new List<DishItem>();

    void Start()
    {
        RandomizeNewDish();
    }

    private void InitializeDishItems()
    {
        // Add all allowed ingredients first,
        // Then add random number of excess
        int itemQty = Random.Range(_currDish.MinIngredients, _currDish.MaxIngredients + 1);

        List<Shape> starterShapes = new List<Shape>();
        foreach(string ingredient in _currDish.AllowedIngredients)
        {
            if (starterShapes.Count >= itemQty)
                break;

            starterShapes.Add(DrawManager.Instance.GetShapeFromName(ingredient));
        }

        List<Shape> shapes = new List<Shape>();
        for (int i = 0; i < (itemQty - starterShapes.Count); ++i)
        {
            shapes.Add(starterShapes[Random.Range(0, starterShapes.Count)]);
        }

        shapes.AddRange(starterShapes);
        // Shuffle shapes using fisher–yates algorithm
        for (int i = 0; i < shapes.Count; i++)
        {
            // Pick random Element
            int j = Random.Range(i, shapes.Count);

            // Swap Elements
            (shapes[j], shapes[i]) = (shapes[i], shapes[j]);
        }

        // Create dish items based on shapes
        foreach(Shape shape in shapes)
        {
            GameObject item = Instantiate(_dishItemPrefab, _tableParentTransform);
            DishItem dishItem = item.GetComponent<DishItem>();
            dishItem.Initialize(shape);
            _items.Add(dishItem);
        }
    }

    private void ResetDish()
    {
        foreach(DishItem dishItem in _items)
        {
            dishItem.KillAnimation();
        }

        foreach (Transform child in _tableParentTransform)
        {
            Destroy(child.gameObject);
        }

        _items.Clear();
        _currDish = null;
        _currItemIndex = -1;
    }

    private void MergeIngredients()
    {

    }

    public void RandomizeNewDish()
    {
        ResetDish();
        _currDish = Dishes[Random.Range(0, Dishes.Count)];
        _text.text = _currDish.Name;
        InitializeDishItems();
        _currItemIndex = 0;
    }

    public void CheckDrawnShape(Shape shape)
    {
        if (shape == null)
            return;

        if (_currItemIndex >= 0 && _currItemIndex < _items.Count)
        {
            DishItem currItem = _items[_currItemIndex];
            if (shape == currItem.Shape)
            {
                currItem.ActivateItem();
                _currItemIndex++;

                if (_currItemIndex == _items.Count)
                {
                    // TODO: Next customer call
                    RandomizeNewDish();
                }
            }
        }
    }
}
