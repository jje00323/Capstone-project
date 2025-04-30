using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor
}

public class EquipmentData : ItemData
{
    [Header("공통 장비 정보")]
    public EquipmentType equipmentType;
}