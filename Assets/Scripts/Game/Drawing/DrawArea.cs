using UnityEngine;
using UnityEngine.EventSystems;

public class DrawArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        DrawManager.Instance.IsOverDrawingArea = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DrawManager.Instance.IsOverDrawingArea = false;
    }
}
