using UnityEngine;
using UnityEngine.EventSystems;

public class MovableHeaderUI : MonoBehaviour, IDragHandler
{
    public RectTransform targetToMove;

    public void OnDrag(PointerEventData eventData)
    {
        if (targetToMove != null)
        {
            targetToMove.anchoredPosition += eventData.delta;
        }
    }
}