using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    [Header("스킬 데이터 (ScriptableObject)")]
    public JobSkillData skillData;

    private Dictionary<string, GameObject> hitboxPrefabs = new();
    private Dictionary<string, GameObject> effectPrefabs = new();
    private Dictionary<string, float> skillCooldowns = new();

    private GameObject activeEffect;
    private string currentJob = "";
    private Vector3? pendingMouseTarget = null;

    private Dictionary<string, float> skillLastUsedTime = new();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        LoadSkillsFromData(skillData);
    }

    public void ExecuteSkill(string skillKey, Vector3? mouseTarget)
    {
        var skill = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
        if (skill == null) return;

        pendingMouseTarget = mouseTarget;

        playerMovement.StopAgent();
        if (pendingMouseTarget.HasValue)
            playerMovement.RotateToPosition(pendingMouseTarget.Value);
        else
            playerMovement.RotateToMouse();

        animator.applyRootMotion = true;
        animator.Play(skill.skillAnimation.name);

        if (playerAttack != null)
            playerAttack.EnterCombatMode();
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

    public float GetSkillCooldown(string skillKey)
    {
        return skillCooldowns.TryGetValue(skillKey, out float cooldown) ? cooldown : 0f;
    }

    public void SetSkillCooldown(string skillKey, float time)
    {
        skillCooldowns[skillKey] = time;
    }

    public void UpdateCurrentJob(JobManager.JobType newJob)
    {
        currentJob = newJob.ToString();
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
        skillCooldowns.Clear();

        currentJob = skillData.jobType.ToString();

        foreach (var skill in skillData.skills)
        {
            string key = currentJob + "_" + skill.skillKey;

            if (skill.hitboxPrefab != null)
                hitboxPrefabs[key] = skill.hitboxPrefab;

            if (skill.effectPrefab != null)
                effectPrefabs[key] = skill.effectPrefab;

            skillCooldowns[skill.skillKey] = skill.cooldown;
        }

        Debug.Log($"[SkillController] {currentJob} 스킬 데이터 로드 완료");
    }

    private SkillInfo FindSkillInfoByName(string skillName)
    {
        foreach (var s in skillData.skills)
            if (s.skillName == skillName) return s;

        foreach (var baseSkill in skillData.skills)
        {
            var upgradeData = SkillUpgradeManager.Instance.GetUpgradeDataFor(baseSkill.skillName);
            if (upgradeData == null || upgradeData.upgradeOptions == null) continue;

            foreach (var upgraded in upgradeData.upgradeOptions)
                if (upgraded.skillName == skillName) return upgraded;
        }

        return null;
    }

    public float GetSkillLastUsedTime(string skillKey)
    {
        return skillLastUsedTime.TryGetValue(skillKey, out float time) ? time : -999f;
    }

    public void SetSkillLastUsedTime(string skillKey, float time)
    {
        skillLastUsedTime[skillKey] = time;
    }
}
