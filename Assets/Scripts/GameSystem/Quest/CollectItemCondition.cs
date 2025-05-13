using UnityEngine;

[CreateAssetMenu(fileName = "New Collect Item Condition", menuName = "Quests/Conditions/CollectItem")]
public class CollectItemCondition : QuestCondition
{
    public ItemData targetItem;
    public int requiredAmount;

    public override bool IsMet(PlayerQuestProgress progress)
    {
        int index = progress.questData.conditions.IndexOf(this);
        return progress.conditionProgress[index] >= requiredAmount;
    }
}