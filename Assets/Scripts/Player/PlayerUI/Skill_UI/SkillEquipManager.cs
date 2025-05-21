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
        //  중복 계열 검사: 자기 슬롯은 제외
        foreach (var pair in equippedSkills)
        {
            if (pair.Key == key) continue; // 자기 슬롯은 제외

            bool isSameLine =
                pair.Value == skill ||
                pair.Value.originalSkill == skill ||
                skill.originalSkill == pair.Value;

            if (isSameLine)
            {
                Debug.LogWarning($"[SkillEquipManager] {skill.skillName}는 이미 슬롯({pair.Key})에 장착되어 있습니다. 중복 불가.");
                return;
            }
        }

        //  정상 장착
        equippedSkills[key] = skill;
        skill.skillKey = key;
    }

    public Dictionary<string, SkillInfo> GetAllEquippedSkills()
    {
        return equippedSkills; // 슬롯 키 → 스킬
    }


    public SkillInfo GetEquippedSkill(string key)
    {
        if (equippedSkills.TryGetValue(key, out var skill))
            return skill;
        return null;
    }

    public string FindEquippedSlotKey(SkillInfo skill)
    {
        foreach (var kvp in equippedSkills)
        {
            if (kvp.Value == skill || kvp.Value.originalSkill == skill || skill.originalSkill == kvp.Value)
                return kvp.Key;
        }
        return null;
    }


}