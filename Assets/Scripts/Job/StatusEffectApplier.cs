using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    MaxHP,
    CurrentHP,
    MaxMP,
    CurrentMP,
    Attack,
    Defense,
    MoveSpeed,
    CritRate,
    CritDamage,
    ExpBonus,
    HealthRegen,
    ManaRegen
}

public struct StatModifier
{
    public StatType type;
    public float value;
    public bool isFlat;

    public StatModifier(StatType type, float value, bool isFlat = true)
    {
        this.type = type;
        this.value = value;
        this.isFlat = isFlat;
    }
}

public static class StatusEffectApplier
{
    public static void ApplyStatModifiers(CharacterStatus target, List<StatModifier> modifiers)
    {
        foreach (var mod in modifiers)
        {
            ApplyStatModifier(target, mod);
        }

        if (target is PlayerStatus player)
            player.UpdateAllUI(); // 플레이어만 UI 갱신
    }

    public static void ApplyStatModifier(CharacterStatus target, StatModifier mod)
    {
        switch (mod.type)
        {
            case StatType.MaxHP:
                target.maxHP += mod.value;
                break;
            case StatType.CurrentHP:
                target.currentHP = Mathf.Clamp(target.currentHP + mod.value, 0, target.maxHP);
                break;
            case StatType.Attack:
                target.attack += mod.value;
                break;
            case StatType.Defense:
                target.defense += mod.value;
                break;
            // MaxMP, CurrentMP는 PlayerStatus일 때만 적용
            case StatType.MaxMP:
            case StatType.CurrentMP:
                if (target is PlayerStatus p)
                {
                    if (mod.type == StatType.MaxMP)
                        p.maxMP += mod.value;
                    else
                        p.currentMP = Mathf.Clamp(p.currentMP + mod.value, 0, p.maxMP);
                }
                break;
        }
    }
}