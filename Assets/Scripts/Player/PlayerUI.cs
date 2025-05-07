using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Slider mpSlider;
    public TextMeshProUGUI mpText;
    public Slider expSlider;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI levelText;

    public void UpdateHP(float current, float max)
    {
        hpSlider.value = current / max;
        hpText.text = $"{current:0}/{max:0}";
    }

    public void UpdateMP(float current, float max)
    {
        mpSlider.value = current / max;
        mpText.text = $"{current:0}/{max:0}";
    }

    public void UpdateEXP(float current, float max)
    {
        expSlider.value = current / max;
        expText.text = $"{current:0}/{max:0}";
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "Lv. " + level;
    }

    public void UpdateAll(float hpCurrent, float hpMax, float mpCurrent, float mpMax, float expCurrent, float expMax, int level)
    {
        UpdateHP(hpCurrent, hpMax);
        UpdateMP(mpCurrent, mpMax);
        UpdateEXP(expCurrent, expMax);
        UpdateLevel(level);
    }
}