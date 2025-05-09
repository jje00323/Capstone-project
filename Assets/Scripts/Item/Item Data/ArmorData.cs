using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArmor", menuName = "Inventory/Armor Data")]
public class ArmorData : EquipmentData
{
    [Header("°©¿Ê Àü¿ë ½ºÅÈ")]
    public int Defense;
    public int MaxHP;
    

    [Header("ºÎ°¡ È¿°ú")]
    public float HealthRegen;

    public override List<StatModifier> GetAllStatModifiers()
    {
        var all = base.GetAllStatModifiers();
        all.Add(new StatModifier(StatType.Defense, Defense, true));
        all.Add(new StatModifier(StatType.MaxHP, MaxHP, true));
        all.Add(new StatModifier(StatType.HealthRegen, HealthRegen, true));
        return all;
    }
}