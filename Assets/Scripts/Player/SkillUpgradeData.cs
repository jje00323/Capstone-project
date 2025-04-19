using UnityEngine;

[CreateAssetMenu(fileName = "SkillUpgradeData", menuName = "Skills/SkillUpgradeData")]
public class SkillUpgradeData : ScriptableObject
{
    public string skillName;                    // 원본 스킬
    public SkillInfo[] upgradeOptions;        // 최대 3개의 업그레이드 옵션

    public bool HasUpgradeOptions() => upgradeOptions != null && upgradeOptions.Length > 0;
}