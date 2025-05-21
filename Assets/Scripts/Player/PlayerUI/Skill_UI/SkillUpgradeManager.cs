//  SkillUpgradeManager.cs 수정
using UnityEngine;
using System.Collections.Generic;

public class SkillUpgradeManager : MonoBehaviour
{
    public static SkillUpgradeManager Instance;

    [SerializeField] private List<SkillUpgradeData> allUpgradeData;

    //  현재 업그레이드 상태 추적용 맵
    private Dictionary<SkillInfo, SkillInfo> upgradeMap = new();

    private void Awake()
    {
        Instance = this;
    }

    public SkillUpgradeData GetUpgradeDataFor(SkillInfo baseSkill)
    {
        if (baseSkill == null) return null;
        return allUpgradeData.Find(data => data.skillName == baseSkill.skillName);
    }

    public SkillUpgradeData GetUpgradeDataFor(string baseSkillName)
    {
        return allUpgradeData.Find(data => data.skillName == baseSkillName);
    }

    public void AutoLinkUpgradeToBase(JobSkillData[] allJobSkills)
    {
        foreach (var jobData in allJobSkills)
        {
            foreach (var baseSkill in jobData.skills)
            {
                SkillUpgradeData upgradeData = GetUpgradeDataFor(baseSkill);
                if (upgradeData == null || upgradeData.upgradeOptions == null) continue;

                foreach (var upgraded in upgradeData.upgradeOptions)
                {
                    upgraded.originalSkill = baseSkill;
                }
            }
        }
    }

    //  업그레이드 적용 기록
    public void SetCurrentUpgrade(SkillInfo original, SkillInfo upgraded)
    {
        if (upgradeMap.ContainsKey(original))
            upgradeMap[original] = upgraded;
        else
            upgradeMap.Add(original, upgraded);
    }

    //  현재 선택된 버전 반환
    public SkillInfo GetCurrentVersionOf(SkillInfo baseSkill)
    {
        if (upgradeMap.TryGetValue(baseSkill, out var upgraded))
            return upgraded;
        return baseSkill;
    }

    //  업그레이드 초기화 (예: 리셋 시)
    public void ClearUpgrades()
    {
        upgradeMap.Clear();
    }


}
