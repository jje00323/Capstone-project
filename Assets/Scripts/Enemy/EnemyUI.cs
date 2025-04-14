using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public GameObject hpBarGroup;
    public Slider hpSlider;

    private Coroutine hideCoroutine;
    private Transform mainCam;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Main Camera를 찾을 수 없습니다. EnemyUI가 회전하지 않습니다.");
        }
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            transform.rotation = Quaternion.LookRotation(mainCam.forward, mainCam.up);
        }
    }

    public void ShowHP(float current, float max)
    {
        Debug.Log($"[EnemyUI] ShowHP 호출됨: {current}/{max}");

        if (hpSlider == null)
        {
            Debug.LogWarning("EnemyUI: 컴포넌트 누락");
            return;
        }

        hpSlider.value = current / max;
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hpBarGroup.SetActive(false);
    }
}



