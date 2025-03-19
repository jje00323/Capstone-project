using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ���� ��� (�÷��̾�)
    public Vector3 offset = new Vector3(0, 7, -4.5f); // ī�޶� ��ġ ������

    void LateUpdate()
    {
        if (target != null)
        {
            // ĳ���� ��ġ + ���������� ����
            transform.position = target.position + offset;

         
        }
    }
}
