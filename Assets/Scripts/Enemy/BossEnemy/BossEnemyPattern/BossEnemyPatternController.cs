using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyPatternController : MonoBehaviour
{
    private Dictionary<int, float> cooldownTimers = new Dictionary<int, float>();
    private int lastUsedIndex = -1;

    // 콤보/Far 공격 쿨타임 범위
    private readonly float comboMinCooldown = 5f;
    private readonly float comboMaxCooldown = 10f;

    private readonly float farMinCooldown = 5f;
    private readonly float farMaxCooldown = 10f;

    // 공격 Index 범위
    private readonly int[] lightAttacks = { 0, 1, 2 };
    private readonly int[] comboAttacks = { 3, 4, 5 };
    private readonly int[] farAttacks = { 6, 7 };
    private const int wallAttackIndex = 8;

    private void Awake()
    {
        // 쿨타임 초기화
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
        // 우선순위 1: 벽 회피 (연속 허용)
        if (isNearWall)
            return wallAttackIndex;

        // 우선순위 2: Far 공격
        if (distanceToPlayer >= 6f)
        {
            List<int> available = new List<int>();
            foreach (int i in farAttacks)
            {
                if (Time.time >= cooldownTimers[i])
                    available.Add(i);
            }

            if (available.Count > 0)
                return SelectRandomAndRecord(available, useCooldown: true, isCombo: false);
        }

        // 우선순위 3: Close 공격
        if (distanceToPlayer <= 4f)
        {
            List<int> available = new List<int>();

            // Light (0~2) → 쿨타임 없음, 중복 제한만 적용
            foreach (int i in lightAttacks)
            {
                if (i != lastUsedIndex)
                    available.Add(i);
            }

            // Combo (3~5) → 쿨타임 + 중복 제한
            foreach (int i in comboAttacks)
            {
                if (i != lastUsedIndex && Time.time >= cooldownTimers[i])
                    available.Add(i);
            }

            if (available.Count > 0)
                return SelectRandomAndRecord(available, useCooldown: true, isCombo: true);
        }

        return -1; // 패턴 없음
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
}
