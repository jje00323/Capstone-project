using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("장비 슬롯 종류")]
    public EquipmentType slotType;

    [Header("UI")]
    public Image iconImage;
    public TextMeshProUGUI nameText;

    private EquipmentData equippedItem;

    public static EquipmentSlotUI draggedEquipSlot;

    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySlotUI.draggedItem == null || InventorySlotUI.draggedSlotUI == null)
            return;

        if (!(InventorySlotUI.draggedItem is EquipmentData equipment))
            return;

        if (equipment.equipmentType != slotType)
        {
            Debug.LogWarning($"[장비 장착 실패] {equipment.equipmentType}는 {slotType} 슬롯에 맞지 않음");
            return;
        }

        // 기존 장비 백업 (스왑은 안 하지만 여차하면 확장 가능)
        var oldItem = equippedItem;

        // 장비 적용
        SetItem(equipment);
        EquipmentManager.Instance.EquipItem(equipment);

        // 출처 제거
        if (InventorySlotUI.draggedSlotUI is InventorySlotUI invSlot)
        {
            invSlot.RemoveItemFromSlot();
        }
        else if (InventorySlotUI.draggedSlotUI is QuickSlotUI quickSlot)
        {
            quickSlot.RemoveItemFromSlot();
        }
        else if (InventorySlotUI.draggedSlotUI is EquipmentSlotUI otherEquipSlot)
        {
            otherEquipSlot.RemoveItemFromSlot(); // 만약 장비 → 장비 교환을 고려한다면 SetItem(oldItem)으로 교체
        }

        // 정리
        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedEquipSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void SetItem(EquipmentData item)
    {
        equippedItem = item;

        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
            nameText.text = item.itemName;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        EquipmentManager.Instance.UnequipItem(equippedItem);
        equippedItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        nameText.text = slotType.ToString();
    }

    public EquipmentData GetEquippedItem() => equippedItem;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (equippedItem == null) return;

        InventorySlotUI.draggedItem = equippedItem;
        InventorySlotUI.draggedSlotUI = this;
        draggedEquipSlot = this;
        DragIconUI.Instance.Show(equippedItem.icon);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedEquipSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void RemoveItemFromSlot()
    {
        ClearSlot();
    }
}