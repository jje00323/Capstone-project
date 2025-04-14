using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("히트박스 프리팹")]
    public GameObject attackHitboxPrefab;

    public void SpawnAttackHitbox()
    {
        if (attackHitboxPrefab == null) return;

        GameObject instance = Instantiate(attackHitboxPrefab);
        Hitbox hitbox = instance.GetComponent<Hitbox>();

        if (hitbox != null)
        {
            hitbox.Initialize(transform, true); // SpawnPoint가 아닌 enemy.transform 기준
            instance.tag = "EnemyHitbox";
        }
    }
}
