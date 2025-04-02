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

    private void OnTriggerEnter(Collider other)//투사체만 colider사용
    {
        if (!isProjectile || !initialized) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"[Hitbox Projectile] 적 히트! 데미지: {damage}");
            }

            Destroy(gameObject); // 투사체는 맞고 사라짐
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
        if (!initialized || caster == null) return;

        Vector3 center = caster.position + caster.TransformDirection(offset);
        Collider[] hits;

        switch (shape)
        {
            case ShapeType.Sphere:
                hits = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Enemy"));
                break;

            case ShapeType.Box:
                hits = Physics.OverlapBox(center, boxSize * 0.5f, caster.rotation, LayerMask.GetMask("Enemy"));
                break;

            case ShapeType.Cone:
                hits = Physics.OverlapSphere(center, coneDistance, LayerMask.GetMask("Enemy"));
                List<Collider> coneHits = new List<Collider>();
                foreach (var col in hits)
                {
                    Vector3 dirToTarget = (col.transform.position - caster.position).normalized;
                    float angle = Vector3.Angle(caster.forward, dirToTarget);
                    float distance = Vector3.Distance(caster.position, col.transform.position);

                    //  아주 가까운 적은 방향 무시하고 무조건 포함
                    if (distance <= 2f || angle < coneAngle * 0.7f)
                    {
                        coneHits.Add(col);
                    }
                }
                hits = coneHits.ToArray();
                break;

            default:
                hits = new Collider[0];
                break;
        }

        foreach (Collider col in hits)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"[Hitbox] 적 히트! 데미지: {damage}");
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