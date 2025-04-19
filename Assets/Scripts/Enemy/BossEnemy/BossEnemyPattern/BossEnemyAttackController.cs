using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttackController : MonoBehaviour
{
    [Header("공격 스킬 프리팹")]
    public GameObject slashPrefab;
    public GameObject stompPrefab;
    public GameObject spinPrefab;

    public void UseSlashAttack()
    {
        SpawnHitbox(slashPrefab);
    }

    public void UseStompAttack()
    {
        SpawnHitbox(stompPrefab);
    }

    public void UseSpinAttack()
    {
        SpawnHitbox(spinPrefab);
    }

    private void SpawnHitbox(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject instance = Instantiate(prefab, transform.position, transform.rotation);
        Hitbox hitbox = instance.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            hitbox.Initialize(transform, true);
        }
    }
}