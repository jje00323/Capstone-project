using UnityEngine;

[CreateAssetMenu(fileName = "NewRune", menuName = "Inventory/Rune Data")]
public class RuneData : EquipmentData
{
    public int strengthBonus;
    public int agilityBonus;
    public int intelligenceBonus;
    public float moveSpeedBonus;
    public float expBonus;
}