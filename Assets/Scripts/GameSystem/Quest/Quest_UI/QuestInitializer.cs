using UnityEngine;

public class QuestInitializer : MonoBehaviour
{
    [Header("초기 퀘스트 설정")]
    [SerializeField] private QuestData[] defaultQuests;

    void Start()
    {
        foreach (var quest in defaultQuests)
        {
            if (!QuestManager.Instance.IsQuestActive(quest.questID))
            {
                QuestManager.Instance.AcceptQuest(quest);
                Debug.Log($"[초기화] 기본 퀘스트 수락됨: {quest.questTitle}");
            }
        }
    }
}