using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum QuestState
{
    NotStarted,
    InProgress,
    Completed,
    Rewarded
}
[System.Serializable]
public class PlayerQuestProgress
{
    public QuestData questData;
    public QuestState state;

    // 각 조건에 대한 진행 수치 (조건 리스트의 인덱스와 매칭)
    public List<int> conditionProgress = new List<int>();

    public PlayerQuestProgress(QuestData data)
    {
        questData = data;
        state = QuestState.InProgress;
        // 조건 수만큼 초기화 (모두 0부터 시작)
        for (int i = 0; i < data.conditions.Count; i++)
        {
            conditionProgress.Add(0);
        }
    }

    // 진행 수치 업데이트 (예: 슬라임 처치 수 1 증가)
    public void IncrementProgress(int conditionIndex, int amount = 1)
    {
        if (conditionIndex >= 0 && conditionIndex < conditionProgress.Count)
        {
            conditionProgress[conditionIndex] += amount;
        }
    }

    // 퀘스트의 모든 조건이 충족되었는지 확인
    public bool IsAllConditionsMet()
    {
        for (int i = 0; i < questData.conditions.Count; i++)
        {
            if (!questData.conditions[i].IsMet(this))
                return false;
        }
        return true;
    }
}

