using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;

    public void Setup(ItemReward reward)
    {
        icon.sprite = reward.item.icon;
        icon.enabled = true;
        countText.text = $"x{reward.amount}";
        gameObject.SetActive(true);
    }

    public void Clear()
    {
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "";
        gameObject.SetActive(false);
    }
}