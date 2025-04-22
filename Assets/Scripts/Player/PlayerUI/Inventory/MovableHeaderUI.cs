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

    public GameObject inventoryUIRoot;

    public void CloseUI()
    {
        if (inventoryUIRoot != null)
            inventoryUIRoot.SetActive(false);
    }
}