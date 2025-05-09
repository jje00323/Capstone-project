using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Helmet,
    Chest,
    Legs,
    Gloves,
    Boots,
    Rune
}

public class EquipmentData : ItemData
{
    [Header("공통 장비 정보")]
    public EquipmentType equipmentType;

    [Header("직업 제한")]
    public List<JobManager.JobType> allowedJobs;

    [Header("스탯 변화")]
    public List<StatModifier> statModifiers = new();

    public virtual List<StatModifier> GetAllStatModifiers()
    {
        return new List<StatModifier>(statModifiers); // 기본값만 반환
    }

}

