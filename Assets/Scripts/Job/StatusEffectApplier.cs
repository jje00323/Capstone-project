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
    CritRate,
    CritDamage,
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

            case StatType.MaxMP:
                if (target is PlayerStatus mpTarget1)
                    mpTarget1.maxMP += mod.value;
                break;
            case StatType.CurrentMP:
                if (target is PlayerStatus mpTarget2)
                    mpTarget2.currentMP = Mathf.Clamp(mpTarget2.currentMP + mod.value, 0, mpTarget2.maxMP);
                break;

            case StatType.CritRate:
                if (target is PlayerStatus critTarget1)
                    critTarget1.critRate += mod.value;
                break;
            case StatType.CritDamage:
                if (target is PlayerStatus critTarget2)
                    critTarget2.critDamage += mod.value;
                break;

            case StatType.HealthRegen:
            case StatType.ManaRegen:
                Debug.Log("[TODO] Regen 스탯은 별도 시스템으로 관리 필요");
                break;
        }
        Debug.Log($"[ApplyStatModifier] {mod.type} {(mod.value >= 0 ? "+" : "")}{mod.value}");
    }
}