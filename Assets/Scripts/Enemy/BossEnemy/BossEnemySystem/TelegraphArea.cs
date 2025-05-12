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

    public void Initialize(Vector3 spawnPos, Quaternion rotation, Transform casterTransform)
    {
        caster = casterTransform;

        //// Y축 높이는 항상 offset 기준으로 고정 (장판이 바닥에 딱 붙게)
        //spawnPos.y = offset.y; // 일반적으로 0.01f

        transform.position = spawnPos + casterTransform.TransformDirection(offset);

        // 회전은 보스가 바라보는 방향 그대로, 추가 회전 없이
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
                    float radius = hit.radius * 2f;
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
        transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, t);

        if (time >= duration)
            Destroy(gameObject);
    }
}
