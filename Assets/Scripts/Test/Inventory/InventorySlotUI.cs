using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;

    private InventorySlot slotData;
    private int _slotIndex;

    // 전역 드래그용 아이템 참조
    public static ItemData draggedItem;

    // 슬롯 인덱스 설정
    public void SetSlotIndex(int index)
    {
        _slotIndex = index;
    }

    // 슬롯 데이터 설정
    public void SetSlot(InventorySlot slot)
    {
        this.slotData = slot;

        if (slot.item != null)
        {
            iconImage.sprite = slot.item.icon;
            iconImage.gameObject.SetActive(true);
            quantityText.text = slot.quantity.ToString();
        }
        else
        {
            ClearSlot();
        }
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.gameObject.SetActive(false);
        quantityText.text = "";
    }

    // 슬롯 데이터 반환
    public InventorySlot GetSlotData()
    {
        return slotData;
    }

    // 툴팁 표시
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotData != null && slotData.item != null)
        {
            TooltipUI.Instance.Show(slotData.item.itemName, slotData.item.description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData != null && slotData.item != null)
        {
            draggedItem = slotData.item;
            Debug.Log($"[InventorySlotUI] 드래그 시작: {draggedItem.itemName}");

            if (DragIconUI.Instance != null)
            {
                DragIconUI.Instance.Show(slotData.item.icon);
                Debug.Log("[InventorySlotUI] DragIconUI 호출됨");
            }
            else
            {
                Debug.LogError("[InventorySlotUI] DragIconUI.Instance 가 null임!");
            }
        }
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        // DragIconUI 자체가 Update에서 마우스를 따라가므로 이 부분은 비워둬도 OK
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        draggedItem = null;
        DragIconUI.Instance.Hide();
    }
}