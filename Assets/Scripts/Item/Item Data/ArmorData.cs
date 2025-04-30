using UnityEngine;

[CreateAssetMenu(fileName = "NewArmor", menuName = "Inventory/Armor Data")]
public class ArmorData : EquipmentData
{
    [Header("°©¿Ê Àü¿ë ½ºÅÈ")]
    public int physicalDefense;
    public int magicalDefense;
    public int maxHealth;
    public float healthRegenRate;

    [Header("ºÎ°¡ È¿°ú")]
    public int fireResistance;
    public int iceResistance;
    public bool immuneToStatusEffect;
}