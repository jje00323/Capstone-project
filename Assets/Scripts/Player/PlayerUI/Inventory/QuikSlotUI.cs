using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class QuickSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("슬롯 설정")]
    [SerializeField] private int slotIndex;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("입력 액션 연결")]
    [SerializeField] private InputActionReference useSlotAction;

    private ItemData currentItem;
    private int currentAmount = 0;

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
        RefreshSlotUI();
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
        if (currentItem is ConsumableData consumable)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null && player.TryGetComponent(out PlayerStatus playerStatus))
            {
                consumable.ApplyEffect(playerStatus);
            }

            currentAmount--;
            if (currentAmount <= 0)
                ClearSlot();
            else
                countText.text = currentAmount.ToString();
        }
    }

    public ItemData GetItem() => currentItem;
    public int GetAmount() => currentAmount;

    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySlotUI.draggedItem == null || InventorySlotUI.draggedSlotUI == null) return;
        if (InventorySlotUI.draggedItem.itemType != ItemType.Consumable) return;

        if (InventorySlotUI.draggedSlotUI is QuickSlotUI draggedQuick)
        {
            // QuickSlot <-> QuickSlot 교환
            var tempItem = currentItem;
            var tempAmount = currentAmount;

            SetItem(draggedQuick.currentItem, draggedQuick.currentAmount);
            draggedQuick.SetItem(tempItem, tempAmount);
        }
        else if (InventorySlotUI.draggedSlotUI is InventorySlotUI draggedInv)
        {
            var sourceSlot = draggedInv.GetSlotData();
            if (sourceSlot == null || sourceSlot.item == null) return;

            // 인벤토리 → 퀵슬롯: 복사해서 소유
            SetItem(sourceSlot.item, sourceSlot.quantity);
            draggedInv.RemoveItemFromSlot();
        }

        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedQuickSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        InventorySlotUI.draggedItem = currentItem;
        InventorySlotUI.draggedSlotUI = this;
        draggedQuickSlot = this;
        DragIconUI.Instance.Show(currentItem.icon);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
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