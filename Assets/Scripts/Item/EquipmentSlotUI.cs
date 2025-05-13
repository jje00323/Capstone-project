using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("장비 슬롯 종류")]
    public EquipmentType slotType;

    [Header("UI")]
    public Image iconImage;

    private EquipmentData equippedItem;
    private int currentAmount = 1;

    public static EquipmentSlotUI draggedEquipSlot;

    public string itemName;
    public string itemDescription;

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

        var oldItem = equippedItem;

        // 장비 장착
        SetItem(equipment);
        EquipmentManager.Instance.EquipItem(equipment);

        if (oldItem != null)
        {
            // 교환: 기존 장비를 원래 드래그한 슬롯에 넣기
            switch (InventorySlotUI.draggedSlotUI)
            {
                case InventorySlotUI invSlot:
                    invSlot.SetItemToSlot(oldItem, 1);
                    break;
                case QuickSlotUI quickSlot:
                    
                    break;
                case EquipmentSlotUI equipSlot:
                    equipSlot.SetItem(oldItem);
                    break;
            }
        }
        else
        {
            // 장비창이 비어 있었던 경우 → 원래 슬롯 비우기
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

            itemName = item.itemName;
            itemDescription = item.description;
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



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (equippedItem == null) return; //  아이템이 없을 때 툴팁 차단

        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Show(itemName, itemDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
    }
}