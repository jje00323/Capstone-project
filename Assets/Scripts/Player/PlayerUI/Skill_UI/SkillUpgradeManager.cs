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

        Debug.Log($"[SkillUpgradeManager] 찾는 대상: {baseSkill.skillName}");

        foreach (var data in allUpgradeData)
        {
            Debug.Log($"[SkillUpgradeManager] 비교 중: '{data.skillName}' vs '{baseSkill.skillName}' → {data.skillName == baseSkill.skillName}");
        }

        var result = allUpgradeData.Find(data => data.skillName == baseSkill.skillName);

        if (result == null)
            Debug.LogError("[SkillUpgradeManager] 일치하는 SkillUpgradeData를 찾지 못했습니다.");

        return result;
    }
}