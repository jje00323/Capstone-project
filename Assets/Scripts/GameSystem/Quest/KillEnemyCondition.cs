using UnityEngine;

[CreateAssetMenu(fileName = "New Kill Enemy Condition", menuName = "Quests/Conditions/KillEnemy")]
public class KillEnemyCondition : QuestCondition
{
    public string targetEnemyTag;
    public int requiredKillCount;

    public override bool IsMet(PlayerQuestProgress progress)
    {
        int index = progress.questData.conditions.IndexOf(this);
        return progress.conditionProgress[index] >= requiredKillCount;
    }
}