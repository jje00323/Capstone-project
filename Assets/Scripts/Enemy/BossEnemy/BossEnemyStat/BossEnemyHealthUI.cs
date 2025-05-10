using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossEnemyHealthUI : MonoBehaviour
{
    [Header("보스 참조")]
    public BossEnemyStatus bossStatus;

    [Header("체력바 이미지")]
    [SerializeField] private Image frontFill;
    [SerializeField] private Image backFill;
    [SerializeField] private Image background;

    [Header("보스 체력 표시")]
    [SerializeField] private TextMeshProUGUI hpBarCountText;    // 보스 체력 줄 수
    [SerializeField] private TextMeshProUGUI hpValueText;       // 현재 체력 / 최대 체력
    [SerializeField] private TextMeshProUGUI bossLevelText;     // 보스 레벨

    [Header("색상 설정")]
    public Color[] hpColors;

    private float segmentHP;
    private int totalBars;
    private float displayedRatio = 1f;
    private bool initialized = false;

    private void Update()
    {
        if (!initialized)
        {
            Debug.Log("[BossHPUI] 초기화 시도 중...");
            if (bossStatus == null)
            {
                Debug.LogError("[BossHPUI] bossStatus가 연결되지 않았습니다.");
                return;
            }

            if (bossStatus.bossData == null)
            {
                Debug.LogWarning("[BossHPUI] bossStatus는 있지만 bossData가 아직 null입니다.");
                return;
            }

            totalBars = bossStatus.bossData.hpBarCount;
            segmentHP = bossStatus.maxHP / totalBars;
            initialized = true;

            Debug.Log($"[BossHPUI] 초기화 완료: segmentHP = {segmentHP}, totalBars = {totalBars}");
        }

        float currentHP = bossStatus.currentHP;
        int currentBarIndex = Mathf.FloorToInt(currentHP / segmentHP);
        float currentBarHP = currentHP % segmentHP;

        if (Mathf.Approximately(currentBarHP, 0) && currentHP > 0)
        {
            currentBarIndex -= 1;
            currentBarHP = segmentHP;
        }

        float targetRatio = currentBarHP / segmentHP;
        displayedRatio = Mathf.Lerp(displayedRatio, targetRatio, Time.deltaTime * 10f);
        frontFill.fillAmount = displayedRatio;
        backFill.fillAmount = (currentBarIndex > 0) ? 1f : 0f;

        // 체력 줄 수 표시 (남은 줄 수 기준)
        int remainingBars = Mathf.CeilToInt(currentHP / segmentHP);
        hpBarCountText.text = $"x{remainingBars}";

        // 체력 수치 표시
        hpValueText.text = $"{Mathf.CeilToInt(currentHP)} / {Mathf.CeilToInt(bossStatus.maxHP)}";

        // 보스 레벨 표시
        if (bossStatus.bossData != null)
        {
            bossLevelText.text = $"Lv.{bossStatus.bossData.bossLevel}";
        }

        // 색상 처리
        if (hpColors != null && hpColors.Length > 0)
        {
            // 1. segmentHP = maxHP / totalBars → 초기화 시 이미 계산됨
            // 2. 현재 체력 기준 몇 번째 segment에 해당하는지 (0부터 시작)
            int currentSegment = Mathf.FloorToInt((bossStatus.maxHP - currentHP) / segmentHP);

            // 3. 순환되는 색상 인덱스 계산
            int colorIndex = currentSegment % hpColors.Length;
            int nextColorIndex = (colorIndex + 1) % hpColors.Length;

            // 4. 색상 적용
            frontFill.color = hpColors[colorIndex];
            backFill.color = hpColors[nextColorIndex];
        }

        //Debug.Log($"[BossHPUI] FrontFill.fillAmount = {frontFill.fillAmount}, color = {frontFill.color}");

        //Debug.Log($"[BossHPUI] HP: {currentHP} / Segment: {segmentHP}, Bars: {currentBarIndex}");
    }

    public void SetBoss(BossEnemyStatus status)
    {
        bossStatus = status;
        initialized = false; // 다시 초기화 가능하도록
    }
}
