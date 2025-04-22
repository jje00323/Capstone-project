using UnityEngine;
using System.Collections.Generic;

public class SkillUpgradeManager : MonoBehaviour
{
    public static SkillUpgradeManager Instance;

    [SerializeField] private List<SkillUpgradeData> allUpgradeData;

    private void Awake()
    {
        Instance = this;
    }

    public SkillUpgradeData GetUpgradeDataFor(SkillInfo baseSkill)
    {
        if (baseSkill == null)
        {
            Debug.LogWarning("[SkillUpgradeManager] baseSkill이 null입니다.");
            return null;
        }

        //Debug.Log($"[SkillUpgradeManager] 찾는 대상: {baseSkill.skillName}");

        foreach (var data in allUpgradeData)
        {
            //Debug.Log($"[SkillUpgradeManager] 비교 중: '{data.skillName}' vs '{baseSkill.skillName}' → {data.skillName == baseSkill.skillName}");
        }

        var result = allUpgradeData.Find(data => data.skillName == baseSkill.skillName);

        //if (result == null)
        //    Debug.LogError("[SkillUpgradeManager] 일치하는 SkillUpgradeData를 찾지 못했습니다.");

        return result;
    }

    public void AutoLinkUpgradeToBase(JobSkillData[] allJobSkills)
    {
        foreach (var jobData in allJobSkills)
        {
            foreach (var baseSkill in jobData.skills)
            {
                SkillUpgradeData upgradeData = GetUpgradeDataFor(baseSkill.skillName);

                if (upgradeData == null || upgradeData.upgradeOptions == null) continue;

                foreach (var upgraded in upgradeData.upgradeOptions)
                {
                    upgraded.originalSkill = baseSkill;

                    //Debug.Log($"[링크완료] '{upgraded.skillName}' → 원본: '{upgraded.originalSkill?.skillName}'");
                }
            }
        }

    }

    // 기존 메서드에 skillName 기반 버전 추가
    public SkillUpgradeData GetUpgradeDataFor(string baseSkillName)
    {
        foreach (var data in allUpgradeData)
        {
            Debug.Log($"[SkillUpgradeManager] 비교 중: '{data.skillName}' vs '{baseSkillName}' → {data.skillName == baseSkillName}");

            if (data.skillName == baseSkillName)
            {
                return data;
            }
        }

        //Debug.LogError($"[SkillUpgradeManager] 일치하는 SkillUpgradeData를 찾지 못했습니다: {baseSkillName}");
        return null;
    }
}