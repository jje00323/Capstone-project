using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuickSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private QuickSlotUI[] slots;

    private ItemData currentItem;
    private int currentAmount;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) slots[0].UseItem();
        if (Input.GetKeyDown(KeyCode.Alpha2)) slots[1].UseItem();
        if (Input.GetKeyDown(KeyCode.Alpha3)) slots[2].UseItem();
        if (Input.GetKeyDown(KeyCode.Alpha4)) slots[3].UseItem();
    }

    public void SetItem(ItemData item, int amount)
    {
        currentItem = item;
        currentAmount = amount;

        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        countText.text = amount > 1 ? amount.ToString() : "";
    }

    public void ClearSlot()
    {
        currentItem = null;
        iconImage.enabled = false;
        countText.text = "";
    }

    public void UseItem()
    {
        if (currentItem != null)
        {
            Debug.Log($"[퀵슬롯 {slotIndex}] {currentItem.itemName} 사용됨!");
            // 여기서 실제 효과 처리 가능
            currentAmount--;

            if (currentAmount <= 0)
            {
                ClearSlot();
            }
            else
            {
                countText.text = currentAmount.ToString();
            }
        }
    }

    public ItemData GetItem() => currentItem;
    public int GetAmount() => currentAmount;

    // 드래그 앤 드롭 처리
    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = InventorySlotUI.draggedItem;

        if (draggedItem == null)
        {
            Debug.LogWarning("드래그된 아이템이 없음");
            return;
        }

        // 소비 아이템만 등록 가능
        if (draggedItem.itemType != ItemType.Consumable)
        {
            Debug.Log("이 슬롯에는 소비 아이템만 등록할 수 있습니다!");
            return;
        }

        Debug.Log($"[퀵슬롯 등록] {draggedItem.itemName} → 슬롯 {slotIndex}");
        SetItem(draggedItem, 1); // 기본 수량 1개로 등록
    }
}