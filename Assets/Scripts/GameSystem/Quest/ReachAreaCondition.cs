using UnityEngine;

[CreateAssetMenu(fileName = "New Reach Area Condition", menuName = "Quests/Conditions/ReachArea")]
public class ReachAreaCondition : QuestCondition
{
    public string areaID;

    public override bool IsMet(PlayerQuestProgress progress)
    {
        int index = progress.questData.conditions.IndexOf(this);
        return progress.conditionProgress[index] >= 1;
    }
}