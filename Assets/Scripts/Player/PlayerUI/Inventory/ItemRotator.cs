//using UnityEngine;

//public class ItemRotator : MonoBehaviour
//{
//    [Header("회전 설정")]
//    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

//    [Header("부유 설정")]
//    [SerializeField] private float floatAmplitude = 0.25f; // 위아래 이동 폭
//    [SerializeField] private float floatFrequency = 1f;    // 초당 진동 횟수

//    private Vector3 startPos;

//    void Start()
//    {
//        startPos = transform.position;
//    }

//    void Update()
//    {
//        // 회전
//        transform.Rotate(rotationSpeed * Time.deltaTime);

//        // 부유 (위아래 움직임)
//        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
//        transform.position = startPos + new Vector3(0f, newY, 0f);
//    }
//}