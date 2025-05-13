using UnityEngine;

[CreateAssetMenu(fileName = "New Talk To NPC Condition", menuName = "Quests/Conditions/TalkToNPC")]
public class TalkToNPCCondition : QuestCondition
{
    public string npcID;

    public override bool IsMet(PlayerQuestProgress progress)
    {
        int index = progress.questData.conditions.IndexOf(this);
        return progress.conditionProgress[index] >= 1;
    }
}