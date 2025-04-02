using UnityEngine;

[CreateAssetMenu(fileName = "JobSkillData", menuName = "Skills/JobSkillData")]
public class JobSkillData : ScriptableObject
{
    public JobManager.JobType jobType;   // 직업 유형 (Warrior, Mage 등)
    public SkillInfo[] skills;           // 직업이 가진 모든 스킬
}

[System.Serializable]
public class SkillInfo
{
    public string skillKey;     // 키 (Q, W, E, R)
    public string skillName;    // 스킬 이름
    public Sprite skillIcon;    // 스킬 아이콘
    public string description;  // 설명
    public float cooldown;      // 쿨타임
    public GameObject hitboxPrefab; // 해당 스킬의 히트박스 프리팹 (선택적)
    public GameObject effectPrefab; // 스킬 이펙트 프리팹 (선택적)
    public float effectDuration; // 이펙트 지속시간
}
