using System.Collections.Generic;
using UnityEngine;

public class BossEnemyPatternController : MonoBehaviour
{
    private Dictionary<int, float> cooldownTimers = new Dictionary<int, float>();
    private int lastUsedIndex = -1;

    private readonly float comboMinCooldown = 5f;
    private readonly float comboMaxCooldown = 10f;
    private readonly float farMinCooldown = 5f;
    private readonly float farMaxCooldown = 10f;

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

        if (distanceToPlayer <= 3f) // Close 공격
        {
            List<int> available = new List<int>();

            // Light 공격 (쿨타임 없음, 중복만 방지)
            foreach (int i in lightAttacks)
            {
                if (i != lastUsedIndex)
                    available.Add(i);
            }

            // Combo 공격 (쿨타임 존재, 중복 방지)
            foreach (int i in comboAttacks)
            {
                if (i != lastUsedIndex && Time.time >= cooldownTimers[i])
                    available.Add(i);
            }

            if (available.Count > 0)
                return SelectRandomAndRecord(available, useCooldown: true, isCombo: true);
            else
                return -1; // 선택 실패
        }
        else if (distanceToPlayer > 3f && distanceToPlayer <= 6f) // Far 공격
        {
            List<int> available = new List<int>();
            foreach (int i in farAttacks)
            {
                if (Time.time >= cooldownTimers[i])
                    available.Add(i);
            }

            if (available.Count > 0)
                return SelectRandomAndRecord(available, useCooldown: true, isCombo: false);
            else
                return -1; // 선택 실패
        }

        return -1; // 6 이상일 경우 Far 공격 가능할 때만 PatternState 들어오게 설계
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
