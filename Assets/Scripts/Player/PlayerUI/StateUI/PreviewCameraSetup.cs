using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class PreviewCameraSetup : MonoBehaviour
{
    void Start()
    {
        var cam = GetComponent<Camera>();
        var urpData = GetComponent<UniversalAdditionalCameraData>();
        if (urpData != null)
        {
            // 배경을 SolidColor + 알파 0으로 설정
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0); // 완전 투명
        }
        else
        {
            Debug.LogWarning("[PreviewCameraSetup] URP 카메라 데이터를 찾을 수 없습니다.");
        }
    }
}