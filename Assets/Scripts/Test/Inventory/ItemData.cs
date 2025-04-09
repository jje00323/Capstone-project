using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
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

    [Header("장비 전용 (선택)")]
    public GameObject equipPrefab; // 무기나 방어구 장착 시 사용될 프리팹
}

public enum EquipmentType { Weapon, Armor }

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment Data")]
public class EquipmentData : ItemData
{
    public EquipmentType equipmentType;

    [Header("스탯 정보")]
    public int attackPower;
    public int defensePower;
    public float attackSpeed;
    public float criticalChance;
}
