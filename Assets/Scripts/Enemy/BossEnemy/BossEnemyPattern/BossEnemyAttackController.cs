using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttackController : MonoBehaviour
{
    //[Header("히트박스 프리팹 (AttackIndex 순서대로 연결)")]
    //public GameObject[] attackPrefabs = new GameObject[8];


    //[System.Serializable]
    //public class TelegraphInfo
    //{
    //    public GameObject telegraphPrefab;
    //    public ShapeType shapeType = ShapeType.None;
    //}

    //[SerializeField]
    //[Header("경고 장판 프리팹 (AttackIndex 순서대로 연결)")]
    //private TelegraphInfo[] telegraphInfos = new TelegraphInfo[8];

    [Header("ScriptableObject 기반 보스 공격 데이터")]
    [SerializeField]
    private BossAttackData[] attackDataList = new BossAttackData[8];

    private int hitboxInvokeCount = 0;
    private int telegraphInvokeCount = 0;

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
    
    public void SpawnAttackBySubtable()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
            return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (index < 0 || index >= attackDataList.Length)
            return;

        var data = attackDataList[index];
        if (data == null || data.hitboxPrefabs == null)
            return;

        if (hitboxInvokeCount >= data.hitboxPrefabs.Count)
            return; // 호출 횟수 초과 시 무시

        GameObject prefab = data.hitboxPrefabs[hitboxInvokeCount];
        if (prefab == null)
            return;

        GameObject instance = Instantiate(prefab, transform.position, transform.rotation);
        instance.tag = "EnemyHitbox";

        Hitbox hitbox = instance.GetComponent<Hitbox>();
        if (hitbox != null)
            hitbox.Initialize(transform, true);

        hitboxInvokeCount++; // 다음 호출을 위해 증가
    }
    /// <summary>
    /// 경고 장판 생성 (애니메이션 이벤트에서 호출)
    /// </summary>

    public void SpawnTelegraphBySubtable()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
            return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (index < 0 || index >= attackDataList.Length)
            return;

        var data = attackDataList[index];
        if (data == null || data.telegraphInfos == null)
            return;

        if (telegraphInvokeCount >= data.telegraphInfos.Count)
            return;

        var info = data.telegraphInfos[telegraphInvokeCount];
        if (info == null || info.telegraphPrefab == null || info.shapeType == ShapeType.None)
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
        Quaternion rotation = Quaternion.LookRotation(dir);

        GameObject instance = Instantiate(info.telegraphPrefab, spawnPos, rotation);
        TelegraphArea telegraph = instance.GetComponent<TelegraphArea>();
        if (telegraph != null)
        {
            telegraph.shape = info.shapeType;
            telegraph.Initialize(spawnPos, rotation, transform);
        }

        telegraphInvokeCount++;
    }
    public void BeginAttack()
    {
        hitboxInvokeCount = 0; // 첫 히트박스부터 시작
        telegraphInvokeCount = 0;
    }
}
