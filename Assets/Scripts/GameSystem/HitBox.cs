using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public enum ShapeType { Sphere, Box, Cone }

    [Header("히트박스 설정")]
    public float damage = 10f;
    public float duration = 1f;

    [Header("스킬 사거리")]
    public float castRange = 10f;

    [Header("반복 판정 설정")]
    public float startDelay = 0f;
    public int repeatCount = 1;
    public float repeatInterval = 0.5f;

    [Header("범위 설정")]
    public ShapeType shape = ShapeType.Sphere;
    public float radius = 2f;
    public Vector3 boxSize = new Vector3(2f, 2f, 2f);
    public float coneAngle = 45f;
    public float coneDistance = 3f;
    public Vector3 offset = Vector3.forward;

    [Header("투사체 설정")]
    public bool isProjectile = false;
    public float projectileSpeed = 10f;
    public Rigidbody projectileRigidbody;

    [Header("위치 설정")]
    public bool useMousePosition = false;                  //  마우스 위치로 생성할지 여부
    public LayerMask mouseGroundMask = ~0;                 //  마우스 위치 탐지 레이어 (-1은 모든 레이어)

    [Header("디버그")]
    public bool drawGizmos = true;
    public Color gizmoColor = Color.red;

    [SerializeField] private bool followCaster = false;

    private Transform caster;
    private bool initialized = false;
    private bool isHitboxActive = false;

    public void Initialize(Transform casterTransform, bool shouldFollow, Vector3? overrideMousePos = null)
    {
        caster = casterTransform;
        followCaster = shouldFollow;
        initialized = true;

        if (useMousePosition)
        {
            Vector3 spawnPos = overrideMousePos ?? MouseUtility.GetMouseWorldPosition(mouseGroundMask) ?? caster.position;
            transform.position = spawnPos;
            transform.rotation = MouseUtility.GetLookRotationTo(caster.position, spawnPos);
            followCaster = false;
        }
        else
        {
            transform.position = caster.position + caster.TransformDirection(offset);
            transform.rotation = caster.rotation;
        }

        if (isProjectile)
            LaunchProjectile();
        else
            StartCoroutine(HandleHitbox());
    }

    private void Update()
    {
        if (!initialized || caster == null || !followCaster) return;

        transform.position = caster.position + caster.TransformDirection(offset);
        transform.rotation = caster.rotation;
    }

    private void LaunchProjectile()
    {
        if (projectileRigidbody == null)
            projectileRigidbody = GetComponent<Rigidbody>();

        if (projectileRigidbody != null)
            projectileRigidbody.velocity = transform.forward * projectileSpeed;

        StartCoroutine(DestroyAfterDuration());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isProjectile || !initialized) return;

        if (CompareTag("PlayerHitbox") && other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyStatus>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"[Hitbox Projectile] 적 피격! 데미지: {damage}");
            }

            Destroy(gameObject);
        }
        else if (CompareTag("EnemyHitbox") && other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerStatus>();
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
            isHitboxActive = true;
            ApplyDamage();
            yield return new WaitForSeconds(0.1f);
            isHitboxActive = false;

            if (i < repeatCount - 1)
                yield return new WaitForSeconds(repeatInterval);
        }

        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void ApplyDamage()
    {
        if (!initialized)
        {
            Debug.LogWarning("[HitBox] 초기화되지 않음");
            return;
        }

        Vector3 center = followCaster && caster != null
            ? caster.position + caster.TransformDirection(offset)
            : transform.position + transform.TransformDirection(offset);

        Collider[] hits = null;
        List<Collider> filteredHits = new List<Collider>();
        LayerMask targetLayer = GetTargetLayerMask();

        switch (shape)
        {
            case ShapeType.Sphere:
                hits = Physics.OverlapSphere(center, radius, targetLayer);
                filteredHits.AddRange(hits);
                break;

            case ShapeType.Box:
                hits = Physics.OverlapBox(center, boxSize * 0.5f, transform.rotation, targetLayer);
                filteredHits.AddRange(hits);
                break;

            case ShapeType.Cone:
                hits = Physics.OverlapSphere(center, coneDistance, targetLayer);
                foreach (var col in hits)
                {
                    Vector3 dirToTarget = (col.transform.position - center).normalized;
                    float angle = Vector3.Angle(transform.forward, dirToTarget);
                    float distance = Vector3.Distance(center, col.transform.position);

                    if (distance <= 2f || angle < coneAngle * 0.7f)
                        filteredHits.Add(col);
                }
                break;
        }

        foreach (var col in filteredHits)
        {
            if (CompareTag("PlayerHitbox") && col.CompareTag("Enemy"))
            {
                var enemy = col.GetComponent<EnemyStatus>();
                if (enemy != null) enemy.TakeDamage(damage);
            }
            else if (CompareTag("EnemyHitbox") && col.CompareTag("Player"))
            {
                var player = col.GetComponent<PlayerStatus>();
                if (player != null) player.TakeDamage(damage);
            }
        }
    }

    private LayerMask GetTargetLayerMask()
    {
        if (CompareTag("PlayerHitbox"))
            return LayerMask.GetMask("Enemy", "EnemyIgnorePlayer");
        else if (CompareTag("EnemyHitbox"))
            return LayerMask.GetMask("Player", "PlayerIgnoreEnemy");

        return 0;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (!isProjectile && Application.isPlaying && !isHitboxActive) return;

        Vector3 center;

        if (Application.isPlaying && caster != null && followCaster)
            center = caster.position + caster.TransformDirection(offset);
        else
            center = transform.position + transform.TransformDirection(offset);

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

    // 마우스 관련 유틸리티 (필요시 별도 클래스 분리 가능)
    public static class MouseUtility
    {
        public static Vector3? GetMouseWorldPosition(LayerMask groundMask)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
                return hit.point;
            return null;
        }

        public static Quaternion GetLookRotationTo(Vector3 origin, Vector3 target)
        {
            Vector3 dir = (target - origin).normalized;
            dir.y = 0f;
            return dir != Vector3.zero ? Quaternion.LookRotation(dir) : Quaternion.identity;
        }
    }
}