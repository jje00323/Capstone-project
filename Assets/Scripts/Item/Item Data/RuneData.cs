using UnityEngine;

[CreateAssetMenu(fileName = "NewRune", menuName = "Inventory/Rune Data")]
public class RuneData : ItemData
{
    [Header("·é½ºÅæ °­È­ È¿°ú")]
    public int strengthBonus;
    public int agilityBonus;
    public int intelligenceBonus;
    public float moveSpeedBonus;
    public float expBonus;
}