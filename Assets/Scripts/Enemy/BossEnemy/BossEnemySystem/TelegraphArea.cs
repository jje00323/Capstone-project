using UnityEngine;

public enum ShapeType { None, Cone, Circle, Box, Projectile }

public class TelegraphArea : MonoBehaviour
{
    [Header("기본 정보")]
    public ShapeType shape = ShapeType.Cone;
    public Vector3 offset = Vector3.zero;
    public float growTime = 1.5f;
    public float duration = 3f;

    [Header("동적 스케일")]
    public bool autoScaleFromHitbox = true;
    public GameObject hitboxPrefab;

    private Vector3 finalScale = Vector3.one;
    private Transform caster;
    private float time = 0f;

    public void Initialize(Vector3 spawnPos, Quaternion rotation, Transform casterTransform, bool isJumpAttack = false)
    {
        caster = casterTransform;

        if (isJumpAttack)
        {
            transform.position = spawnPos + casterTransform.TransformDirection(offset);
        }
        else
        {
            transform.position = casterTransform.position + casterTransform.TransformDirection(offset);
        }

        transform.rotation = rotation;

        if (autoScaleFromHitbox && hitboxPrefab != null)
        {
            Hitbox hit = hitboxPrefab.GetComponent<Hitbox>();
            if (hit == null) return;

            switch (shape)
            {
                case ShapeType.Cone:
                    float distance = hit.coneDistance;
                    finalScale = new Vector3(distance, distance, distance);
                    break;
                case ShapeType.Circle:
                    float radius = hit.radius;
                    finalScale = new Vector3(radius, radius, radius);
                    break;
                case ShapeType.Box:
                    finalScale = hit.boxSize;
                    break;
                default:
                    finalScale = Vector3.one * 3f;
                    break;
            }
        }
    }


    void Update()
    {
        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / growTime);

        if (shape == ShapeType.Box)
        {
            // 네모 장판일 경우 → 앞 방향으로만 커지도록 처리
            Vector3 scaled = Vector3.Lerp(Vector3.zero, finalScale, t);
            transform.localScale = scaled;

            // 중심 위치를 offset (절반 거리만큼 앞으로 이동)
            Vector3 forward = caster.forward;
            transform.position = caster.position + caster.TransformDirection(offset) + forward * (scaled.z / 2f);
        }
        else
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, t);
        }

        if (time >= duration)
            Destroy(gameObject);
    }
}

