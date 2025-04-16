using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider hpSlider;
    public Slider mpSlider;
    public Slider expSlider;
    public TextMeshProUGUI levelText;

    public void UpdateHP(float current, float max)
    {
        hpSlider.value = current / max;
    }

    public void UpdateMP(float current, float max)
    {
        mpSlider.value = current / max;
    }

    public void UpdateEXP(float current, float max)
    {
        expSlider.value = current / max;
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "Lv. " + level.ToString();
    }
}
