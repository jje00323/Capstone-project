using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        if (SkillUIManager.Instance != null)
            SkillUIManager.Instance.InitializeSkillDataOnly();  // UI가 꺼져 있어도 연결

        // 다른 시스템도 여기에 추가 가능
        // InventoryManager.Instance.Initialize();
    }
}