using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    public enum ShapeType { Sphere, Box, Cone }

    [Header("히트박스 설정")]
    public float damage = 10f;
    public float duration = 1f;
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

    [Header("이펙트 설정")]
    public GameObject effectPrefab;
    public float effectDuration = 2f;

    [Header("동작 설정")]
    public bool followCaster = false;

    [Header("디버그")]
    public bool drawGizmos = true;
    public Color gizmoColor = Color.red;

    private Transform caster;
    private bool initialized = false;
    private bool isHitboxActive = false;

    public void Initialize(Transform casterTransform, bool shouldFollow)
    {
        caster = casterTransform;
        followCaster = shouldFollow;
        initialized = true;

        transform.position = caster.position + caster.TransformDirection(offset);
        transform.rotation = caster.rotation;

        if (effectPrefab != null)
        {
            GameObject vfx = Instantiate(effectPrefab, transform.position, transform.rotation);
            if (followCaster)
                vfx.transform.SetParent(transform);
            Destroy(vfx, effectDuration);
        }

        StartCoroutine(HandleHitbox());
    }

    private void Update()
    {
        if (!initialized || caster == null || !followCaster) return;

        transform.position = caster.position + caster.TransformDirection(offset);
        transform.rotation = caster.rotation;
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
        Vector3 center = transform.position + transform.TransformDirection(offset);
        Collider[] hits = null;
        List<Collider> filteredHits = new List<Collider>();
        LayerMask targetLayer = LayerMask.GetMask("Player");

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
            if (col.CompareTag("Player"))
            {
                var player = col.GetComponent<PlayerStatus>();
                if (player != null) player.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (!Application.isPlaying && !isHitboxActive) return;

        Vector3 center = transform.position + transform.TransformDirection(offset);

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