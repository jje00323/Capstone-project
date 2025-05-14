using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttackController : MonoBehaviour
{
    [Header("ScriptableObject 기반 보스 공격 데이터")]
    [SerializeField]
    private BossAttackData[] attackDataList = new BossAttackData[8];

    private int hitboxInvokeCount = 0;
    private int telegraphInvokeCount = 0;

    private BossEnemyFSM bossFSM;

    private Coroutine animSpeedRoutine = null;

    private float animStartTime = -1f;

    [System.Serializable]
    public struct AnimSpeedProfile
    {
        public int attackIndex;        // 공격 인덱스
        public float startSpeed;       // 시작 배속
        public float targetSpeed;      // 최종 배속
        public float duration;         // 서서히 복구 시간
        public float finalSpeed;       // 복구 완료 시 속도 (보통 1.0)
    }
    [SerializeField]
    private List<AnimSpeedProfile> animSpeedProfiles = new List<AnimSpeedProfile>();

    private Dictionary<int, AnimSpeedProfile> profileLookup;

    private void Awake()
    {
        bossFSM = GetComponent<BossEnemyFSM>();
        if (bossFSM == null)
            Debug.LogError("[AttackController] BossEnemyFSM 컴포넌트가 없습니다.");

        // Dictionary 초기화
        profileLookup = new Dictionary<int, AnimSpeedProfile>();
        foreach (var profile in animSpeedProfiles)
        {
            profileLookup[profile.attackIndex] = profile;
        }
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
    /// 
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

        bool isJumpAttack = (index == 6);

        Vector3 spawnPos;
        if (isJumpAttack)
        {
            // 점프 공격일 경우 타겟 위치가 중심 (보스가 점프 착지하는 위치)
            spawnPos = target.position;
        }
        else
        {
            float fullDistance = Vector3.Distance(bossPos, target.position);
            float adjustedDistance = Mathf.Max(fullDistance - 1.5f, 0f);
            spawnPos = bossPos + dir * adjustedDistance;
        }

        Quaternion rotation = Quaternion.LookRotation(dir);

        GameObject instance = Instantiate(info.telegraphPrefab, spawnPos, rotation);
        TelegraphArea telegraph = instance.GetComponent<TelegraphArea>();
        if (telegraph != null)
        {
            telegraph.shape = info.shapeType;
            telegraph.Initialize(spawnPos, rotation, transform, isJumpAttack);
        }

        telegraphInvokeCount++;
    }
    private IEnumerator SmoothAnimSpeed(float from, float to, float duration)
    {
        float elapsed = 0f;
        bossFSM.Animator.speed = from;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentSpeed = Mathf.Lerp(from, to, t);
            bossFSM.Animator.speed = currentSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        bossFSM.Animator.speed = to;
        animSpeedRoutine = null;
        Debug.Log("[AnimSpeed] 배속 보간 완료");
    }

    // 애니메이션 중 배속 조절 시작
    public void OnAnimSpeedStart()
    {
        if (bossFSM == null || bossFSM.Animator == null) return;

        int index = bossFSM.bossStatus.lastAttackIndex;

        if (profileLookup.TryGetValue(index, out AnimSpeedProfile profile))
        {
            if (animSpeedRoutine != null)
                StopCoroutine(animSpeedRoutine);

            animSpeedRoutine = StartCoroutine(SmoothAnimSpeed(profile.startSpeed, profile.targetSpeed, profile.duration));
        }
        else
        {
            Debug.Log($"[AnimSpeed] 인덱스 {index}에 대한 배속 프로필이 없습니다.");
        }
    }
    // 애니메이션 배속 초기화
    public void OnAnimSpeedEnd()
    {
        if (bossFSM == null || bossFSM.Animator == null) return;

        int index = bossFSM.bossStatus.lastAttackIndex;

        if (animSpeedRoutine != null)
        {
            StopCoroutine(animSpeedRoutine);
            animSpeedRoutine = null;
        }

        if (profileLookup.TryGetValue(index, out AnimSpeedProfile profile))
        {
            bossFSM.Animator.speed = profile.finalSpeed;
        }
        else
        {
            bossFSM.Animator.speed = 1f; // 기본값
        }

        Debug.Log($"[AnimSpeed] 배속 복구 완료 (Index: {index})");
    }
    public void BeginAttack()
    {
        hitboxInvokeCount = 0; // 첫 히트박스부터 시작
        telegraphInvokeCount = 0;
    }

    /// <summary>
    /// 애니메이션 이벤트: 시작 프레임 (예: 9프레임)
    /// </summary>
    public void AnimTimeStart()
    {
        animStartTime = Time.time;
        Debug.Log($"[AnimTimer] 시작 시간 기록됨: {animStartTime:F4}초");
    }

    /// <summary>
    /// 애니메이션 이벤트: 종료 프레임 (예: 42프레임)
    /// </summary>
    public void AnimTimeEnd()
    {
        if (animStartTime < 0f)
        {
            Debug.LogWarning("[AnimTimer] 시작 시간이 설정되지 않았습니다.");
            return;
        }

        float elapsed = Time.time - animStartTime;
        Debug.Log($"[AnimTimer] 총 경과 시간: {elapsed:F4}초 (시작→종료 프레임)");
        animStartTime = -1f; // 초기화
    }
}
