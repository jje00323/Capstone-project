using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : CharacterStatus
{

    [Header("UI 연결")]
    public PlayerStateUI stateUI;


    private JobManager.JobType currentJob;


    public float maxMP;
    public float currentMP;

    public float maxEXP;
    public float currentEXP;

    public int level;

    
    public float critRate;
    public float critDamage;
    public float moveSpeed;

    public PlayerUI playerUI;

    private readonly List<Coroutine> activeBuffs = new();
    private Coroutine hpRegenCoroutine;
    private Coroutine mpRegenCoroutine;
    private Dictionary<StatType, Coroutine> activeBuffDict = new();



    void Start()
    {
        if (stateUI == null)
        {
            // 보조 안전 장치 (비활성화 포함해서 찾기)
            var canvasRoot = GameObject.Find("UIRoot"); // 이름은 실제 UI 루트 GameObject 이름으로 바꾸세요
            if (canvasRoot != null)
                stateUI = canvasRoot.GetComponentInChildren<PlayerStateUI>(true);

            if (stateUI != null)
                Debug.Log("[PlayerStatus] stateUI 수동 연결 성공");
            else
                Debug.LogWarning("[PlayerStatus] PlayerStateUI를 찾을 수 없습니다.");
        }

        UpdateAllUI();
    }

    public void ApplyJobStats(JobStatusData data)
    {
        level = 1;

        maxHP = data.baseHP;
        maxMP = data.baseMP;
        currentHP = maxHP;
        currentMP = maxMP;

       
        attack = data.baseAttack;
        defense = data.baseDefense;

        critRate = data.critical;
        critDamage = data.critical_Damage;


        UpdateAllUI();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        playerUI.UpdateHP(currentHP, maxHP);
        Debug.Log($"player 현재 체력: {currentHP}");
    }

    public override void Heal(float amount)
    {
        base.Heal(amount); // 상위 클래스 기능 사용
        playerUI?.UpdateHP(currentHP, maxHP); // UI 업데이트 등 플레이어 전용 처리
    }

    public void UseMana(float amount)
    {
        currentMP = Mathf.Clamp(currentMP - amount, 0, maxMP);
        playerUI.UpdateMP(currentMP, maxMP);
    }

    public void GainEXP(float amount)
    {
        currentEXP += amount;
        var statData = JobManager.Instance.GetStatData(currentJob); // 현재 직업의 성장 스탯 가져오기

        while (currentEXP >= maxEXP)
        {
            if (statData != null)
            {
                GainLevel(statData);
            }
            else
            {
                Debug.LogWarning($"[PlayerStatus] {currentJob}의 JobStatusData가 존재하지 않습니다.");
                break;
            }
        }

        playerUI.UpdateEXP(currentEXP, maxEXP);
    }

    public void GainLevel(JobStatusData statData)
    {
        int previousLevel = level;

        currentEXP -= maxEXP;
        level++;

        maxHP += statData.hpPerLevel;
        maxMP += statData.mpPerLevel;
        // 필요 시 attack, defense 등도 증가
        currentHP = maxHP;
        currentMP = maxMP;

        if (SkillPointManager.Instance != null)
        {
            SkillPointManager.Instance.GainPointByLevel(previousLevel);
            Debug.Log($"[레벨업] Lv.{previousLevel} → Lv.{level} / 스킬포인트 +{previousLevel}");
        }
        else
        {
            Debug.LogWarning("[레벨업] SkillPointManager가 초기화되지 않았습니다.");
        }

        UpdateAllUI();
    }

    public void ApplyTemporaryModifier(StatModifier modifier, float duration)
    {
        if (activeBuffDict.TryGetValue(modifier.type, out Coroutine existingBuff))
        {
            Debug.Log($"[버프 중첩 차단] 이미 {modifier.type} 버프가 적용되어 있음");
            return;
        }

        StatusEffectApplier.ApplyStatModifiers(this, new List<StatModifier> { modifier });

        Coroutine newBuff = StartCoroutine(RemoveModifierAfterTime(modifier, duration));
        activeBuffs.Add(newBuff);
        activeBuffDict[modifier.type] = newBuff;
        LogCurrentStats();
    }

    private IEnumerator RemoveModifierAfterTime(StatModifier modifier, float duration)
    {
        yield return new WaitForSeconds(duration);

        var inverse = new StatModifier(modifier.type, -modifier.value, modifier.isFlat);
        StatusEffectApplier.ApplyStatModifiers(this, new List<StatModifier> { inverse });

        activeBuffDict.Remove(modifier.type);
        Debug.Log($"[버프 해제] {modifier.type} 버프 종료됨");
    }

    public void ClearAllBuffs()
    {
        foreach (var buff in activeBuffs)
        {
            if (buff != null) StopCoroutine(buff);
        }
        activeBuffs.Clear();
    }

    

    public void StartHPRegen(float amountPerSecond, float duration)
    {
        if (hpRegenCoroutine != null) StopCoroutine(hpRegenCoroutine);
        hpRegenCoroutine = StartCoroutine(RegenCoroutine(amountPerSecond, duration, isHP: true));
    }

    public void StartMPRegen(float amountPerSecond, float duration)
    {
        if (mpRegenCoroutine != null) StopCoroutine(mpRegenCoroutine);
        mpRegenCoroutine = StartCoroutine(RegenCoroutine(amountPerSecond, duration, isHP: false));
    }

    private IEnumerator RegenCoroutine(float amountPerSecond, float duration, bool isHP)
    {
        float elapsed = 0f;
        float interval = 1f;

        while (elapsed < duration)
        {
            if (isHP)
                Heal(amountPerSecond);
            else
                UseMana(-amountPerSecond);

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        Debug.Log($"[지속 회복 종료] {(isHP ? "HP" : "MP")} +{amountPerSecond}/초, {duration}초");
    }

    public void UpdateAllUI()
    {
        Debug.Log("[UpdateAllUI] 호출됨");

        playerUI.UpdateHP(currentHP, maxHP);
        playerUI.UpdateMP(currentMP, maxMP);
        playerUI.UpdateEXP(currentEXP, maxEXP);
        playerUI.UpdateLevel(level);

        if (stateUI == null)
        {
            Debug.LogWarning("[UpdateAllUI] stateUI가 null입니다. UI가 갱신되지 않습니다.");
        }
        else
        {
            Debug.Log("[UpdateAllUI] stateUI 연결 확인됨 → UpdateStats() 호출 시도");
            stateUI.UpdateStats();
        }

        Debug.Log($"[스탯 상태] " +
            $"LV: {level}, " +
            $"HP: {currentHP}/{maxHP}, " +
            $"MP: {currentMP}/{maxMP}, " +
            $"ATK: {attack}, DEF: {defense}, " +
            $"CritRate: {critRate}, CritDmg: {critDamage}");
    }

    protected override void OnDeath()
    {
        Debug.Log("플레이어 사망 처리");
        // 플레이어 사망 처리 추가
    }

    public void SetJob(JobManager.JobType job)
    {
        currentJob = job;
    }

    private void LogCurrentStats()
    {
        Debug.Log($"[현재 스탯] HP: {currentHP}/{maxHP}, MP: {currentMP}/{maxMP}");
        // 여기서 공격력, 방어력 등도 출력 가능
        // 예: Debug.Log($"[공격력] {attack}, [방어력] {defense}");
    }
}

