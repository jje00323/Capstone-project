using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionUI : MonoBehaviour
{
    public CharacterSelectCameraController cameraController;

    // 인덱스 순서대로 직업 지정 (카메라 위치 순서와 동일해야 함)
    public JobManager.JobType[] jobTypePerIndex;

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
        int index = cameraController.currentIndex; // ↓ 이걸 가져올 수 있도록 cameraController에 프로퍼티 추가 필요
        if (index < 0 || index >= jobTypePerIndex.Length)
        {
            Debug.LogError("[CharacterSelectionUI] 유효하지 않은 직업 인덱스입니다.");
            return;
        }

        JobManager.JobType selectedJob = jobTypePerIndex[index];
        JobManager.Instance.currentJob = selectedJob;
        //JobManager.Instance.autoApplyJobOnStart = true;

        Debug.Log($"[CharacterSelectionUI] 선택된 직업: {selectedJob}");

        SceneManager.LoadScene("MainGameScene"); // 또는 "TutorialScene"
    }
}