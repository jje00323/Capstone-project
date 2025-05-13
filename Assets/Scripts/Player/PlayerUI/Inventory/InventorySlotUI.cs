using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;

    private InventorySlot slotData;
    private int _slotIndex;

    public static ItemData draggedItem;
    public static MonoBehaviour draggedSlotUI;

    public void SetSlotIndex(int index) => _slotIndex = index;

    public void SetSlot(InventorySlot slot)
    {
        slotData = slot;
        RefreshSlotUI();
    }

    public InventorySlot GetSlotData() => slotData;

    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.gameObject.SetActive(false);
        quantityText.text = "";
    }

    public void RefreshSlotUI()
    {
        if (slotData.item != null)
        {
            iconImage.sprite = slotData.item.icon;

          
            if (!iconImage.gameObject.activeSelf)
                iconImage.gameObject.SetActive(true);

            if (!quantityText.gameObject.activeSelf)
                quantityText.gameObject.SetActive(true);

            quantityText.text = slotData.quantity > 1 ? slotData.quantity.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotData.item != null)
            TooltipUI.Instance.Show(slotData.item.itemName, slotData.item.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData.item == null) return;

        draggedItem = slotData.item;
        draggedSlotUI = this;
        DragIconUI.Instance.Show(slotData.item.icon);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 필수로 있어야 드래그 감지됨 (비워도 됨)
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedItem = null;
        draggedSlotUI = null;
        DragIconUI.Instance.Hide();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedItem == null || draggedSlotUI == null || draggedSlotUI == this) return;

        // 기존 아이템 백업
        var tempItem = slotData.item;
        var tempQty = slotData.quantity;

        switch (draggedSlotUI)
        {
            case QuickSlotUI quick:
                // 퀵슬롯 → 인벤토리 드래그 시, 인벤토리에는 아무것도 하지 않고 퀵슬롯만 삭제
                Debug.Log("[InventorySlotUI] 퀵슬롯 → 인벤토리 드래그 감지됨 → 퀵슬롯 아이템 삭제 처리");
                quick.ClearSlot(); // 퀵슬롯 비움
                break;

            case InventorySlotUI otherSlot:
                var otherData = otherSlot.GetSlotData();
                slotData.item = otherData.item;
                slotData.quantity = otherData.quantity;
                RefreshSlotUI();
                otherData.item = tempItem;
                otherData.quantity = tempQty;
                otherSlot.RefreshSlotUI();
                break;

            case EquipmentSlotUI equipSlot:
                var equipItem = equipSlot.GetEquippedItem();
                if (equipItem != null)
                {
                    slotData.item = equipItem;
                    slotData.quantity = 1;
                    RefreshSlotUI();
                    equipSlot.RemoveItemFromSlot();
                }
                break;
        }

        // 마무리
        draggedItem = null;
        draggedSlotUI = null;
        DragIconUI.Instance.Hide();
    }

    public void SetItemToSlot(ItemData item, int quantity = 1)
    {
        slotData.item = item;
        slotData.quantity = quantity;
        RefreshSlotUI();
    }

    public void RemoveItemFromSlot()
    {
        Debug.Log($"[InventorySlotUI] RemoveItemFromSlot() - 제거 대상: {(slotData.item != null ? slotData.item.itemName : "없음")}");
        slotData.item = null;
        slotData.quantity = 0;
        RefreshSlotUI();
    }

    public int GetAmount() => slotData != null ? slotData.quantity : 1;
}