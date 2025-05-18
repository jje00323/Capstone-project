using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/QuestData")]


public class QuestData : ScriptableObject
{
    [Header("기본 정보")]
    public int questID;
    public string questTitle;
    [TextArea]
    public string questObjective;
    public string questDescription;
    

    public QuestType questType;
    public int levelRequirement = 1;
    public int nextQuestID = -1; // 다음 퀘스트 ID (없으면 -1)

    [Header("수행 조건")]
    public List<QuestCondition> conditions;

    [Header("보상")]
    public QuestReward reward;

    [Header("퀘스트 시작 내레이션")]
    [TextArea]
    public string startNarration; // 퀘스트 시작 시 출력될 혼잣말 또는 설명
}