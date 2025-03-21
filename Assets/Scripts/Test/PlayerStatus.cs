using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP = 100f;

    public float maxMP = 50f;
    public float currentMP = 50f;

    public float maxEXP = 100f;
    public float currentEXP = 0f;

    public int level = 1;

    public PlayerUI playerUI;

    void Start()
    {
        UpdateAllUI();
    }

    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        playerUI.UpdateHP(currentHP, maxHP);
        Debug.Log("TakeDamage() 호출됨, 현재 체력: " + currentHP);
    }
    public void Heal(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        playerUI.UpdateHP(currentHP, maxHP);
    }

    public void UseMana(float amount)
    {
        currentMP = Mathf.Clamp(currentMP - amount, 0, maxMP);
        playerUI.UpdateMP(currentMP, maxMP);
    }

    public void GainEXP(float amount)
    {
        currentEXP += amount;
        if (currentEXP >= maxEXP)
        {
            currentEXP -= maxEXP;
            level++;
            // 레벨업시 maxEXP 증가 같은 로직 추가 가능
            playerUI.UpdateLevel(level);
        }
        playerUI.UpdateEXP(currentEXP, maxEXP);
    }

    public void UpdateAllUI()
    {
        playerUI.UpdateHP(currentHP, maxHP);
        playerUI.UpdateMP(currentMP, maxMP);
        playerUI.UpdateEXP(currentEXP, maxEXP);
        playerUI.UpdateLevel(level);
    }
}
