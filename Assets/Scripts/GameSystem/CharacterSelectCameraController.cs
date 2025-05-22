using System.Collections;
using UnityEngine;

public class CharacterSelectCameraController : MonoBehaviour
{
    public Transform[] cameraPositions;
    public float transitionSpeed = 2f;

    public int currentIndex = 0;
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main?.transform;

        if (camTransform == null)
        {
            Debug.LogError("[CameraController] Main Camera가 씬에 없습니다. 태그가 'MainCamera'인지 확인하세요.");
            return;
        }

        if (cameraPositions == null || cameraPositions.Length == 0)
        {
            Debug.LogError("[CameraController] cameraPositions 배열이 비어 있습니다. 인스펙터에서 설정했는지 확인하세요.");
            return;
        }

        Debug.Log("[CameraController] Start() → 초기 카메라 위치 설정");
        MoveToPosition(currentIndex);
    }

    public void Next()
    {
        currentIndex = (currentIndex + 1) % cameraPositions.Length;
        Debug.Log($"[CameraController] Next() 호출 → currentIndex = {currentIndex}");
        MoveToPosition(currentIndex);
    }

    public void Prev()
    {
        currentIndex = (currentIndex - 1 + cameraPositions.Length) % cameraPositions.Length;
        Debug.Log($"[CameraController] Prev() 호출 → currentIndex = {currentIndex}");
        MoveToPosition(currentIndex);
    }

    void MoveToPosition(int index)
    {
        if (cameraPositions[index] == null)
        {
            Debug.LogError($"[CameraController] cameraPositions[{index}]가 null입니다.");
            return;
        }

        Debug.Log($"[CameraController] MoveToPosition() → index = {index}, pos = {cameraPositions[index].position}");
        StopAllCoroutines();
        StartCoroutine(LerpToPosition(cameraPositions[index]));
    }

    IEnumerator LerpToPosition(Transform target)
    {
        Debug.Log("[CameraController] LerpToPosition() 시작");

        while (Vector3.Distance(camTransform.position, target.position) > 0.05f)
        {
            camTransform.position = Vector3.Lerp(camTransform.position, target.position, Time.deltaTime * transitionSpeed);
            camTransform.rotation = Quaternion.Slerp(camTransform.rotation, target.rotation, Time.deltaTime * transitionSpeed);
            yield return null;
        }

        camTransform.position = target.position;
        camTransform.rotation = target.rotation;

        Debug.Log("[CameraController] LerpToPosition() 완료");
    }
}