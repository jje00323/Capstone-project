using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public enum ShapeType { Sphere, Box, Cone }

    [Header("히트박스 설정")]
    public float damage = 10f;
    public float duration = 1f;
    public bool followCaster = true;

    [Header("반복 판정 설정")]
    public float startDelay = 0f;
    public int repeatCount = 1;
    public float repeatInterval = 0.5f;

    [Header("범위 설정")]
    public ShapeType shape = ShapeType.Sphere;
    public float radius = 2f; // Sphere / Cone
    public Vector3 boxSize = new Vector3(2f, 2f, 2f);
    public float coneAngle = 45f; // degrees
    public float coneDistance = 3f;
    public Vector3 offset = Vector3.forward;

    [Header("투사체 설정")]
    public bool isProjectile = false;
    public float projectileSpeed = 10f;
    public Rigidbody projectileRigidbody;

    [Header("디버그용")]
    public bool drawGizmos = true;

    private bool initialized = false;
    public Color gizmoColor = Color.red;
    public Transform caster;
    private bool isHitboxActive = false;


    private void Update()
    {
        if (followCaster && caster != null)
        {
            transform.position = caster.position + caster.TransformDirection(offset);
        }

    }
    public void Initialize(Transform casterTransform)
    {
        caster = casterTransform;
        initialized = true;

        if (isProjectile)
        {
            LaunchProjectile();
        }
        else
        {
            StartCoroutine(HandleHitbox());
        }
    }

    private void LaunchProjectile()
    {
        if (projectileRigidbody == null)
        {
            projectileRigidbody = GetComponent<Rigidbody>();
        }

        if (projectileRigidbody != null)
        {
            projectileRigidbody.velocity = transform.forward * projectileSpeed;
        }

        // 간단한 단일 판정 후 일정 시간 뒤 제거
        StartCoroutine(DestroyAfterDuration());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProjectile || !initialized) return;

        // 히트박스가 플레이어 공격이면 적에게 적용
        if (CompareTag("PlayerHitbox") && other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"[Hitbox Projectile] 적 피격! 데미지: {damage}");
            }

            Destroy(gameObject);
        }
        // 히트박스가 몬스터 공격이면 플레이어에게 적용
        else if (CompareTag("EnemyHitbox") && other.CompareTag("Player"))
        {
            PlayerStatus player = other.GetComponent<PlayerStatus>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"[Hitbox Projectile] 플레이어 피격! 데미지: {damage}");
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private IEnumerator HandleHitbox()
    {
        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i < repeatCount; i++)
        {
            isHitboxActive = true;        //  활성화 시작
            ApplyDamage();
            yield return new WaitForSeconds(0.1f); // 판정 지속 시간 (디버깅용)
            isHitboxActive = false;       //  비활성화

            if (i < repeatCount - 1)
                yield return new WaitForSeconds(repeatInterval);
        }

        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void ApplyDamage()
    {
        if (!initialized || caster == null)
        {
            Debug.LogWarning("[HitBox] 초기화되지 않았거나 Caster 없음");
            return;
        }

        Debug.Log($"[HitBox] {gameObject.name}, Tag: {gameObject.tag}, Layer: {LayerMask.LayerToName(gameObject.layer)}");

        Vector3 center = caster.position + caster.TransformDirection(offset);
        Collider[] hits;

        string targetLayer = gameObject.CompareTag("PlayerHitbox") ? "Enemy" : "Player";
        Debug.Log($"[HitBox] 타겟 레이어: {targetLayer}");

        List<Collider> filteredHits = new List<Collider>();

        switch (shape)
        {
            case ShapeType.Sphere:
                hits = Physics.OverlapSphere(center, radius, LayerMask.GetMask(targetLayer));
                foreach (var col in hits)
                {
                    Vector3 dirToTarget = (col.transform.position - caster.position).normalized;
                    float angle = Vector3.Angle(caster.forward, dirToTarget);
                    float distance = Vector3.Distance(caster.position, col.transform.position);

                    Debug.Log($"[Sphere] 대상: {col.name}, 거리: {distance}, 각도: {angle}");

                    // 필터링 완화: 반경 이내 + 전방 120도 이내 허용
                    if (distance <= radius && angle < 120f)
                    {
                        filteredHits.Add(col);
                        Debug.Log($"[Sphere] 감지됨 → {col.name}");
                    }
                }
                break;

            case ShapeType.Box:
                hits = Physics.OverlapBox(center, boxSize * 0.5f, caster.rotation, LayerMask.GetMask(targetLayer));
                foreach (var col in hits)
                {
                    Vector3 dirToTarget = (col.transform.position - caster.position).normalized;
                    float angle = Vector3.Angle(caster.forward, dirToTarget);
                    float distance = Vector3.Distance(caster.position, col.transform.position);

                    Debug.Log($"[Box] 대상: {col.name}, 거리: {distance}, 각도: {angle}");

                    if (distance <= radius && angle < 120f)
                    {
                        filteredHits.Add(col);
                        Debug.Log($"[Box] 감지됨 → {col.name}");
                    }
                }
                break;

            case ShapeType.Cone:
                hits = Physics.OverlapSphere(center, coneDistance, LayerMask.GetMask(targetLayer));
                foreach (var col in hits)
                {
                    Vector3 dirToTarget = (col.transform.position - caster.position).normalized;
                    float angle = Vector3.Angle(caster.forward, dirToTarget);
                    float distance = Vector3.Distance(caster.position, col.transform.position);

                    Debug.Log($"[Cone] 대상: {col.name}, 거리: {distance}, 각도: {angle}");

                    if (distance <= 2f || angle < coneAngle * 0.7f)
                    {
                        filteredHits.Add(col);
                        Debug.Log($"[Cone] 감지됨 → {col.name}");
                    }
                }
                break;

            default:
                hits = new Collider[0];
                break;
        }

        // 데미지 처리
        foreach (var col in filteredHits)
        {
            if (gameObject.CompareTag("PlayerHitbox") && col.CompareTag("Enemy"))
            {
                var enemy = col.GetComponent<EnemyController>();
                if (enemy != null) enemy.TakeDamage(damage);
            }
            else if (gameObject.CompareTag("EnemyHitbox") && col.CompareTag("Player"))
            {
                var player = col.GetComponent<PlayerStatus>();
                if (player != null) player.TakeDamage(damage);
                Debug.Log("[HitBox] 플레이어에게 데미지 적용 완료");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        //  투사체가 아닌 일반 히트박스는 판정 중일 때만 보여줌
        if (!isProjectile && Application.isPlaying && !isHitboxActive) return;


        Vector3 center;

        if (Application.isPlaying && caster != null && followCaster)
        {
            center = caster.position + caster.TransformDirection(offset);
        }
        else
        {
            center = transform.position + transform.TransformDirection(offset);
        }

        Gizmos.color = gizmoColor;

        switch (shape)
        {
            case ShapeType.Sphere:
                Gizmos.DrawWireSphere(center, radius);
                break;

            case ShapeType.Box:
                Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, boxSize);
                Gizmos.matrix = Matrix4x4.identity;
                break;

            case ShapeType.Cone:
                Gizmos.DrawRay(center, transform.forward * coneDistance);
                Vector3 right = Quaternion.Euler(0, coneAngle * 0.5f, 0) * transform.forward;
                Vector3 left = Quaternion.Euler(0, -coneAngle * 0.5f, 0) * transform.forward;
                Gizmos.DrawRay(center, right * coneDistance);
                Gizmos.DrawRay(center, left * coneDistance);
                break;
        }
    }


}