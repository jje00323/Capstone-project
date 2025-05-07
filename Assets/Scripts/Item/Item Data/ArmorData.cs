using UnityEngine;

[CreateAssetMenu(fileName = "NewArmor", menuName = "Inventory/Armor Data")]
public class ArmorData : EquipmentData
{
    [Header("°©¿Ê Àü¿ë ½ºÅÈ")]
    public int Defense;
    public int maxHealth;
    

    [Header("ºÎ°¡ È¿°ú")]
    public float healthRegenRate;
}