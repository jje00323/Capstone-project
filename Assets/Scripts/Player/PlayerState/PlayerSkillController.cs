using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillController : MonoBehaviour
{
    [Header("스킬 데이터 (ScriptableObject)")]
    public JobSkillData skillData;

    private Dictionary<string, GameObject> hitboxPrefabs = new();
    private Dictionary<string, GameObject> effectPrefabs = new();
    private Dictionary<string, float> skillLastUsedTime = new();
    private Dictionary<string, float> skillCooldowns = new();

    private GameObject activeEffect;
    private GameObject player;
    private string currentJob = "";

    private void Awake()
    {
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

    public void ActivateHitbox(string skillName, Vector3? mouseTarget = null)
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
            if (hitbox.useMousePosition && mouseTarget.HasValue)
                hitbox.SetFixedMousePosition(mouseTarget.Value);

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

    public float GetLastUsedTime(string skillKey)
    {
        return skillLastUsedTime.TryGetValue(skillKey, out float lastTime) ? lastTime : -999f;
    }

    public void SaveSkillUseTime(string skillKey)
    {
        skillLastUsedTime[skillKey] = Time.time;
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
}