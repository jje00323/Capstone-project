using UnityEngine;
using TMPro;

public class PlayerStateUI : MonoBehaviour
{
    [Header("스탯 UI 연결")]
    public TextMeshProUGUI maxHPText;
    public TextMeshProUGUI maxMPText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI critRateText;
    public TextMeshProUGUI critDamageText;
    public TextMeshProUGUI healthRegenText;
    public TextMeshProUGUI manaRegenText;

    private PlayerStatus player;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerStatus>();
        if (player == null)
        {
            Debug.LogError("[PlayerStateUI] PlayerStatus를 찾을 수 없습니다.");
            return;
        }

        UpdateStats();
    }

    public void UpdateStats()
    {
        if (player == null) return;

        maxHPText.text = $"{player.maxHP:F0}";
        maxMPText.text = $"{player.maxMP:F0}";
        attackText.text = $"{player.attack:F1}";
        defenseText.text = $"{player.defense:F1}";
        critRateText.text = $"{player.critRate * 100f:F1}%";
        critDamageText.text = $"{player.critDamage * 100f:F1}%";

        // 현재는 고정값 또는 외부 연산 필요
        healthRegenText.text = "-";  // 필요 시 playerStatus에서 값을 가져오도록 확장
        manaRegenText.text = "-";
    }
}