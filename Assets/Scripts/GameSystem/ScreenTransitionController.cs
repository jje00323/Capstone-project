using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScreenTransitionController : MonoBehaviour
{
    public static ScreenTransitionController Instance;

    public CanvasGroup blackoutOverlay;
    public TextMeshProUGUI centerText;
    public float fadeDuration = 2f;

    private bool waitingForInput = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        blackoutOverlay.alpha = 0f;
        centerText.gameObject.SetActive(false);
    }

    public void StartRelicCutscene()
    {
        StartCoroutine(RelicCutsceneRoutine());
    }

    private IEnumerator RelicCutsceneRoutine()
    {
        // 1. 어둡게 페이드인
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            blackoutOverlay.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        // 2. 글귀 1
        centerText.gameObject.SetActive(true);
        centerText.text = "봉인의 열쇠는 하나의 생명\n열쇠는 시간의 문을 열고,\n한 생명을 대가로 과거의 영웅을 현실로 이끌었다";
        yield return new WaitForSeconds(3f);

        // 3. 글귀 2
        centerText.text = "한때 유물을 지키던 자들\n역사에 묻혔던 그들의 칼날이,\n다시 운명을 향해 겨눠진다.";
        yield return new WaitForSeconds(3f);

        // 4. [F] 입력 유도
        centerText.text = "<size=150%>[G]</size>\n계승할 영웅을 선택하세요";
        waitingForInput = true;
    }

    private void Update()
    {
        if (waitingForInput && Input.GetKeyDown(KeyCode.G))
        {
            waitingForInput = false;
            SceneManager.LoadScene("Charactor_Scene"); // 정확한 씬 이름으로 바꿔주세요
        }
    }
}