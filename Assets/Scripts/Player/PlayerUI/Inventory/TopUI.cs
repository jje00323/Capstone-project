using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUI : MonoBehaviour
{
    void OnEnable()
    {
        // 방법 1: Canvas 우선순위 조절
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = gameObject.AddComponent<Canvas>();

        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

        // 방법 2 (같은 부모 안에서): 계층 가장 뒤로 보내기
        transform.SetAsLastSibling();
    }
}