using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("히트박스 설정")]
    public float damage = 10f;                   // 데미지
    public float duration = 1f;                  // 마지막 반복 후 제거까지 시간

    [Header("반복 생성 설정")]
    public float startDelay = 0f;                // 첫 생성까지 대기 시간
    public int repeatCount = 1;                  // 몇 번 반복할지
    public float repeatInterval = 0.5f;          // 반복 간격

    [Header("투사체 설정")]
    public bool isProjectile = false;            // 날아가는 형태인지
    public float projectileSpeed = 10f;          // 투사체 속도

    private Collider[] allColliders;
    private bool hasLaunched = false;

    void Awake()
    {
        allColliders = GetComponents<Collider>();
        DisableHit();

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
        DisableHit(); //  처음엔 충돌 못 하도록 비활성화

        if (isProjectile)
        {
            TriggerProjectile(); // 투사체는 한 번만 날아감
        }
        else
        {
            StartCoroutine(HandleSpawnSequence()); // 반복형 히트박스는 껐다 켰다 반복
        }
    }

    private IEnumerator HandleSpawnSequence()
    {
        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < repeatCount; i++)
        {
            EnableHit();
            yield return new WaitForSeconds(0.1f); // 잠깐 충돌 허용
            DisableHit();

            if (i < repeatCount - 1)
                yield return new WaitForSeconds(repeatInterval);
        }

        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void EnableHit()
    {
        foreach (var col in allColliders)
            col.enabled = true;
    }

    private void DisableHit()
    {
        foreach (var col in allColliders)
            col.enabled = false;
    }



    private void TriggerProjectile()
    {

        if (hasLaunched) return;

        hasLaunched = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * projectileSpeed;
        }

        EnableHit();

        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($" 적이 공격당함! 데미지: {damage}");
            }

            if (isProjectile)
            {
                Destroy(gameObject); // 투사체는 충돌 시 제거
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // collider가 꺼져있으면 Gizmo도 그리지 않음
        foreach (var col in GetComponents<Collider>())
        {
            if (!col.enabled) continue;

            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider box)
                Gizmos.DrawWireCube(box.center, box.size);
            else if (col is SphereCollider sphere)
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}