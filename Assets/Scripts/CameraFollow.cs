using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 7, -4.5f); // 카메라 위치 오프셋

    void LateUpdate()
    {
        if (target != null)
        {
            // 캐릭터 위치 + 오프셋으로 고정
            transform.position = target.position + offset;

         
        }
    }
}
