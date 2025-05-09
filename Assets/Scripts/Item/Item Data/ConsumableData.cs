using UnityEngine;

public enum ConsumableEffectType
{
    None,
    HealHP,
    RestoreMana,
    BuffAttack,
    BuffDefense,
    RegenHP,       // 초당 체력 회복
    RegenMP        // 초당 마나 회복
}

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable Data")]
public class ConsumableData : ItemData
{
    [Header("소모품 효과")]
    public ConsumableEffectType effectType;
    public int amount;
    public float duration;

    /// <summary>
    /// 효과 적용
    /// </summary>
    public void ApplyEffect(PlayerStatus playerStatus)
    {
        switch (effectType)
        {
            case ConsumableEffectType.HealHP:
                playerStatus.Heal(amount);
                break;

            case ConsumableEffectType.RestoreMana:
                playerStatus.UseMana(-amount);
                break;

            case ConsumableEffectType.BuffAttack:
                playerStatus.ApplyTemporaryModifier(new StatModifier(StatType.Attack, amount, true), duration);
                break;

            case ConsumableEffectType.BuffDefense:
                playerStatus.ApplyTemporaryModifier(new StatModifier(StatType.Defense, amount, true), duration);
                break;

            case ConsumableEffectType.RegenHP:
                playerStatus.StartHPRegen(amount, duration);
                break;

            case ConsumableEffectType.RegenMP:
                playerStatus.StartMPRegen(amount, duration);
                break;

            default:
                Debug.LogWarning($"[Consumable] {effectType} 효과 미구현");
                break;
        }
    }


}