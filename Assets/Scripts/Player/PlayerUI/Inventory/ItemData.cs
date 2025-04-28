using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Rune,
    Consumable,
    Material,
    Quest,
    Etc
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemType itemType;

    [Header("스택 설정")]
    public bool isStackable = false;
    public int maxStack = 1;
}