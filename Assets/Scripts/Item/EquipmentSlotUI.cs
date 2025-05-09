using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("장비 슬롯 종류")]
    public EquipmentType slotType;

    [Header("UI")]
    public Image iconImage;
    public TextMeshProUGUI nameText;

    private EquipmentData equippedItem;

    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySlotUI.draggedItem == null) return;

        if (!(InventorySlotUI.draggedItem is EquipmentData equipment)) return;

        if (equipment.equipmentType != slotType)
        {
            Debug.LogWarning($"[장비 장착 실패] {equipment.equipmentType}는 {slotType} 슬롯에 맞지 않음");
            return;
        }

        // 기존 장비 장착
        EquipmentManager.Instance.EquipItem(equipment);
        SetItem(equipment);

        //  draggedSlotUI 안전하게 캐스팅 후 제거
        if (InventorySlotUI.draggedSlotUI is InventorySlotUI invSlot)
        {
            invSlot.RemoveItemFromSlot();
        }
        else if (InventorySlotUI.draggedSlotUI is QuickSlotUI quickSlot)
        {
            quickSlot.RemoveItemFromSlot();
        }
        else if (InventorySlotUI.draggedSlotUI is EquipmentSlotUI equipSlot)
        {
            equipSlot.RemoveItemFromSlot();
        }

        // 초기화
        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        DragIconUI.Instance.Hide();
    }

    public void SetItem(EquipmentData item)
    {
        equippedItem = item;
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        nameText.text = item.itemName;
    }

    public void ClearSlot()
    {
        equippedItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        nameText.text = slotType.ToString();
    }

    public EquipmentData GetEquippedItem() => equippedItem;

    //  드래그로 장비 해제
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (equippedItem == null) return;

        InventorySlotUI.draggedItem = equippedItem;
        InventorySlotUI.draggedSlotUI = this; //  추가
        DragIconUI.Instance.Show(equippedItem.icon);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlotUI.draggedItem = null;
        DragIconUI.Instance.Hide();
    }

    public void RemoveItemFromSlot()
    {
        EquipmentManager.Instance.UnequipItem(equippedItem);
        ClearSlot();
    }
}