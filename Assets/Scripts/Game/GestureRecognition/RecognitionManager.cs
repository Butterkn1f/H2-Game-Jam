using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class RecognitionManager : Common.DesignPatterns.Singleton<RecognitionManager>
{
    private DrawManager _drawManager;
    private Sequence lineSequence;

    #region Customizable Values
    [SerializeField] private Button _templateModeButton;
    [SerializeField] private Button _reviewTemplates;
    [SerializeField] private TMP_InputField _templateName;
    [SerializeField] private TemplateReviewPanel _templateReviewPanel;

    [SerializeField] private Canvas _templateCanvas;

    [SerializeField] private Gradient _frenzyColor;
    #endregion

    private GestureTemplates _templates => GestureTemplates.Get();
    //private static readonly DollarOneRecognizer _dollarOneRecognizer = new DollarOneRecognizer();
    private static readonly DollarPRecognizer _dollarPRecognizer = new DollarPRecognizer();
    private IRecognizer _currentRecognizer = _dollarPRecognizer;
    [HideInInspector] public RecognizerState _state = RecognizerState.RECOGNITION;

    public enum RecognizerState
    {
        RECOGNITION,
        TEMPLATE,
        TEMPLATE_REVIEW
    }

    [Serializable]
    public struct GestureTemplate
    {
        public string Name;
        public DollarPoint[] Points;

        public GestureTemplate(string templateName, DollarPoint[] preparePoints)
        {
            Name = templateName;
            Points = preparePoints;
        }
    }

    private string TemplateName => _templateName.text;


    private void Start()
    {
        _drawManager = DrawManager.Instance;
        _drawManager.OnDrawFinished += OnDrawFinished;
        _templateModeButton.onClick.AddListener(() => SetupState(RecognizerState.TEMPLATE));
        _reviewTemplates.onClick.AddListener(() => SetupState(RecognizerState.TEMPLATE_REVIEW));

        SetupState(_state);
        SubscribeTemplateEditing();
    }

    private void SubscribeTemplateEditing()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.Semicolon))
            .Subscribe(_ => SwitchEditingMode())
            .AddTo(this);
    }

    private void SwitchEditingMode()
    {
        if (_state == RecognizerState.RECOGNITION)
        {
            _templateCanvas.gameObject.SetActive(true);
            SetupState(RecognizerState.TEMPLATE);
        }
        else
        {
            _templateCanvas.gameObject.SetActive(false);
            SetupState(RecognizerState.RECOGNITION);
        }
    }

    private void SetupState(RecognizerState state)
    {
        _state = state;
        _templateModeButton.image.color = _state == RecognizerState.TEMPLATE ? Color.green : Color.white;
        _reviewTemplates.image.color = _state == RecognizerState.TEMPLATE_REVIEW ? Color.green : Color.white;
        _templateName.gameObject.SetActive(_state == RecognizerState.TEMPLATE);

        //_drawable.gameObject.SetActive(state != RecognizerState.TEMPLATE_REVIEW);
        _templateReviewPanel.SetVisibility(state == RecognizerState.TEMPLATE_REVIEW);

        // If state is not recognition, subscribe to space check to invoke draw finished
        _drawManager.SubscribeDrawFinished(state != RecognizerState.RECOGNITION);

        if (state == RecognizerState.TEMPLATE_REVIEW)
        {
            DrawManager.Instance.SubscribePressEvents(false);
        }
        else
        {
            DrawManager.Instance.SubscribePressEvents(true);
        }
        _drawManager.ResetDrawing();
    }

    private void OnDrawFinished(DollarPoint[] points)
    {
        if (_state == RecognizerState.TEMPLATE)
        {
            GestureTemplate preparedTemplate =
                new GestureTemplate(TemplateName, _currentRecognizer.Normalize(points, 64));
            _templates.RawTemplates.Add(new GestureTemplate(TemplateName, points));
            _templates.ProceedTemplates.Add(preparedTemplate);
        }
        else
        {
            RecognizePoints(points);
        }
    }

    private void RecognizePoints(DollarPoint[] points)
    {
        //  (string, float) result = _dollarOneRecognizer.DoRecognition(points, 64, _templates.GetTemplates());
        (string, float) result = _currentRecognizer.DoRecognition(points, 64, _templates.RawTemplates);
        Debug.Log($"Recognized: {result.Item1}, Distance: {result .Item2}");

        Shape shape = null;
        lineSequence = DOTween.Sequence();
        Color2 currColor = _drawManager._lineObject.GetCurrentColor2();

        if (Frenzy.Instance.FrenzyEnabled.GetValue())
        {
            // Correct no matter what is drawn
            // Blend from default to frenzy line's color
            Color2 tarColor = new Color2(_frenzyColor.colorKeys[0].color, _frenzyColor.colorKeys[1].color);
            lineSequence.Append(_drawManager._lineObject._lineRenderer
                .DOColor(currColor, tarColor, 0.25f))
                .AppendInterval(0.15f);

            currColor = tarColor;
        }
        else if (result.Item2 <= 2)
        {
            // Check for shape recognition
            ShapeType shapeType = (ShapeType)System.Enum.Parse(typeof(ShapeType), result.Item1);
            shape = _drawManager.GetShapeFromType(shapeType);
            if (shape != null)
            {
                // Blend from default to target shape's color
                Color2 tarColor = new Color2(shape.Color, shape.Color);
                lineSequence.Append(_drawManager._lineObject._lineRenderer
                    .DOColor(currColor, tarColor, 0.25f))
                    .AppendInterval(0.15f);

                currColor = tarColor;
            }
        }

        // Fade out line
        Color2 invisColor = new Color2(currColor.ca * new Vector4(1, 1, 1, 0), currColor.cb * new Vector4(1, 1, 1, 0));
        lineSequence.Append(_drawManager._lineObject._lineRenderer
                .DOColor(currColor, invisColor, 0.5f));

        DishManager.Instance.CheckDrawnShape(shape);
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        _templates.Save();
    }

    public void StopLineAnimation()
    {
        lineSequence.Kill();
    }
}