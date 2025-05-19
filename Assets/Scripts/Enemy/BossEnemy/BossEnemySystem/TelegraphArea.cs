using System.Collections;
using UnityEngine;

public class TelegraphArea : MonoBehaviour
{
    [Header("기본 정보")]
    public BossHitbox.ShapeType shape = BossHitbox.ShapeType.Cone;
    public Vector3 offset = Vector3.zero;
    public float growTime = 1.5f;
    public float duration = 3f;

    [Header("동적 스케일")]
    public bool autoScaleFromHitbox = true;
    public GameObject hitboxPrefab;

    private Vector3 finalScale = Vector3.one;
    //private Transform caster;
    private float time = 0f;

    private bool initializedExternally;

    //public void Initialize(Vector3 spawnPos, Quaternion rotation, Transform casterTransform, bool isJumpAttack = false)
    //{
    //    initializedExternally = true;

    //    Vector3 offsetXZ = new Vector3(offset.x, 0f, offset.z);
    //    Vector3 worldXZ = rotation * offsetXZ;
    //    float y = offset.y;

    //    transform.position = spawnPos + worldXZ + new Vector3(0f, y, 0f);
    //    transform.rotation = rotation;

    //    if (autoScaleFromHitbox && hitboxPrefab != null)
    //    {
    //        BossHitbox hit = hitboxPrefab.GetComponent<BossHitbox>();
    //        if (hit != null)
    //        {
    //            switch (hit.shape)
    //            {
    //                case BossHitbox.ShapeType.Sphere:
    //                    finalScale = Vector3.one * hit.radius;
    //                    break;
    //                case BossHitbox.ShapeType.Box:
    //                    finalScale = hit.boxSize;
    //                    break;
    //                case BossHitbox.ShapeType.Cone:
    //                    finalScale = new Vector3(
    //                        hit.coneDistance,
    //                        1f,
    //                        hit.coneDistance
    //                    );
    //                    break;
    //            }
    //        }
    //    }
    //}
    public void Initialize(Vector3 spawnPos, Quaternion rotation, Transform casterTransform, bool isJumpAttack = false)
    {
        initializedExternally = true;

        Vector3 offsetXZ = new Vector3(offset.x, 0f, offset.z);
        Vector3 worldXZ = rotation * offsetXZ;
        float y = offset.y;

        transform.position = spawnPos + worldXZ + new Vector3(0f, y, 0f);
        transform.rotation = rotation;

        transform.localScale = Vector3.zero;

        Debug.Log($"[TelegraphArea] Initialized at: {transform.position}, offset 적용 후");
        Debug.Log($"[TelegraphArea] Caster: {casterTransform.name}, Offset: {offset}, Shape: {shape}");

        if (autoScaleFromHitbox && hitboxPrefab != null)
        {
            BossHitbox hit = hitboxPrefab.GetComponent<BossHitbox>();
            if (hit != null)
            {
                switch (hit.shape)
                {
                    case BossHitbox.ShapeType.Sphere:
                        finalScale = Vector3.one * hit.radius;
                        break;
                    case BossHitbox.ShapeType.Box:
                        finalScale = hit.boxSize;
                        break;
                    case BossHitbox.ShapeType.Cone:
                        finalScale = new Vector3(
                            hit.coneDistance,
                            1f,
                            hit.coneDistance
                        );
                        break;
                }
            }
        }
    }

    private void Update()
    {
        if (!initializedExternally) return;

        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / growTime);
        transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, t);

        if (time > duration)
            Destroy(gameObject);
    }
}