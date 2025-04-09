using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    
    [Header("Options")]
    [Range(0, 10)]
    [SerializeField] private int _horizontalSlotCount = 8;  // 슬롯 가로 개수
    [Range(0, 10)]
    [SerializeField] private int _verticalSlotCount = 6;    // 슬롯 세로 개수
    [SerializeField] private float _slotMargin = 8f;        // 슬롯 간 간격
    [SerializeField] private float _contentAreaPadding = 20f; // 슬롯 영역 내부 여백
    [Range(32, 100)]
    [SerializeField] private float _slotSize = 80f;         // 슬롯 크기

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT;  // 슬롯 배치될 영역
    [SerializeField] private GameObject _slotUiPrefab;      // 슬롯 프리팹

    private List<InventorySlotUI> _slotUIList;


    public static InventoryUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("InventoryUI: 중복 인스턴스 발견됨. 기존 인스턴스를 유지합니다.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitSlots();
        RefreshAllSlots();
    }

    

    private void InitSlots()
    {
        if (_slotUIList != null && _slotUIList.Count > 0)
        {
            foreach (var slot in _slotUIList)
            {
                if (slot != null)
                    Destroy(slot.gameObject);
            }
            _slotUIList.Clear();
        }

        // 슬롯 프리팹 설정
        _slotUiPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(_slotSize, _slotSize);

        _slotUiPrefab.TryGetComponent(out InventorySlotUI itemSlot);
        if (itemSlot == null)
            _slotUiPrefab.AddComponent<InventorySlotUI>();

        _slotUiPrefab.SetActive(false);

        Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
        Vector2 curPos = beginPos;

        _slotUIList = new List<InventorySlotUI>(_verticalSlotCount * _horizontalSlotCount);

        for (int j = 0; j < _verticalSlotCount; j++)
        {
            for (int i = 0; i < _horizontalSlotCount; i++)
            {
                int slotIndex = (_horizontalSlotCount * j) + i;

                var slotRT = CloneSlot();
                slotRT.pivot = new Vector2(0f, 1f); // 왼쪽 상단 기준
                slotRT.anchorMin = new Vector2(0f, 1f);
                slotRT.anchorMax = new Vector2(0f, 1f);
                slotRT.anchoredPosition = curPos;
                slotRT.localScale = Vector3.one;
                slotRT.gameObject.SetActive(true);
                slotRT.gameObject.name = $"Item Slot [{slotIndex}]";

                var slotUI = slotRT.GetComponent<InventorySlotUI>();
                slotUI.SetSlotIndex(slotIndex);
                _slotUIList.Add(slotUI);

                curPos.x += (_slotMargin + _slotSize);
            }

            curPos.x = beginPos.x;
            curPos.y -= (_slotMargin + _slotSize);
        }

        if (_slotUiPrefab.scene.rootCount != 0)
            Destroy(_slotUiPrefab);

        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(_slotUiPrefab);
            RectTransform rt = slotGo.GetComponent<RectTransform>();
            rt.SetParent(_contentAreaRT, false);
            return rt;
        }
    }

    public void RefreshAllSlots()
    {
        var slots = InventoryManager.Instance.slots;

        for (int i = 0; i < _slotUIList.Count; i++)
        {
            if (i < slots.Count)
                _slotUIList[i].SetSlot(slots[i]);
            else
                _slotUIList[i].ClearSlot();
        }
    }

    public void AddItemAndRefresh(ItemData item, int amount = 1)
    {
        bool success = InventoryManager.Instance.AddItem(item, amount);
        if (success)
        {
            RefreshAllSlots();
            RefreshQuickSlots();  //  퀵슬롯도 같이 갱신
        }
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }

    public void RefreshQuickSlots()
    {
        var quickSlots = FindObjectsOfType<QuickSlotUI>();
        foreach (var slot in quickSlots)
        {
            slot.RefreshSlotUI();
        }
    }

    // 기존의 Close 버튼용 함수도 같이 유지 가능

}
