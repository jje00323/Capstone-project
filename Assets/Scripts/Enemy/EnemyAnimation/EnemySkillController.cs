using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    [Header("스킬 히트박스 프리팹")]
    public GameObject skillHitboxPrefab;
    public Transform spawnPoint;

    public void SpawnSkillHitbox()
    {
        if (skillHitboxPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("[EnemySkillController] 스킬 히트박스 프리팹 또는 스폰 위치가 없습니다.");
            return;
        }

        GameObject instance = Instantiate(
            skillHitboxPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Hitbox hitbox = instance.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            hitbox.Initialize(transform, false);
            instance.tag = "EnemyHitbox";
        }
    }
}