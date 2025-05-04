using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkillController : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private PlayerMovement playerMovement;
    private PlayerAttack playerattack;
    private Animator animator;

    [Header("스킬 데이터 (ScriptableObject)")]
    public JobSkillData skillData;

    private Dictionary<string, GameObject> hitboxPrefabs = new();
    private Dictionary<string, GameObject> effectPrefabs = new();
    private Dictionary<string, float> skillLastUsedTime = new();
    private Dictionary<string, float> skillCooldowns = new();

    private GameObject activeEffect;
    private GameObject player;
    private bool isSkillActive = false;
    private string currentJob = "";

    private Vector3? pendingMouseTarget = null;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        LoadSkillsFromData(skillData);
    }

    public void LoadSkillsFromData(JobSkillData newData)
    {
        if (newData == null)
        {
            Debug.LogWarning("[SkillController] 스킬 데이터가 없습니다.");
            return;
        }

        skillData = newData;
        hitboxPrefabs.Clear();
        effectPrefabs.Clear();
        skillLastUsedTime.Clear();
        skillCooldowns.Clear();

        currentJob = skillData.jobType.ToString();

        foreach (var skill in skillData.skills)
        {
            string key = currentJob + "_" + skill.skillKey;

            if (skill.hitboxPrefab != null)
                hitboxPrefabs[key] = skill.hitboxPrefab;

            if (skill.effectPrefab != null)
                effectPrefabs[key] = skill.effectPrefab;

            skillLastUsedTime[skill.skillKey] = -999f;
            skillCooldowns[skill.skillKey] = skill.cooldown;
        }

        Debug.Log($"[SkillController] {currentJob} 스킬 데이터 로드 완료");
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame) TryUseSkill("Q");
        if (Keyboard.current.wKey.wasPressedThisFrame) TryUseSkill("W");
        if (Keyboard.current.eKey.wasPressedThisFrame) TryUseSkill("E");
        if (Keyboard.current.rKey.wasPressedThisFrame) TryUseSkill("R");
    }

    public void TryUseSkill(string skillKey)
    {
        if (isSkillActive || !stateMachine.CanSkill()) return;

        var skill = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
        if (skill == null) return;

        float cooldown = GetSkillCooldown(skillKey);
        if (Time.time - GetLastUsedTime(skillKey) < cooldown) return;

        pendingMouseTarget = Hitbox.MouseUtility.GetMouseWorldPosition(LayerMask.GetMask("Ground"));

        playerMovement.StopAgent();
        if (pendingMouseTarget.HasValue)
            playerMovement.RotateToPosition(pendingMouseTarget.Value);
        else
            playerMovement.RotateToMouse();

        animator.applyRootMotion = true;
        animator.Play(skill.skillAnimation.name);

        skillLastUsedTime[skillKey] = Time.time;
        isSkillActive = true;

        var attack = GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.EnterCombatMode();
            //attack.ForceEndCombo();
        }
    }

    public void ActivateHitbox(string skillName)
    {
        var skillInfo = FindSkillInfoByName(skillName);
        if (skillInfo == null || skillInfo.hitboxPrefab == null)
        {
            Debug.LogWarning($"[Hitbox] {skillName} 스킬에 유효한 히트박스 프리팹 없음.");
            return;
        }

        GameObject instance = Instantiate(skillInfo.hitboxPrefab, transform.position + transform.forward, transform.rotation);
        Hitbox hitbox = instance.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            if (hitbox.useMousePosition && pendingMouseTarget.HasValue)
                hitbox.SetFixedMousePosition(pendingMouseTarget.Value);

            hitbox.Initialize(transform, skillInfo.followCaster);
        }
    }

    public void SpawnEffect(string skillName)
    {
        var skill = FindSkillInfoByName(skillName);
        if (skill == null || skill.effectPrefab == null) return;

        Transform spawnTransform = skill.effectPrefab.transform.Find("EffectSpawnPoint");

        Vector3 spawnPosition = spawnTransform != null
            ? transform.position + transform.TransformDirection(spawnTransform.localPosition)
            : transform.position + transform.forward;

        Quaternion spawnRotation = spawnTransform != null
            ? transform.rotation * spawnTransform.localRotation
            : transform.rotation;

        activeEffect = Instantiate(skill.effectPrefab, spawnPosition, spawnRotation);
        if (skill.effectDuration > 0f)
            Destroy(activeEffect, skill.effectDuration);
    }

    public void DestroyEffect()
    {
        if (activeEffect != null)
        {
            Destroy(activeEffect);
            activeEffect = null;
        }
    }

    public void EndSkill()
    {
        isSkillActive = false;
        animator.applyRootMotion = false;
        playerMovement.ResumeAgent();

        animator.SetTrigger("EndSkill");
        stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
    }

    private float GetSkillCooldown(string skillKey)
    {
        return skillCooldowns.TryGetValue(skillKey, out float cooldown) ? cooldown : 0f;
    }

    private float GetLastUsedTime(string skillKey)
    {
        return skillLastUsedTime.TryGetValue(skillKey, out float lastTime) ? lastTime : -999f;
    }

    public void UpdateCurrentJob(JobManager.JobType newJob)
    {
        currentJob = newJob.ToString();
        LoadSkillsFromData(skillData);
    }

    public SkillInfo GetSkillInfoByKey(string skillKey)
{
    return SkillEquipManager.Instance.GetEquippedSkill(skillKey);
}


    private SkillInfo FindSkillInfoByName(string skillName)
    {
        // 1. 기본 스킬에서 탐색
        foreach (var s in skillData.skills)
            if (s.skillName == skillName) return s;

        // 2. 강화 스킬에서 탐색
        foreach (var baseSkill in skillData.skills)
        {
            var upgradeData = SkillUpgradeManager.Instance.GetUpgradeDataFor(baseSkill.skillName);
            if (upgradeData == null || upgradeData.upgradeOptions == null) continue;

            foreach (var upgraded in upgradeData.upgradeOptions)
                if (upgraded.skillName == skillName) return upgraded;
        }

        return null;
    }
}
