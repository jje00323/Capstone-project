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

    private EquipmentData equippedItem;
    private int currentAmount = 1;

    public static EquipmentSlotUI draggedEquipSlot;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("[EquipmentSlotUI] OnDrop 시작");

        if (InventorySlotUI.draggedItem == null || InventorySlotUI.draggedSlotUI == null)
        {
            Debug.LogWarning("[EquipmentSlotUI] 드래그된 항목이 null입니다.");
            return;
        }

        if (!(InventorySlotUI.draggedItem is EquipmentData equipment))
        {
            Debug.LogWarning("[EquipmentSlotUI] 드래그된 아이템이 EquipmentData가 아닙니다.");
            return;
        }

        if (equipment.equipmentType != slotType)
        {
            Debug.LogWarning($"[장비 장착 실패] {equipment.equipmentType}는 {slotType} 슬롯에 맞지 않음");
            return;
        }

        Debug.Log($"[장비 장착 시도] {equipment.itemName} → {slotType}");

        // 기존 장비 백업
        var oldItem = equippedItem;

        // 장비 장착
        SetItem(equipment);
        EquipmentManager.Instance.EquipItem(equipment);

        // 기존 장비를 인벤토리에 추가
        if (oldItem != null)
        {
            bool added = InventoryManager.Instance.AddItem(oldItem, 1);
            if (!added)
                Debug.LogWarning($"[장비 교체 실패] 인벤토리에 {oldItem.itemName} 추가 실패");
        }

        // 드래그된 슬롯에서 제거
        switch (InventorySlotUI.draggedSlotUI)
        {
            case InventorySlotUI invSlot:
                invSlot.RemoveItemFromSlot();
                break;
            case QuickSlotUI quickSlot:
                quickSlot.RemoveItemFromSlot();
                break;
            case EquipmentSlotUI equipSlot:
                equipSlot.RemoveItemFromSlot();
                break;
        }

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
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        equippedItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    public EquipmentData GetEquippedItem() => equippedItem;
    public int GetAmount() => currentAmount;

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
        if (equippedItem != null)
        {
            EquipmentManager.Instance.UnequipItem(equippedItem); // 스탯 제거
        }

        ClearSlot(); // UI 정리
    }
}