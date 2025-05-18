using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections;
//using System.Collections.Generic;

public class PlayerSkillUI : MonoBehaviour
{

}
//{
//    [Header("스킬 데이터")]
//    public JobSkillData skillData;

//    [Header("버튼 프리팹 & 부모")]
//    public GameObject skillButtonPrefab;
//    public Transform skillButtonParent;

//    private Dictionary<string, Coroutine> cooldownCoroutines = new();
//    private Dictionary<string, (Image overlay, TextMeshProUGUI text)> uiElements = new();

//    public static PlayerSkillUI Instance { get; private set; }

//    [HideInInspector] public PlayerSkillController skillController; // 연결 필요

//    private void Awake()
//    {
//        Instance = this;
//    }

//    private void Start()
//    {
//        if (skillController == null)
//            skillController = FindObjectOfType<PlayerSkillController>();

//        LoadSkillButtons(skillData);
//    }

//    public void LoadSkillButtons(JobSkillData data)
//    {
//        foreach (SkillInfo skill in data.skills)
//        {
//            GameObject btnObj = Instantiate(skillButtonPrefab, skillButtonParent);
//            btnObj.SetActive(true);

//            Image btnImage = btnObj.GetComponent<Image>();
//            if (btnImage != null && skill.skillIcon != null)
//                btnImage.sprite = skill.skillIcon;

//            Image cooldownOverlay = btnObj.transform.Find("CooldownOverlay")?.GetComponent<Image>();
//            TextMeshProUGUI cooldownText = btnObj.transform.Find("CooldownText")?.GetComponent<TextMeshProUGUI>();

//            if (cooldownOverlay != null) cooldownOverlay.gameObject.SetActive(false);
//            if (cooldownText != null) cooldownText.gameObject.SetActive(false);

//            uiElements[skill.skillKey] = (cooldownOverlay, cooldownText);

//            btnObj.GetComponent<Button>().onClick.AddListener(() =>
//            {
//                skillController?.TryUseSkill(skill.skillKey);
//                StartUICooldown(skill.skillKey, skill.cooldown);
//            });
//        }
//    }

//    public void StartUICooldown(string skillKey, float cooldown)
//    {
//        if (!uiElements.ContainsKey(skillKey)) return;

//        if (cooldownCoroutines.ContainsKey(skillKey))
//            StopCoroutine(cooldownCoroutines[skillKey]);

//        cooldownCoroutines[skillKey] = StartCoroutine(UICooldownRoutine(skillKey, cooldown));
//    }

//    private IEnumerator UICooldownRoutine(string skillKey, float cooldown)
//    {
//        var (overlay, text) = uiElements[skillKey];
//        if (overlay == null || text == null) yield break;

//        overlay.gameObject.SetActive(true);
//        text.gameObject.SetActive(true);

//        float timer = cooldown;
//        while (timer > 0f)
//        {
//            timer -= Time.deltaTime;
//            text.text = Mathf.CeilToInt(timer).ToString() + "s";
//            overlay.fillAmount = timer / cooldown;
//            yield return null;
//        }

//        overlay.gameObject.SetActive(false);
//        text.gameObject.SetActive(false);
//    }

//    public void ReloadUI(JobSkillData newData)
//    {
//        skillData = newData;

//        // 기존 버튼 정리
//        foreach (Transform child in skillButtonParent)
//        {
//            Destroy(child.gameObject);
//        }

//        uiElements.Clear();
//        cooldownCoroutines.Clear();

//        // 새로운 데이터로 다시 로드
//        LoadSkillButtons(skillData);
//    }

//}
