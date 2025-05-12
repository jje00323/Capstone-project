using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphArea : MonoBehaviour
{
    public enum ShapeType { Circle, Box, Cone }

    [Header("기본 설정")]
    public float duration = 1.5f; // 장판 유지 시간
    public float growTime = 1.0f; // 커지는 시간
    public ShapeType shape;
    public float radius = 3f;
    public Vector3 boxSize = Vector3.one;
    public float coneAngle = 45f;
    public float coneDistance = 6f;
    public Vector3 offset = Vector3.zero;
    public bool followCaster = false;

    [Header("색상 변화")]
    public Color startColor = new Color(1f, 0f, 0f, 0.2f);
    public Color endColor = new Color(1f, 0f, 0f, 0.7f);

    [Header("타이밍 동기화")]
    public float hitboxDelay = 1.5f; // 이 시간이 지나면 히트박스 생성
    public GameObject hitboxPrefab;

    private Transform caster;
    private float time = 0f;
    private Material mat;
    private MeshRenderer rend;
    private bool hitboxSpawned = false;

    public void Initialize(Transform caster)
    {
        this.caster = caster;
        rend = GetComponent<MeshRenderer>();
        mat = rend.material;
        mat.color = startColor;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (followCaster && caster != null)
        {
            transform.position = caster.position + caster.TransformDirection(offset);
            transform.rotation = caster.rotation;
        }

        time += Time.deltaTime;

        float t = Mathf.Clamp01(time / growTime);

        // 콘의 거리와 시야각에 따라 시각적 크기 반영 (간단하게 Z 방향만 키움)
        switch (shape)
        {
            case ShapeType.Cone:
                // FanPlane_70deg.fbx는 Z+ 기준, 정확한 시각 구현을 위해 스케일을 고정
                Vector3 targetScale = new Vector3(12f, 12f, 1f);
                transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
                break;

            default:
                transform.localScale = Vector3.one * t;
                break;
        }

        if (mat != null)
            mat.color = Color.Lerp(startColor, endColor, t);

        if (time >= duration)
        {
            Destroy(gameObject);
        }
    }
}

