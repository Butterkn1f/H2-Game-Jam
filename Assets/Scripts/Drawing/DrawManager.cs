using System;
using UniRx;
using UnityEngine;

public class DrawManager : Common.DesignPatterns.Singleton<DrawManager>
{
    #region Customizable Variables
    [SerializeField] private GameObject _linePrefab;
    public float MinPointDistance = 0.15f;
    #endregion

    private CompositeDisposable _pressDisposables;

    private Camera _camera;
    [HideInInspector] public LineObject _lineObject;

    /// <summary>
    /// Initialize variables
    /// 変数の初期化
    /// </summary>
    private void Start()
    {
        _camera = Camera.main;
        _lineObject = null;
        _pressDisposables = new CompositeDisposable();
        SubscribeStateChange();
    }

    /// <summary>
    /// Enable or disable drawing based on GameManager's states
    /// GameManagerの状態に応じた描画の有効化・無効化
    /// </summary>
    private void SubscribeStateChange()
    {
        SubscribeBeginPress();
        SubscribePressing();
        SubscribeReleasePress();

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

    private void UnsubscribeAllPressEvents()
    {
        _pressDisposables.Dispose();
    }

    /// <summary>
    /// Switch GameManager states and instantiate a new line prefab
    /// GameManagerの状態を切り替え、新しいラインプレファブをインスタンス化する
    /// </summary>
    private void BeginDraw()
    {
        /*GameManager.Instance.GameState.SetState(GameStateType.Drawing);*/
        ResetDrawing();
        GameObject newLine = Instantiate(_linePrefab);
        _lineObject = newLine.GetComponent<LineObject>();
    }

    /// <summary>
    /// Line follows mouse cursor
    /// 線がマウスカーソルに追従する
    /// </summary>
    private void Draw()
    {
        if (_lineObject == null)
            return;

        Vector2 newPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        _lineObject.AddPoint(newPos);
    }

    /// <summary>
    /// Enable physics on the line and begin testing
    /// 回線で物理演算を有効にし、テストを開始する
    /// </summary>
    private void EndDraw()
    {
        if (_lineObject == null)
            return;

/*        if (_lineObject.PointCount < 2) // Not a line! セリフではありません！
        {
            Destroy(_lineObject.gameObject);
        }
        else
        {
            _lineObject.EnablePhysics();
            GameManager.Instance.GameState.SetState(GameStateType.Testing);
        }*/
    }

    private void ResetDrawing()
    {
        _pressDisposables = new CompositeDisposable();
        if (_lineObject == null)
            return;

        Destroy(_lineObject.gameObject);
        _lineObject = null;
    }
}
