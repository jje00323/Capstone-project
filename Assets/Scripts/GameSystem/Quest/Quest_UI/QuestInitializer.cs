using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInitializer : MonoBehaviour
{
    [SerializeField] private List<QuestData> defaultQuests;

    void Start()
    {
        Debug.Log("[QuestInitializer] 메인 게임 씬 진입 → 퀘스트 강제 초기화 및 재등록");

        // 1. 이전 퀘스트 전부 제거
        QuestManager.Instance.ResetAllQuests();

        // 2. defaultQuests 무조건 재등록
        foreach (var quest in defaultQuests)
        {
            QuestManager.Instance.AcceptQuest(quest);
            Debug.Log($"[QuestInitializer] 퀘스트 재등록 완료: {quest.questTitle}");
        }
    }
}