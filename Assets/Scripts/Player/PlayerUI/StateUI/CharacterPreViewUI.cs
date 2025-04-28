using UnityEngine;
using System.Collections;

public class CharacterPreviewUI : MonoBehaviour
{
    [Header("모델 생성 위치")]
    public Transform previewParent;

    [Header("Animator (UI 오브젝트에 고정)")]
    public Animator previewAnimator;

    [Header("스케일 및 위치 보정")]
    public float previewScale = 100f;
    public float yOffset = 0f;

    private GameObject currentPreviewModel;
    private JobManager.JobType lastJobType;

    private void OnEnable()
    {
        if (JobManager.Instance != null)
            JobManager.Instance.OnJobChanged += OnJobChanged;

        previewAnimator.enabled = true;
        StartCoroutine(LoadPreviewDelayed());
    }

    private void OnDisable()
    {
        if (JobManager.Instance != null)
            JobManager.Instance.OnJobChanged -= OnJobChanged;
    }

    private void OnJobChanged(JobManager.JobType newJob)
    {
        StartCoroutine(LoadPreviewDelayed());
    }

    private IEnumerator LoadPreviewDelayed()
    {
        yield return null; // 한 프레임 대기
        LoadCurrentJobModel();
    }

    public void LoadCurrentJobModel()
    {
        if (previewParent == null || previewAnimator == null)
        {
            Debug.LogError("[CharacterPreviewUI] previewParent 또는 previewAnimator 누락");
            return;
        }

        var job = JobManager.Instance.GetCurrentJob();
        var resources = JobManager.Instance.jobResourcesList.Find(j => j.jobType == job);
        if (resources == null || resources.modelPrefab == null || resources.avatar == null)
        {
            Debug.LogError($"[CharacterPreviewUI] 리소스 누락: {job}");
            return;
        }

        // 직업이 바뀌었을 때만 새로 생성
        if (currentPreviewModel == null || lastJobType != job)
        {
            if (currentPreviewModel != null)
                Destroy(currentPreviewModel);

            currentPreviewModel = Instantiate(resources.modelPrefab, previewParent);
            currentPreviewModel.transform.localPosition = new Vector3(0, yOffset, 0);
            currentPreviewModel.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            currentPreviewModel.transform.localScale = Vector3.one * previewScale;

            int layer = LayerMask.NameToLayer("PreviewModel");
            currentPreviewModel.layer = layer;
            foreach (Transform t in currentPreviewModel.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;

            lastJobType = job;
        }

        // Animator에 Avatar 적용만 (모델 새로 생성 안 해도)
        previewAnimator.avatar = resources.avatar;
        previewAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        previewAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        string idleState = GetIdleStateName(job);
        StartCoroutine(PlayIdleNextFrame(idleState));

        Debug.Log($"[CharacterPreviewUI] 모델 프리팹: {resources.modelPrefab.name}");
        Debug.Log($"[CharacterPreviewUI] Avatar 이름: {resources.avatar.name}");
    }

    private IEnumerator PlayIdleNextFrame(string stateName)
    {
        yield return null;
        yield return null;

        previewAnimator.enabled = true;
        previewAnimator.Play(stateName, 0, 0f);
        Debug.Log($"[CharacterPreviewUI] 상태 재생됨: {stateName}");
    }

    private string GetIdleStateName(JobManager.JobType job)
    {
        return job switch
        {
            JobManager.JobType.Basic => "Idle_Basic",
            JobManager.JobType.Warrior => "Idle_Warrior",
            JobManager.JobType.Mage => "Idle_Mage",
            JobManager.JobType.Archer => "Idle_Archer",
            _ => "Idle"
        };
    }

    public void RotatePreview(float angle)
    {
        if (currentPreviewModel != null)
            currentPreviewModel.transform.Rotate(Vector3.up, angle);
    }
}