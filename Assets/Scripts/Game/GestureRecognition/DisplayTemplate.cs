using System.Linq;
using UnityEngine;

public class DisplayTemplate : MonoBehaviour
{
    public void Draw(RecognitionManager.GestureTemplate gestureTemplate, DollarOneRecognizer.Step step)
    {
        DrawManager.Instance.BeginDraw(true);
        DrawManager.Instance._lineObject.Initialize();
        foreach (DollarPoint dollarPoint in gestureTemplate.Points)
        {
            DrawManager.Instance._lineObject.AddPoint(dollarPoint.Point);
        }
    }

    public void Clear()
    {
        DrawManager.Instance.ResetDrawing();
    }
}