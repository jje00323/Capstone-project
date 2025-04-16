using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public bool IsEmpty => item == null;

    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    public void SetItem(ItemData newItem, int amount)
    {
        item = newItem;
        quantity = amount;
    }

    public bool CanStackWith(ItemData newItem)
    {
        return item != null &&
               item == newItem &&
               item.isStackable &&
               quantity < item.maxStack;
    }

    public int AddToStack(int amount)
    {
        if (!item.isStackable) return 0;

        int space = item.maxStack - quantity;
        int added = Mathf.Min(space, amount);
        quantity += added;
        return added;
    }
}