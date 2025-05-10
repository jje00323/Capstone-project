using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttackController : MonoBehaviour
{
    [Header("히트박스 프리팹 (AttackIndex 순서대로 연결)")]
    public GameObject[] attackPrefabs = new GameObject[8];

    private BossEnemyFSM bossFSM;

    private void Awake()
    {
        bossFSM = GetComponent<BossEnemyFSM>();
        if (bossFSM == null)
        {
            Debug.LogError("[BossEnemyAttackController] BossEnemyFSM 컴포넌트를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출. 현재 AttackIndex에 맞는 히트박스를 생성함.
    /// </summary>
    public void SpawnAttackByIndex()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
        {
            Debug.LogWarning("[BossEnemyAttackController] bossFSM 또는 bossStatus가 null입니다.");
            return;
        }

        int index = bossFSM.bossStatus.lastAttackIndex;

        if (index < 0 || index >= attackPrefabs.Length)
        {
            Debug.LogWarning($"[BossEnemyAttackController] 유효하지 않은 AttackIndex: {index}");
            return;
        }

        GameObject prefab = attackPrefabs[index];
        if (prefab == null)
        {
            Debug.LogWarning($"[BossEnemyAttackController] AttackIndex {index}에 연결된 히트박스 프리팹이 없습니다.");
            return;
        }

        GameObject instance = Instantiate(prefab, transform.position, transform.rotation);
        instance.tag = "EnemyHitbox";

        Hitbox hitbox = instance.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            hitbox.Initialize(transform, true);
        }
        else
        {
            Debug.LogWarning("[BossEnemyAttackController] 히트박스 프리팹에 Hitbox 컴포넌트가 없습니다.");
        }
    }
}
