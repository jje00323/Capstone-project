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
        public int attackIndex;
        public float startSpeed;
        public float targetSpeed;
        public float duration;
        public float finalSpeed;
    }

    [SerializeField]
    private List<AnimSpeedProfile> animSpeedProfiles = new List<AnimSpeedProfile>();
    private Dictionary<int, AnimSpeedProfile> profileLookup;

    private void Awake()
    {
        bossFSM = GetComponent<BossEnemyFSM>();
        if (bossFSM == null)
            Debug.LogError("[AttackController] BossEnemyFSM 컴포넌트가 없습니다.");

        profileLookup = new Dictionary<int, AnimSpeedProfile>();
        foreach (var profile in animSpeedProfiles)
            profileLookup[profile.attackIndex] = profile;
    }

    public void SpawnAttackBySubtable()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
            return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (index < 0 || index >= attackDataList.Length)
            return;

        var data = attackDataList[index];
        if (data == null || data.hitboxInfos == null)
            return;

        if (hitboxInvokeCount >= data.hitboxInfos.Count)
            return;

        var info = data.hitboxInfos[hitboxInvokeCount];
        if (info == null || info.hitboxPrefab == null)
            return;

        GameObject instance = Instantiate(info.hitboxPrefab, transform.position, transform.rotation);
        instance.tag = "EnemyHitbox";

        BossHitbox hitbox = instance.GetComponent<BossHitbox>();
        if (hitbox != null)
        {
            hitbox.Initialize(transform, info.followCaster);
        }

        hitboxInvokeCount++;
    }

    //public void SpawnTelegraphBySubtable()
    //{
    //    if (bossFSM == null || bossFSM.bossStatus == null)
    //        return;

    //    int index = bossFSM.bossStatus.lastAttackIndex;
    //    if (index < 0 || index >= attackDataList.Length)
    //        return;

    //    var data = attackDataList[index];
    //    if (data == null || data.telegraphInfos == null)
    //        return;

    //    if (telegraphInvokeCount >= data.telegraphInfos.Count)
    //        return;

    //    var info = data.telegraphInfos[telegraphInvokeCount];
    //    if (info == null || info.telegraphPrefab == null)
    //        return;

    //    Transform target = bossFSM.bossStatus.target;
    //    if (target == null) return;

    //    //Vector3 bossPos = transform.position;
    //    //Vector3 dir = (target.position - bossPos);
    //    //dir.y = 0f;
    //    //if (dir == Vector3.zero) dir = transform.forward;
    //    //dir.Normalize();

    //    //bool isJumpAttack = (index == 6);
    //    //Vector3 spawnPos = isJumpAttack ? target.position : bossPos + dir * Mathf.Max(Vector3.Distance(bossPos, target.position) - 1.5f, 0f);
    //    //Quaternion rotation = Quaternion.LookRotation(dir);
    //    bool isJumpAttack = (index == 6);

    //    // 보스의 현재 회전값을 그대로 사용
    //    Quaternion rotation = transform.rotation;

    //    // 보스 기준 전방으로 일정 거리 생성
    //    float spawnDistance = 3.5f; // 공격 범위에 따라 조정 가능
    //    Vector3 spawnPos = isJumpAttack
    //        ? target.position
    //        : transform.position + rotation * Vector3.forward * spawnDistance;

    //    GameObject instance = Instantiate(info.telegraphPrefab, spawnPos, rotation);
    //    TelegraphArea telegraph = instance.GetComponent<TelegraphArea>();
    //    if (telegraph != null)
    //    {
    //        telegraph.shape = info.shapeType;
    //        telegraph.Initialize(spawnPos, rotation, transform, isJumpAttack);
    //    }

    //    telegraphInvokeCount++;
    //}
    public void SpawnTelegraphBySubtable()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
            return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (index < 0 || index >= attackDataList.Length)
        {
            Debug.LogWarning($"[Telegraph] 잘못된 index 접근: {index}");
            return;
        }

        var data = attackDataList[index];
        if (data == null || data.telegraphInfos == null)
        {
            Debug.LogWarning($"[Telegraph] attackDataList[{index}]의 telegraphInfos가 null");
            return;
        }

        if (telegraphInvokeCount >= data.telegraphInfos.Count)
        {
            Debug.LogWarning($"[Telegraph] telegraphInvokeCount({telegraphInvokeCount}) >= telegraphInfos.Count({data.telegraphInfos.Count})");
            return;
        }

        var info = data.telegraphInfos[telegraphInvokeCount];
        if (info == null || info.telegraphPrefab == null)
        {
            Debug.LogWarning($"[Telegraph] telegraphInfos[{telegraphInvokeCount}]가 null 또는 프리팹 없음");
            return;
        }

        Transform target = bossFSM.bossStatus.target;
        if (target == null)
        {
            Debug.LogWarning("[Telegraph] 타겟이 null입니다");
            return;
        }

        // 디버그 로그: 보스 위치 및 타겟 거리
        Vector3 bossPos = transform.position;
        Vector3 targetPos = target.position;
        Vector3 dir = targetPos - bossPos;
        dir.y = 0f;
        if (dir == Vector3.zero) dir = transform.forward;
        dir.Normalize();

        bool isJumpAttack = (index == 6);
        Quaternion rotation = transform.rotation; // 수정 적용
        float spawnDistance = 3.5f;
        Vector3 spawnPos = isJumpAttack ? targetPos : bossPos + rotation * Vector3.forward * spawnDistance;

        // 디버그 로그 출력
        Debug.Log($"[Telegraph] 호출됨 - 공격Index: {index}, InvokeCount: {telegraphInvokeCount}, isJump: {isJumpAttack}");
        Debug.Log($"[Telegraph] BossPos: {bossPos}, TargetPos: {targetPos}, SpawnPos: {spawnPos}");
        Debug.Log($"[Telegraph] 보스 회전값: {transform.rotation.eulerAngles}, 생성 회전값: {rotation.eulerAngles}");
        Debug.Log($"[Telegraph] Prefab: {info.telegraphPrefab.name}, Shape: {info.shapeType}");

        GameObject instance = Instantiate(info.telegraphPrefab, spawnPos, rotation);
        TelegraphArea telegraph = instance.GetComponent<TelegraphArea>();
        if (telegraph != null)
        {
            telegraph.shape = info.shapeType;
            telegraph.Initialize(spawnPos, rotation, transform, isJumpAttack);
        }
        else
        {
            Debug.LogWarning("[Telegraph] TelegraphArea 컴포넌트를 찾을 수 없습니다.");
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
            bossFSM.Animator.speed = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        bossFSM.Animator.speed = to;
        animSpeedRoutine = null;
        Debug.Log("[AnimSpeed] 배속 보간 완료");
    }

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
            bossFSM.Animator.speed = profile.finalSpeed;
        else
            bossFSM.Animator.speed = 1f;

        Debug.Log($"[AnimSpeed] 배속 복구 완료 (Index: {index})");
    }

    public void BeginAttack()
    {
        hitboxInvokeCount = 0;
        telegraphInvokeCount = 0;
    }

    public void AnimTimeStart()
    {
        animStartTime = Time.time;
        Debug.Log($"[AnimTimer] 시작 시간 기록됨: {animStartTime:F4}초");
    }

    public void AnimTimeEnd()
    {
        if (animStartTime < 0f)
        {
            Debug.LogWarning("[AnimTimer] 시작 시간이 설정되지 않았습니다.");
            return;
        }

        float elapsed = Time.time - animStartTime;
        Debug.Log($"[AnimTimer] 총 경과 시간: {elapsed:F4}초 (시작→종료 프레임)");
        animStartTime = -1f;
    }
    public GameObject GetTelegraphPrefab(int index)
    {
        if (index < 0 || index >= attackDataList.Length) return null;
        var data = attackDataList[index];
        return (data != null && data.telegraphInfos.Count > 0) ? data.telegraphInfos[0].telegraphPrefab : null;
    }

    public GameObject GetHitboxPrefab(int index)
    {
        if (index < 0 || index >= attackDataList.Length) return null;
        var data = attackDataList[index];
        return (data != null && data.hitboxInfos.Count > 0) ? data.hitboxInfos[0].hitboxPrefab : null;
    }

    public void SpawnLightningAttackGroup(Vector3 pos, int index)
    {
        var telegraphPrefab = GetTelegraphPrefab(index);
        var hitboxPrefab = GetHitboxPrefab(index);

        if (telegraphPrefab != null)
        {
            Debug.Log("[번개패턴] Telegraph Prefab 인스턴스 생성됨");
            GameObject t = Instantiate(telegraphPrefab, pos, Quaternion.identity);
            var ta = t.GetComponent<TelegraphArea>();
            ta?.Initialize(pos, Quaternion.identity, transform, false);
        }
        else
        {
            Debug.LogWarning("[번개패턴] Telegraph Prefab이 null입니다.");
        }

        StartCoroutine(SpawnHitboxDelayed(pos, hitboxPrefab, 1.5f));
    }

    private IEnumerator SpawnHitboxDelayed(Vector3 pos, GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (prefab != null)
        {
            Debug.Log("[번개패턴] 히트박스 프리팹 생성됨");
            GameObject h = Instantiate(prefab, pos, Quaternion.identity);
            var bh = h.GetComponent<BossHitbox>();
            bh?.Initialize(pos, Quaternion.identity, transform, false);
        }
        else
        {
            Debug.LogWarning("[번개패턴] 히트박스 프리팹이 null입니다.");
        }
    }
}