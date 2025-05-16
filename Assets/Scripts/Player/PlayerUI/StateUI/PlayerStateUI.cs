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


    private PlayerStatus Player => GameObject.FindWithTag("Player")?.GetComponent<PlayerStatus>();
    public static PlayerStateUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        Debug.Log("[PlayerStateUI] UpdateStats() 호출됨");

        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("[PlayerStateUI] Player 오브젝트를 찾을 수 없습니다.");
            return;
        }

        var status = playerObj.GetComponent<PlayerStatus>();
        if (status == null)
        {
            Debug.LogWarning("[PlayerStateUI] PlayerStatus 컴포넌트가 없습니다.");
            return;
        }

        Debug.Log($"[PlayerStateUI] 상태 적용 중 → maxHP: {status.maxHP}, ATK: {status.attack}, CritRate: {status.critRate}");

        if (maxHPText == null || attackText == null)
        {
            Debug.LogError("[PlayerStateUI] UI 텍스트 컴포넌트 연결 안됨");
            return;
        }

        maxHPText.text = $"{status.maxHP:F0}";
        maxMPText.text = $"{status.maxMP:F0}";
        attackText.text = $"{status.attack:F0}";
        defenseText.text = $"{status.defense:F0}";
        critRateText.text = $"{status.critRate:F0}%";
        critDamageText.text = $"{status.critDamage:F0}%";
        healthRegenText.text = "-";
        manaRegenText.text = "-";
    }
}