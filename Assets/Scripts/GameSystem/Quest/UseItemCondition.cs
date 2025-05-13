using UnityEngine;

[CreateAssetMenu(fileName = "New Use Item Condition", menuName = "Quests/Conditions/UseItem")]
public class UseItemCondition : QuestCondition
{
    public ItemData requiredItem;
    public int requiredUseCount = 1;

    public override bool IsMet(PlayerQuestProgress progress)
    {
        int index = progress.questData.conditions.IndexOf(this);
        return progress.conditionProgress[index] >= requiredUseCount;
    }
}