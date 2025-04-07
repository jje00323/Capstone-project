using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float duration = 2f;

    private float timer = 0f;
    private int accumulatedDamage = 0;

    public System.Action onDestroyed; // 콜백 등록용

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void SetDamage(int amount, Color color)
    {
        accumulatedDamage = amount;
        textMesh.text = accumulatedDamage.ToString();
        textMesh.color = color;
        timer = 0f;
    }

    public void AddDamage(int amount)
    {
        accumulatedDamage += amount;
        textMesh.text = accumulatedDamage.ToString();
        timer = 0f; // 유지 시간 리셋
    }
    public void SetText(string message, Color color)
    {
        textMesh.text = message;
        textMesh.color = color;
        timer = 0f; // 유지 시간 리셋
    }


    void Update()
    {
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            onDestroyed?.Invoke(); // 사라지기 직전에 알림
            Destroy(gameObject);
        }
    }
}
