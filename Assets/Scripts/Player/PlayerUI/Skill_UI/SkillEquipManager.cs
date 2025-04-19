using UnityEngine;
using System.Collections.Generic;

public class SkillEquipManager : MonoBehaviour
{
    public static SkillEquipManager Instance { get; private set; }

    private Dictionary<string, SkillInfo> equippedSkills = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void EquipSkill(string key, SkillInfo skill)
    {
        if (equippedSkills.ContainsKey(key))
            equippedSkills[key] = skill;
        else
            equippedSkills.Add(key, skill);

        skill.skillKey = key; // 실제 키 반영
    }

    public SkillInfo GetEquippedSkill(string key)
    {
        if (equippedSkills.TryGetValue(key, out var skill))
            return skill;
        return null;
    }

    public Dictionary<string, SkillInfo> GetAllEquippedSkills() => equippedSkills;
}