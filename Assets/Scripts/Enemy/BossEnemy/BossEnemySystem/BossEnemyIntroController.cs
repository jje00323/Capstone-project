using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyIntroController : MonoBehaviour
{
    public static BossEnemyIntroController Instance;

    [Header("블랙아웃 오버레이")]
    [SerializeField] private CanvasGroup overlayGroup;

    [Header("UI 요소들")]
    [SerializeField] private CanvasGroup playerUIGroup;
    [SerializeField] private CanvasGroup bossUIGroup;
    [SerializeField] private List<GameObject> uiToToggleDuringIntro;

    private Dictionary<GameObject, bool> uiActiveStates = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 페이드 아웃: 어둡게
    public void FadeOut(float duration, System.Action onComplete = null)
    {
        StartCoroutine(Fade(1f, duration, onComplete));
    }

    // 페이드 인: 밝게
    public void FadeIn(float duration, System.Action onComplete = null)
    {
        StartCoroutine(Fade(0f, duration, onComplete));
    }

    private IEnumerator Fade(float targetAlpha, float duration, System.Action onComplete)
    {
        float startAlpha = overlayGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            overlayGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        overlayGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }

    public void HideAll()
    {
        uiActiveStates.Clear();

        SetCanvasGroup(playerUIGroup, false);
        SetCanvasGroup(bossUIGroup, false);

        foreach (var ui in uiToToggleDuringIntro)
        {
            if (ui != null)
            {
                uiActiveStates[ui] = ui.activeSelf;
                ui.SetActive(false);
            }
        }
    }

    public void ShowAll()
    {
        SetCanvasGroup(playerUIGroup, true);
        SetCanvasGroup(bossUIGroup, true);

        foreach (var kvp in uiActiveStates)
        {
            if (kvp.Key != null)
                kvp.Key.SetActive(kvp.Value);
        }
    }

    private void SetCanvasGroup(CanvasGroup group, bool active)
    {
        if (group == null) return;

        group.alpha = active ? 1f : 0f;
        group.interactable = active;
        group.blocksRaycasts = active;
    }
}
