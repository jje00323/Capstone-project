using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    public CharacterSelectCameraController cameraController;

    [Header("직업 매핑 (카메라 인덱스 ↔ 직업)")]
    public JobManager.JobType[] jobTypePerIndex;

    [System.Serializable]
    public class JobInfo
    {
        public JobManager.JobType jobType;
        public string displayName;
        [TextArea(2, 5)] public string description;
        [Range(1, 5)] public int difficulty = 3;

        public string recommendedFor;      // 추천 대상 (예: 초보자, 숙련자)
        [TextArea(1, 3)]
        public string playstyleSummary;    // 플레이스타일 요약 
    }


    [Header("직업 정보")]
    public List<JobInfo> jobInfos;

    [Header("UI 참조")]
    public TextMeshProUGUI jobNameText;
    public TextMeshProUGUI jobDescriptionText;

    [Header("난이도 표시")]
    public Image[] difficultyImages; // 총 5개가 할당되어야 함
    public Sprite filledSprite;
    public Sprite emptySprite;

    [Header("추천 및 스타일")]
    public TextMeshProUGUI recommendedText;
    public TextMeshProUGUI playstyleText;

    void Start()
    {
        if (cameraController != null)
        {
            cameraController.OnCameraIndexChanged += UpdateJobInfoUI;

            // 시작 시 초기 직업 정보 표시
            UpdateJobInfoUI(cameraController.CurrentIndex);
        }
    }

    public void OnClickNext()
    {
        cameraController.Next();
    }

    public void OnClickPrev()
    {
        cameraController.Prev();
    }

    public void OnClickConfirm()
    {
        int index = cameraController.CurrentIndex;
        QuestManager.Instance.ResetAllQuests();
        Debug.Log("[CharacterSelection] 퀘스트 초기화 호출됨");
        if (index < 0 || index >= jobTypePerIndex.Length)
        {
            Debug.LogError("[CharacterSelectionUI] 유효하지 않은 직업 인덱스입니다.");
            return;
        }

        JobManager.JobType selectedJob = jobTypePerIndex[index];
        JobManager.Instance.currentJob = selectedJob;

        Debug.Log($"[CharacterSelectionUI] 선택된 직업: {selectedJob}");

        SceneManager.LoadScene("MainGameScene"); // 또는 "TutorialScene"
    }

    public void UpdateJobInfoUI(int index)
    {
        if (index < 0 || index >= jobInfos.Count)
        {
            Debug.LogWarning("[CharacterSelectionUI] 유효하지 않은 인덱스입니다.");
            return;
        }

        var info = jobInfos[index];
        jobNameText.text = info.displayName;
        jobDescriptionText.text = info.description;

        recommendedText.text = $"{info.recommendedFor}";
        playstyleText.text = $"\"{info.playstyleSummary}\"";

        for (int i = 0; i < difficultyImages.Length; i++)
        {
            difficultyImages[i].sprite = (i < info.difficulty) ? filledSprite : emptySprite;
        }
    }
}