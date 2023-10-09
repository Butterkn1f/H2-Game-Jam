using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class RecognitionManager : Common.DesignPatterns.Singleton<RecognitionManager>
{
    private DrawManager _drawManager;
    [SerializeField] private Button _templateModeButton;
    [SerializeField] private Button _reviewTemplates;
    [SerializeField] private TMP_InputField _templateName;
    [SerializeField] private TemplateReviewPanel _templateReviewPanel;

    [SerializeField] private Canvas _templateCanvas;

    private GestureTemplates _templates => GestureTemplates.Get();
    private static readonly DollarOneRecognizer _dollarOneRecognizer = new DollarOneRecognizer();
    private IRecognizer _currentRecognizer = _dollarOneRecognizer;
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
        // TODO: Colour?
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
        _drawManager.ResetDrawing();
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
            //  (string, float) result = _dollarOneRecognizer.DoRecognition(points, 64, _templates.GetTemplates());
            (string, float) result = _currentRecognizer.DoRecognition(points, 64,
                _templates.RawTemplates);
            string resultText = "";
            if (_currentRecognizer is DollarOneRecognizer)
            {
                resultText = $"Recognized: {result.Item1}, Score: {result.Item2}";
            }
            Debug.Log(resultText);
        }
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        _templates.Save();
    }
}