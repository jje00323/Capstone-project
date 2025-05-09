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

    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.gameObject.SetActive(false);
        quantityText.text = "";
    }

    public InventorySlot GetSlotData() => slotData;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotData != null && slotData.item != null)
            TooltipUI.Instance.Show(slotData.item.itemName, slotData.item.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotData != null && slotData.item != null)
        {
            draggedItem = slotData.item;
            draggedSlotUI = this;
            DragIconUI.Instance.Show(slotData.item.icon);
        }
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedItem = null;
        draggedSlotUI = null;
        DragIconUI.Instance.Hide();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedItem == null || draggedSlotUI == null) return;

        if (draggedSlotUI == this) return; // 자기 자신이면 무시

        if (draggedSlotUI is QuickSlotUI quick)
        {
            // 기존 인벤토리 슬롯 아이템 임시 저장
            var tempItem = slotData.item;
            var tempQty = slotData.quantity;

            // 1. 퀵슬롯 -> 인벤토리
            slotData.SetItem(quick.GetItem(), quick.GetAmount());
            SetSlot(slotData);

            // 2. 기존 인벤토리 아이템은 퀵슬롯으로
            quick.SetItem(tempItem, tempQty);
        }

        else if (draggedSlotUI is InventorySlotUI otherSlot)
        {
            // 인벤토리 ↔ 인벤토리 교환
            var tempItem = slotData.item;
            var tempQty = slotData.quantity;

            slotData.SetItem(otherSlot.GetSlotData().item, otherSlot.GetSlotData().quantity);
            SetSlot(slotData);

            otherSlot.GetSlotData().SetItem(tempItem, tempQty);
            otherSlot.SetSlot(otherSlot.GetSlotData());
        }

        draggedItem = null;
        draggedSlotUI = null;
        DragIconUI.Instance.Hide();
    }

    public void SetItemToSlot(ItemData item)
    {
        slotData.item = item;
        slotData.quantity = 1;
        iconImage.sprite = item.icon;
        iconImage.gameObject.SetActive(true);
        quantityText.text = "1";
    }

    public void RemoveItemFromSlot()
    {
        slotData.item = null;
        slotData.quantity = 0;
        ClearSlot();
    }
}
