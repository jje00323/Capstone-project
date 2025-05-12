using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BossEnemyPatternController : MonoBehaviour
{
    private Dictionary<int, float> cooldownTimers = new Dictionary<int, float>();
    private int lastUsedIndex = -1;

    public BossEnemyFSM bossFSM; // 인스펙터에서 직접 연결

    private readonly float comboMinCooldown = 12f;
    private readonly float comboMaxCooldown = 20f;
    private readonly float farMinCooldown = 16f;
    private readonly float farMaxCooldown = 28f;

    private readonly int[] lightAttacks = { 0, 1, 2 };
    private readonly int[] comboAttacks = { 3, 4, 5 };
    private readonly int[] farAttacks = { 6, 7 };
    private const int wallAttackIndex = 8;

    private void Awake()
    {
        foreach (int index in comboAttacks)
            cooldownTimers[index] = -Mathf.Infinity;
        foreach (int index in farAttacks)
            cooldownTimers[index] = -Mathf.Infinity;
    }

    public bool IsFarPatternReady()
    {
        foreach (var index in farAttacks)
        {
            if (Time.time >= cooldownTimers[index])
                return true;
        }
        return false;
    }

    public int GetPattern(float distanceToPlayer, bool isNearWall)
    {
        if (isNearWall)
            return wallAttackIndex;

        List<int> available = new List<int>();

        // 1. Light 공격 (0~3m)
        if (distanceToPlayer <= 3f)
        {
            foreach (int i in lightAttacks)
            {
                if (i != lastUsedIndex)
                    available.Add(i);
            }
        }

        // 2. Combo 공격 (0~6m)
        if (distanceToPlayer <= 6f)
        {
            foreach (int i in comboAttacks)
            {
                if (i != lastUsedIndex && Time.time >= cooldownTimers[i])
                    available.Add(i);
            }
        }

        // 3. Spin 공격 (6~8m)
        if (distanceToPlayer > 6f && distanceToPlayer <= 8f)
        {
            if (Time.time >= cooldownTimers[7])
                available.Add(7);
        }

        // 4. Jump 공격 (8~12m)
        if (distanceToPlayer > 8f && distanceToPlayer <= 12f)
        {
            if (Time.time >= cooldownTimers[6])
                available.Add(6);
        }

        if (available.Count > 0)
            return SelectRandomAndRecord(available, useCooldown: true, isCombo: false);
        else
            return -1;
    }

    private int SelectRandomAndRecord(List<int> list, bool useCooldown, bool isCombo)
    {
        if (list.Count == 0) return -1;

        int selected = list[Random.Range(0, list.Count)];
        lastUsedIndex = selected;

        if (useCooldown && selected >= 3)
        {
            float cooldown = (selected >= 6) ?
                Random.Range(farMinCooldown, farMaxCooldown) :
                Random.Range(comboMinCooldown, comboMaxCooldown);

            cooldownTimers[selected] = Time.time + cooldown;
        }

        return selected;
    }

    // 단일 인덱스를 반환하고 기록하는 헬퍼 메서드
    private int SelectSingleAndRecord(int index)
    {
        lastUsedIndex = index;

        // 쿨타임 적용 대상일 경우
        if (index == 6 || index == 7)
        {
            float cooldown = Random.Range(farMinCooldown, farMaxCooldown);
            cooldownTimers[index] = Time.time + cooldown;
        }

        return index;
    }
    public bool HasAvailablePattern(float distanceToPlayer, bool isNearWall)
    {
        if (isNearWall)
            return true; // 벽 패턴은 항상 사용 가능

        // 1. Light 공격 (0~3m)
        if (distanceToPlayer <= 3f)
        {
            foreach (int i in lightAttacks)
            {
                if (i != lastUsedIndex)
                    return true;
            }
        }

        // 2. Combo 공격 (0~6m)
        if (distanceToPlayer <= 6f)
        {
            foreach (int i in comboAttacks)
            {
                if (i != lastUsedIndex && Time.time >= cooldownTimers[i])
                    return true;
            }
        }

        // 3. Spin 공격 (6~8m)
        if (distanceToPlayer > 6f && distanceToPlayer <= 8f)
        {
            if (Time.time >= cooldownTimers[7])
                return true;
        }

        // 4. Jump 공격 (8~12m)
        if (distanceToPlayer > 8f && distanceToPlayer <= 12f)
        {
            if (Time.time >= cooldownTimers[6])
                return true;
        }

        return false;
    }
    public void EndBossPattern()
    {
        if (bossFSM != null)
        {
            Debug.Log("[애니메이션 이벤트] EndBossPattern 호출됨");
            bossFSM.Animator.speed = 1f;
            bossFSM.EndBossPattern(); // FSM에서 상태 전환 처리
        }
        else
        {
            Debug.LogWarning("BossEnemyFSM가 연결되지 않았습니다.");
        }
    }

    public void Hmm()
    {
        Debug.Log("이벤트 된다!!");
    }
}
