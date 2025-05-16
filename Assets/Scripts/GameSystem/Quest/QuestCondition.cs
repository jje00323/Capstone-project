using UnityEngine;

public abstract class QuestCondition : ScriptableObject
{
    public string conditionName;
    public abstract bool IsMet(PlayerQuestProgress progress);
}