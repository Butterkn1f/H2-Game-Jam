using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static RecognitionManager;

public class DrawManager : Common.DesignPatterns.Singleton<DrawManager>
{
    #region Customizable Variables
    [SerializeField] private GameObject _linePrefab;
    public float MinPointDistance = 0.15f;
    public List<Shape> Shapes = new List<Shape>();
    #endregion

    private CompositeDisposable _pressDisposables;
    private CompositeDisposable _drawFinishedDisposables;

    private Camera _camera;
    private bool _isDrawing;
    [HideInInspector] public bool IsOverDrawingArea;
    [HideInInspector] public LineObject _lineObject;
    [HideInInspector] public event Action<DollarPoint[]> OnDrawFinished;

    /// <summary>
    /// Initialize variables
    /// 変数の初期化
    /// </summary>
    private void Start()
    {
        _camera = Camera.main;
        _lineObject = null;
        IsOverDrawingArea = false;
        _isDrawing = false;
        SubscribeGameState();
        SubscribeFrenzy();
    }

    private void SubscribeGameState()
    {
        MainGameManager.Instance.GameState.Value.Subscribe(state =>
        {
            switch (state)
            {
                case MainGameState.GAME_PREPARE:
                    // During frenzy, begin allowing to draw at preparation state
                    if (Frenzy.Instance.FrenzyEnabled.GetValue() == true)
                        SubscribePressEvents(true);
                    break;

                case MainGameState.GAME_COOK:
                    // Begin allowing to draw during cooking state only when not in frenzy
                    // if in frenzy, events are already subscribed so no need to do so again
                    if (Frenzy.Instance.FrenzyEnabled.GetValue() == false)
                        SubscribePressEvents(true);
                    break;

                case MainGameState.GAME_WAIT:
                    SubscribePressEvents(false);
                    break;

                default:
                    break;

            }
        }).AddTo(this);
    }

    private void SubscribeFrenzy()
    {
        Frenzy.Instance.FrenzyEnabled.Value.Subscribe(enabled =>
        {
            if (enabled)
            {
                if (MainGameManager.Instance.GameState.GetValue() == MainGameState.GAME_PREPARE)
                {
                    // Enable ability to draw for ingredients in frenzy mode
                    SubscribePressEvents(true);
                }
            }
            else if (MainGameManager.Instance.GameState.GetValue() != MainGameState.GAME_COOK)
            {
                // After fever is over, if not in cooking state, remove ability to draw
                SubscribePressEvents(false);
            }
        }).AddTo(this);
    }

    /// <summary>
    /// Enable or disable drawing based on GameManager's states
    /// GameManagerの状態に応じた描画の有効化・無効化
    /// </summary>
    public void SubscribePressEvents(bool isSubscribe)
    {
        if (isSubscribe)
        {
            SubscribePressEvents(false);
            _pressDisposables = new CompositeDisposable();
            SubscribeBeginPress();
            SubscribePressing();
            SubscribeReleasePress();
        }
        else
        {
            if (_pressDisposables != null)
            {
                ResetDrawing();
                _pressDisposables.Dispose();
                _pressDisposables = null;

            }
        }
    }

    private void SubscribeBeginPress()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => BeginDraw())
            .AddTo(_pressDisposables);
    }

    private void SubscribePressing()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButton(0))
            .Subscribe(_ => Draw())
            .AddTo(_pressDisposables);
    }

    private void SubscribeReleasePress()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Subscribe(_ => EndDraw())
            .AddTo(_pressDisposables);
    }

    /// <summary>
    /// Switch GameManager states and instantiate a new line prefab
    /// GameManagerの状態を切り替え、新しいラインプレファブをインスタンス化する
    /// </summary>
    public void BeginDraw(bool skipChecks = false)
    {
        /*GameManager.Instance.GameState.SetState(GameStateType.Drawing);*/
        if (!skipChecks && (!IsOverDrawingArea || _isDrawing))
            return;

        ResetDrawing();
        GameObject newLine = Instantiate(_linePrefab);
        _lineObject = newLine.GetComponent<LineObject>();
        _isDrawing = true;
    }

    /// <summary>
    /// Line follows mouse cursor
    /// 線がマウスカーソルに追従する
    /// </summary>
    private void Draw()
    {
        if (_lineObject == null || !_isDrawing)
            return;

        if (!IsOverDrawingArea)
        {
            EndDraw();
        }
        else
        {
            Vector2 newPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            _lineObject.AddPoint(newPos);
        }
    }

    /// <summary>
    /// Enable physics on the line and begin testing
    /// 回線で物理演算を有効にし、テストを開始する
    /// </summary>
    private void EndDraw()
    {
        if (_lineObject == null || !_isDrawing)
            return;

        if (_lineObject.PointCount < 2) // Not a line! セリフではありません！
        {
            Destroy(_lineObject.gameObject);
        }
        else
        {
            if (RecognitionManager.Instance._state == RecognizerState.RECOGNITION)
            {
                // Invoke draw immediately
                InvokeDrawFinished();
            }

            // Else, do nothing, just wait for space bar press
        }
        _isDrawing = false;
    }

    private void InvokeDrawFinished(bool resetDrawing = false)
    {
        if (!_lineObject)
            return;
        List<DollarPoint> _drawPoints = new List<DollarPoint>();
        foreach (Vector2 point in _lineObject._points)
        {
            _drawPoints.Add(new DollarPoint() { Point = point, StrokeIndex = 0 });
        }

        OnDrawFinished?.Invoke(_drawPoints.ToArray());

        if (resetDrawing) {
            ResetDrawing();
        }
    }

    public void ResetDrawing()
    {
        RecognitionManager.Instance.StopLineAnimation();
        if (_lineObject == null)
            return;

        Destroy(_lineObject.gameObject);
        _lineObject = null;
        _isDrawing = false;
    }

    public void SubscribeDrawFinished(bool isSubscribe)
    {
        if (isSubscribe)
        {
            if (_drawFinishedDisposables == null || _drawFinishedDisposables.Count == 0)
            {
                _drawFinishedDisposables = new CompositeDisposable();
                Observable.EveryUpdate()
                .Where(_ => Input.GetKeyUp(KeyCode.Space))
                .Subscribe(_ => InvokeDrawFinished(true))
                .AddTo(_drawFinishedDisposables);
            }
        }
        else
        {
            _drawFinishedDisposables?.Dispose();
        }
    }

    public Shape GetShapeFromType(ShapeType type)
    {
        return Shapes.Find(s => s.Type == type);
    }
}
