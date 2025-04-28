using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class PreviewCameraSetup : MonoBehaviour
{
    private Camera cam;
    private RectTransform referenceRect; // 추가

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("[PreviewCameraSetup] 카메라 컴포넌트가 없습니다.");
            return;
        }

        var urpData = GetComponent<UniversalAdditionalCameraData>();
        if (urpData != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0); // 완전 투명
        }
        else
        {
            Debug.LogWarning("[PreviewCameraSetup] URP 데이터가 없습니다. (무시 가능)");
        }

    }

    void Update()
    {
        if (referenceRect == null) return;

        float height = referenceRect.rect.height;
        float width = referenceRect.rect.width;
        float aspect = width / height;

        cam.orthographicSize = (height / 2f) / 100f;
        // 100은 Canvas 스케일 팩터에 따라 조정 필요
    }

    public void SetReferenceRect(RectTransform rect)
    {
        referenceRect = rect;
    }
}