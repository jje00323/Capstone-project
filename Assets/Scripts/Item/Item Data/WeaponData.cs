using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon Data")]
public class WeaponData : EquipmentData
{
    [Header("무기 전용 스탯")]
    public int attackPower;
    public int manaAmount;
    

    [Header("부가 효과")]
    public float manaRegenRate;
    public float CritRate;
    public float CritDamage;
}