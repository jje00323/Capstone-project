using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillController : MonoBehaviour
{
    [Header("스킬 데이터 (ScriptableObject)")]
    public JobSkillData skillData;

    private Dictionary<string, GameObject> hitboxPrefabs = new();
    private Dictionary<string, GameObject> effectPrefabs = new();
    private Dictionary<string, float> skillLastUsedTime = new();
    private Dictionary<string, float> skillCooldowns = new();

    private GameObject activeEffect;
    private string currentJob = "";

    private Vector3? pendingMouseTarget = null;

    private PlayerSkill playerSkill;
    private PlayerInputHandler inputHandler;
    private PlayerControls controls;

    private void Awake()
    {
        playerSkill = GetComponent<PlayerSkill>();
        inputHandler = GetComponent<PlayerInputHandler>();
        controls = new PlayerControls();

        LoadSkillsFromData(skillData);
    }

    public void RegisterSkillKey(string skillKey)
    {
        var action = controls.asset.FindAction($"QWER/{skillKey}");
        if (action == null)
        {
            Debug.LogWarning($"[SkillController] {skillKey}에 대한 InputAction이 존재하지 않음");
            return;
        }

        action.performed += ctx =>
        {
            SkillInfo equipped = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
            if (equipped == null) return;

            if (CanCast(skillKey) && playerSkill.CanUseSkill())
            {
                PrepareSkill(skillKey);
                playerSkill.StartSkill(GetAnimationName(skillKey));
                inputHandler.BlockRightClickForOneFrame();
            }
        };
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

    public bool CanCast(string skillKey)
    {
        return Time.time - GetLastUsedTime(skillKey) >= GetSkillCooldown(skillKey);
    }

    public void PrepareSkill(string skillKey)
    {
        pendingMouseTarget = Hitbox.MouseUtility.GetMouseWorldPosition(LayerMask.GetMask("Ground"));
        skillLastUsedTime[skillKey] = Time.time;
    }

    public string GetAnimationName(string skillKey)
    {
        var skill = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
        return skill?.skillAnimation?.name;
    }

    public void ActivateHitbox(string skillName)
    {
        SkillInfo skillInfo = GetSkillInfo(skillName);

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
        SkillInfo skill = GetSkillInfo(skillName);
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

    public void UpdateCurrentJob(JobManager.JobType newJob)
    {
        currentJob = newJob.ToString();
        LoadSkillsFromData(skillData);
    }

    public Vector3? GetPendingMouseTarget()
    {
        return pendingMouseTarget;
    }

    private float GetSkillCooldown(string skillKey)
    {
        return skillCooldowns.TryGetValue(skillKey, out float cooldown) ? cooldown : 0f;
    }

    private float GetLastUsedTime(string skillKey)
    {
        return skillLastUsedTime.TryGetValue(skillKey, out float lastTime) ? lastTime : -999f;
    }

    private SkillInfo GetSkillInfo(string skillKey)
    {
        foreach (var skill in skillData.skills)
        {
            if (skill.skillKey == skillKey)
                return skill;
        }
        return null;
    }
}