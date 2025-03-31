using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public GameObject hpBarGroup;
    public Slider hpSlider;

    private Coroutine hideCoroutine;

    public void ShowHP(float current, float max)
    {
        if (hpBarGroup != null)
        {
            hpBarGroup.SetActive(true);
            hpSlider.value = current / max;

            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);

            hideCoroutine = StartCoroutine(HideAfterDelay(3f));
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hpBarGroup.SetActive(false);
    }
}

