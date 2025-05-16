using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Options")]
    [Range(0, 10)]
    [SerializeField] private int _horizontalSlotCount = 8;
    [Range(0, 10)]
    [SerializeField] private int _verticalSlotCount = 6;
    [SerializeField] private float _slotMargin = 8f;
    [SerializeField] private float _contentAreaPadding = 20f;
    [Range(32, 100)]
    [SerializeField] private float _slotSize = 80f;

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT;
    [SerializeField] private GameObject _slotUiPrefab;

    private List<InventorySlotUI> _slotUIList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
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
                slotRT.pivot = new Vector2(0f, 1f);
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
            RefreshQuickSlots();
        }
    }

    public void RefreshQuickSlots()
    {
        var quickSlots = FindObjectsOfType<QuickSlotUI>();
        foreach (var slot in quickSlots)
        {
            slot.RefreshSlotUI();
        }
    }
}