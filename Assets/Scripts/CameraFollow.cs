using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 9f, -4.8f); // 위치 오프셋
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = 70f; // FOV 설정
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 위치 고정
            transform.position = target.position + offset;

            // 각도 고정
            transform.rotation = Quaternion.Euler(55f, 0f, 0f);
        }
    }
}
