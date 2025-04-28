using UnityEngine;

public enum ConsumableEffectType
{
    None,
    HealHP,
    RestoreMana,
    BuffAttack,
    BuffDefense,
    CureStatusEffect
}

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable Data")]
public class ConsumableData : ItemData
{
    [Header("소모품 효과")]
    public ConsumableEffectType effectType; // 추가된 부분
    public int amount; // 회복량 또는 버프 수치
    public float duration; // 버프 지속 시간(필요할 경우)
}

