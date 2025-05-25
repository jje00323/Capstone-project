using UnityEditor.Rendering;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
    

    public EnemyDeadState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
       

        // Rigidbody 물리 해제 (땅에 꺼지는 현상 방지)
        if (enemy.Rigidbody != null)
        {
            enemy.Rigidbody.useGravity = false;
            enemy.Rigidbody.velocity = Vector3.zero;
            enemy.Rigidbody.isKinematic = true;
        }

        // 애니메이션 재생
        if (enemy.Animator != null)
        {
            enemy.Animator.SetTrigger("Die");
        }

        // 충돌 유지 시간 확보
        if (enemy.Collider != null)
        {
            enemy.Collider.enabled = true;
        }

        

        if (!string.IsNullOrEmpty(enemy.enemyData.enemyTag))
        {
            QuestManager.Instance.UpdateCondition("KillEnemy", enemy.enemyData.enemyTag);
            Debug.Log($"[EnemyDeadState] 퀘스트 조건 갱신: {enemy.enemyData.enemyTag}");
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            PlayerStatus playerStatus = playerObj.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.GainEXP(enemy.enemyData.expDrop);
                Debug.Log($"[EnemyDeadState] 경험치 {enemy.enemyData.expDrop} 지급됨");
            }
            else
            {
                Debug.LogWarning("[EnemyDeadState] PlayerStatus 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("[EnemyDeadState] Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
        TryDropItems();

        enemy.coroutineRunner.StartCoroutine(DelayDisappear());

    }

    private void TryDropItems()
    {
        var dropList = enemy.enemyData.dropItems;
        if (dropList == null || dropList.Count == 0) return;

        foreach (var drop in dropList)
        {
            if (drop.itemPrefab == null) continue;

            float roll = Random.Range(0f, 100f);
            if (roll <= drop.dropChance)
            {
                Vector2 offset2D = Random.insideUnitCircle * 1.5f;
                Vector3 dropPos = enemy.transform.position + new Vector3(offset2D.x, 0.5f, offset2D.y);

                GameObject item = GameObject.Instantiate(drop.itemPrefab, dropPos, Quaternion.identity);

                if (drop.dropEffectPrefab != null)
                    GameObject.Instantiate(drop.dropEffectPrefab, dropPos, Quaternion.identity);

                

                Debug.Log($"[Drop] {item.name} 드롭됨 (확률: {drop.dropChance}%)");
            }
        }
    }

    

    private Color GetColorByRarity(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Normal: return Color.white;
            case ItemRarity.Rare: return Color.blue;
            case ItemRarity.Epic: return Color.magenta;
            default: return Color.gray;
        }
    }

    public override void Update()
    {
        
        
    }

    public override void Exit()
    {
        
    }

    private System.Collections.IEnumerator DelayDisappear()
    {
        yield return new WaitForSeconds(3f);

        // 충돌 제거
        if (enemy.Collider != null)
        {
            enemy.Collider.enabled = false;
        }

        // 모델 렌더링 제거
        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            rend.enabled = false;
        }

        // 필요 시, 오브젝트 자체 제거 (생략 가능)
         GameObject.Destroy(enemy.gameObject);
    }
}
