using System.Collections;
using UnityEngine;

public class BossEnemyAttackController : MonoBehaviour
{
    [Header("히트박스 프리팹 (AttackIndex 순서대로 연결)")]
    public GameObject[] attackPrefabs = new GameObject[8];

    
    [System.Serializable]
    public class TelegraphInfo
    {
        public GameObject telegraphPrefab;
        public ShapeType shapeType = ShapeType.None;
    }

    [SerializeField]
    [Header("경고 장판 프리팹 (AttackIndex 순서대로 연결)")]
    private TelegraphInfo[] telegraphInfos = new TelegraphInfo[8];

    private BossEnemyFSM bossFSM;

    private void Awake()
    {
        bossFSM = GetComponent<BossEnemyFSM>();
        if (bossFSM == null)
            Debug.LogError("[AttackController] BossEnemyFSM 컴포넌트가 없습니다.");
    }

    /// <summary>
    /// 히트박스 생성 (애니메이션 이벤트에서 호출)
    /// </summary>
    public void SpawnAttackByIndex()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
            return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (index < 0 || index >= attackPrefabs.Length)
            return;

        GameObject prefab = attackPrefabs[index];
        if (prefab == null)
            return;

        GameObject instance = Instantiate(prefab, transform.position, transform.rotation);
        instance.tag = "EnemyHitbox";

        Hitbox hitbox = instance.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            hitbox.Initialize(transform, true);
        }
    }

    /// <summary>
    /// 경고 장판 생성 (애니메이션 이벤트에서 호출)
    /// </summary>
    public void SpawnTelegraphByIndex()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
            return;

        int index = bossFSM.bossStatus.lastAttackIndex;

        if (index < 0 || index >= telegraphInfos.Length)
            return;

        var info = telegraphInfos[index];
        if (info.shapeType == ShapeType.None || info.telegraphPrefab == null)
            return;

        // 대상 방향 계산
        Transform target = bossFSM.bossStatus.target;
        if (target == null) return;

        Vector3 bossPos = transform.position;
        Vector3 dir = (target.position - bossPos);
        dir.y = 0f;
        if (dir == Vector3.zero) dir = transform.forward;
        dir.Normalize();

        float fullDistance = Vector3.Distance(bossPos, target.position);
        float adjustedDistance = Mathf.Max(fullDistance - 1.5f, 0f);
        Vector3 spawnPos = bossPos + dir * adjustedDistance;

        // 부채꼴의 꼭짓점이 보스를 기준으로 Z+ 방향으로 나가게
        Quaternion rotation = Quaternion.LookRotation(dir); // 추가 보정 없이 LookRotation만

        GameObject instance = Instantiate(info.telegraphPrefab, spawnPos, rotation);
        TelegraphArea telegraph = instance.GetComponent<TelegraphArea>();
        if (telegraph != null)
        {
            telegraph.shape = info.shapeType;
            telegraph.Initialize(spawnPos, rotation, transform);
        }
    }
}
