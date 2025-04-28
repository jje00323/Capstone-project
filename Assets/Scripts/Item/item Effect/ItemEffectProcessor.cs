//using UnityEngine;

//public class ItemEffectProcessor : MonoBehaviour
//{
//    private PlayerStatus player; // 플레이어 상태 관리 컴포넌트 참조

//    private void Awake()
//    {
//        player = FindObjectOfType<PlayerStatus>();
//        if (player == null)
//            Debug.LogError("[ItemEffectProcessor] PlayerStatus를 찾을 수 없습니다.");
//    }

//    public void UseItem(ConsumableData consumable)
//    {
//        if (consumable == null)
//        {
//            Debug.LogWarning("사용할 아이템이 null입니다.");
//            return;
//        }

//        switch (consumable.effectType)
//        {
//            case ConsumableEffectType.HealHP:
//                player.Heal(consumable.amount);
//                break;

//            case ConsumableEffectType.RestoreMana:
//                player.RestoreMana(consumable.amount);
//                break;

//            case ConsumableEffectType.BuffAttack:
//                player.ApplyBuff(PlayerBuffType.Attack, consumable.amount, consumable.duration);
//                break;

//            case ConsumableEffectType.BuffDefense:
//                player.ApplyBuff(PlayerBuffType.Defense, consumable.amount, consumable.duration);
//                break;

//            case ConsumableEffectType.CureStatusEffect:
//                player.CureStatusEffects();
//                break;

//            default:
//                Debug.Log("특수효과가 없는 아이템입니다.");
//                break;
//        }
//    }
//}