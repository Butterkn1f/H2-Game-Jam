﻿using System;
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
    }

    /// <summary>
    /// Enable or disable drawing based on GameManager's states
    /// GameManagerの状態に応じた描画の有効化・無効化
    /// </summary>
    public void SubscribePressEvents(bool isSubscribe)
    {
        if (isSubscribe)
        {
            if (_pressDisposables == null || _pressDisposables.Count == 0)
            {
                _pressDisposables = new CompositeDisposable();
                SubscribeBeginPress();
                SubscribePressing();
                SubscribeReleasePress();
            }
        }
        else
        {
            _pressDisposables.Dispose();
        }

        /*GameManager.Instance.GameState.State.Subscribe(state =>
        {
            switch (state)
            {
                case GameStateType.Playing:
                    SubscribeBeginPress();
                    SubscribePressing();
                    SubscribeReleasePress();
                    break;

                case GameStateType.Testing:
                    UnsubscribeAllPressEvents();
                    break;

                case GameStateType.Initialize:
                    ResetDrawing();
                    break;

                default:
                    break;
            }
        }).AddTo(this);*/
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
        if (_lineObject == null)
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
        _pressDisposables = new CompositeDisposable();
        if (_lineObject == null)
            return;

        Destroy(_lineObject.gameObject);
        _lineObject = null;
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

    public Shape GetShapeFromName(string name)
    {
        return Shapes.Find(s => s.Name == name);
    }
}
