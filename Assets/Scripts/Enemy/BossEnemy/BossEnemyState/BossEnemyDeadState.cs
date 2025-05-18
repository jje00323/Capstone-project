using UnityEngine;
using System.Collections;

public class BossEnemyDeadState : BossEnemyState
{
    public BossEnemyDeadState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        boss.Animator.SetTrigger("Die");
        boss.Animator.SetBool("IsMoving", false);

        if (boss.TryGetComponent<Collider>(out var col))
            col.enabled = false;

        boss.StartCoroutine(HandleDeathSequence());

        boss.Animator.speed = 0.5f;
    }

    public override void Update() { }

    public override void Exit() { }

    private IEnumerator HandleDeathSequence()
    {
        // 사망 애니메이션 기다리기 (2초라고 가정)
        yield return new WaitForSeconds(4f);

        // 체력 UI 제거
        BossEnemyHealthUI bossUI = GameObject.FindObjectOfType<BossEnemyHealthUI>();
        if (bossUI != null)
        {
            GameObject.Destroy(bossUI.gameObject);
            Debug.Log("[DeadState] 보스 체력 UI 제거됨");
        }


        // 보물상자 소환
        if (boss.bossData != null && boss.bossData.itemDrops != null && boss.bossData.itemDrops.Length > 0)
        {
            Vector3 spawnPosition = new Vector3(0f, 0f, 0f); // 또는 별도 마법진 위치
            Quaternion spawnRotation = Quaternion.identity;

            foreach (GameObject drop in boss.bossData.itemDrops)
            {
                GameObject.Instantiate(drop, spawnPosition, spawnRotation);
                Debug.Log($"[DeadState] 보물상자 또는 드랍 아이템 생성됨: {drop.name}");
            }
        }

        // 4. 보스 오브젝트 제거
        GameObject.Destroy(boss.gameObject);
    }
}
