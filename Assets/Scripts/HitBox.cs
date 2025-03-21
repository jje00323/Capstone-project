using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 10f; // 기본 공격력
    public float duration = 0.5f; // 히트박스 지속 시간
    private Collider[] allColliders;

    void Awake()
    {
        allColliders = GetComponents<Collider>();
        if (allColliders != null)
        {
            foreach (var col in allColliders)
            {
                col.isTrigger = true;
            }
        }
    }

    void OnEnable()
    {
        StartCoroutine(DisableAfterDuration());
    }

    IEnumerator DisableAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"적이 공격당함! 데미지: {damage}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // 회전/위치 반영
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
            // Capsule, Mesh 등 다른 타입도 원하면 여기에 추가
        }

        Gizmos.matrix = oldMatrix; // 원래 상태 복구
    }
}