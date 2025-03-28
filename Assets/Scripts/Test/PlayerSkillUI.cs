using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerSkillUI : MonoBehaviour
{
    public JobSkillData skillData;
    public GameObject skillButtonPrefab;
    public Transform skillButtonParent;

    void Start()
    {
        LoadSkillButtons(skillData);
    }

    void LoadSkillButtons(JobSkillData data)
    {
        foreach (SkillInfo skill in data.skills)
        {
            GameObject btnObj = Instantiate(skillButtonPrefab, skillButtonParent);
            btnObj.SetActive(true);
            // 아이콘 설정
            Image btnImage = btnObj.GetComponent<Image>();
            if (btnImage != null && skill.skillIcon != null)
                btnImage.sprite = skill.skillIcon;

            // CooldownOverlay, CooldownText 가져오기
            Image cooldownOverlay = btnObj.transform.Find("CooldownOverlay").GetComponent<Image>();
            TextMeshProUGUI cooldownText = btnObj.transform.Find("CooldownText").GetComponent<TextMeshProUGUI>();

            // 초기에 꺼두기
            cooldownOverlay.gameObject.SetActive(false);
            cooldownText.gameObject.SetActive(false);

            // 버튼 클릭 이벤트 추가
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(StartCooldown(skill, cooldownOverlay, cooldownText));
                // 실제 스킬 실행 코드 (예: PlayerSkill.Instance.TrySkillByKey(skill.skillKey));
                Debug.Log(skill.skillName + " 사용!");
            });
        }
    }

    IEnumerator StartCooldown(SkillInfo skill, Image overlay, TextMeshProUGUI cooldownText)
    {
        float cooldown = skill.cooldown;
        overlay.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);

        float timer = cooldown;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            cooldownText.text = Mathf.CeilToInt(timer).ToString() + "s";

            // Overlay의 fill amount로 쿨타임 시각적 표시 (선택적)
            overlay.fillAmount = timer / cooldown;

            yield return null;
        }

        overlay.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
    }
}