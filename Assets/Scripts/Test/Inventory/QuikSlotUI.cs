using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class QuickSlotUI : MonoBehaviour, IDropHandler
{
    [Header("슬롯 설정")]
    [SerializeField] private int slotIndex;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("입력 액션 연결")]
    [SerializeField] private InputActionReference useSlotAction;

    private ItemData currentItem;
    private int currentAmount;
    private InventorySlot linkedSlot;

    private System.Action<InputAction.CallbackContext> cachedCallback;

    private void OnEnable()
    {
        if (useSlotAction != null)
        {
            cachedCallback = ctx => UseItem();
            useSlotAction.action.performed += cachedCallback;
            useSlotAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (useSlotAction != null && cachedCallback != null)
        {
            useSlotAction.action.performed -= cachedCallback;
            useSlotAction.action.Disable();
        }
    }

    public void SetItem(ItemData item, int amount)
    {
        currentItem = item;
        currentAmount = amount;

        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        countText.text = amount > 1 ? amount.ToString() : "";
    }

    public void SetSlot(InventorySlot slot)
    {
        linkedSlot = slot;

        if (slot != null && slot.item != null)
        {
            iconImage.sprite = slot.item.icon;
            iconImage.enabled = true;
            countText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        iconImage.enabled = false;
        countText.text = "";
    }

    public void UseItem()
    {
        Debug.Log($"[퀵슬롯 사용] 슬롯 {slotIndex} 시도");

        if (linkedSlot != null && linkedSlot.item != null)
        {
            Debug.Log($"[퀵슬롯 사용] {linkedSlot.item.itemName} 사용");

            linkedSlot.quantity--;

            if (linkedSlot.quantity <= 0)
            {
                linkedSlot.item = null;
                ClearSlot();
            }
            else
            {
                countText.text = linkedSlot.quantity.ToString();
            }

            InventoryUI.Instance.RefreshAllSlots();
        }
    }

    public ItemData GetItem() => currentItem;
    public int GetAmount() => currentAmount;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = InventorySlotUI.draggedItem;

        if (draggedItem == null)
        {
            Debug.LogWarning("드래그된 아이템이 없음");
            return;
        }

        if (draggedItem.itemType != ItemType.Consumable)
        {
            Debug.Log("소비 아이템만 등록 가능");
            return;
        }

        Debug.Log($"[퀵슬롯 등록] {draggedItem.itemName} → 슬롯 {slotIndex}");

        foreach (var slot in InventoryManager.Instance.slots)
        {
            if (slot.item == draggedItem)
            {
                SetSlot(slot);
                return;
            }
        }

        Debug.LogWarning("[퀵슬롯 등록 실패] 인벤토리에 해당 아이템 슬롯을 찾을 수 없음");
    }

    public void RefreshSlotUI()
    {
        if (linkedSlot != null && linkedSlot.item != null)
        {
            iconImage.sprite = linkedSlot.item.icon;
            iconImage.enabled = true;
            countText.text = linkedSlot.quantity > 1 ? linkedSlot.quantity.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }
}
