using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class QuickSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("½½·Ô ¼³Á¤")]
    [SerializeField] private int slotIndex;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("ÀÔ·Â ¾×¼Ç ¿¬°á")]
    [SerializeField] private InputActionReference useSlotAction;

    private ItemData currentItem;
    private int currentAmount = 1;

    private System.Action<InputAction.CallbackContext> cachedCallback;

    public static QuickSlotUI draggedQuickSlot;

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

    public void SetItem(ItemData item, int amount = 1)
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
        currentAmount = 0;
        iconImage.enabled = false;
        countText.text = "";
    }

    public void UseItem()
    {
        if (currentItem != null && currentItem is ConsumableData consumable)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null && player.TryGetComponent(out PlayerStatus playerStatus))
            {
                consumable.ApplyEffect(playerStatus);
            }

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

    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySlotUI.draggedItem == null || InventorySlotUI.draggedSlotUI == null)
            return;

        if (InventorySlotUI.draggedItem.itemType != ItemType.Consumable)
        {
            Debug.Log("¼Òºñ ¾ÆÀÌÅÛ¸¸ Äü½½·Ô¿¡ µî·Ï °¡´ÉÇÕ´Ï´Ù.");
            return;
        }

        // Äü½½·Ô ¡ê Äü½½·Ô ±³È¯
        if (InventorySlotUI.draggedSlotUI is QuickSlotUI otherQuickSlot)
        {
            var tempItem = otherQuickSlot.currentItem;
            var tempAmount = otherQuickSlot.currentAmount;

            otherQuickSlot.SetItem(this.currentItem, this.currentAmount);
            this.SetItem(tempItem, tempAmount);
        }
        else if (InventorySlotUI.draggedSlotUI is InventorySlotUI invSlot)
        {
            // ±âÁ¸ ½½·Ô ³»¿ë Á¦°Å
            ClearSlot();

            // ½½·Ô¿¡¼­ ¾ÆÀÌÅÛ º¹»ç
            SetItem(InventorySlotUI.draggedItem, 1);
            invSlot.RemoveItemFromSlot();
        }

        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        DragIconUI.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("[QuickSlotUI] OnBeginDrag È£ÃâµÊ");
        Debug.Log("[QuickSlotUI] OnBeginDrag È£ÃâµÊ");
        Debug.Log("[QuickSlotUI] OnBeginDrag È£ÃâµÊ");
        Debug.Log("[QuickSlotUI] OnBeginDrag È£ÃâµÊ");
        Debug.Log("[QuickSlotUI] OnBeginDrag È£ÃâµÊ");
        Debug.Log("[QuickSlotUI] OnBeginDrag È£ÃâµÊ");
        Debug.Log("[QuickSlotUI] OnBeginDrag È£ÃâµÊ");
        Debug.Log("[QuickSlotUI] OnBeginDrag È£ÃâµÊ");
        if (currentItem != null)
        {
            Debug.Log("[QuickSlotUI] µå·¡±× ½ÃÀÛ: " + currentItem.itemName);
            {
                InventorySlotUI.draggedItem = currentItem;
                InventorySlotUI.draggedSlotUI = this;
                draggedQuickSlot = this;
                DragIconUI.Instance.Show(currentItem.icon);
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlotUI.draggedItem = null;
        draggedQuickSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void RefreshSlotUI()
    {
        if (currentItem != null)
        {
            iconImage.sprite = currentItem.icon;
            iconImage.enabled = true;
            countText.text = currentAmount > 1 ? currentAmount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void RemoveItemFromSlot()
    {
        ClearSlot();
    }
}
