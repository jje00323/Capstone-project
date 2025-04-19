using UnityEngine;
using UnityEngine.UI;

public class DragIconUI : MonoBehaviour
{
    public static DragIconUI Instance { get; private set; }

    [SerializeField] private Image iconImage;
    private RectTransform rectTransform;
    private Canvas canvas;

    private bool isDragging = false;

    private void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        iconImage.enabled = false; // 이미지 비활성화만으로 숨김 처리
    }

    private void Update()
    {
        if (!isDragging) return;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            null, // Overlay는 반드시 null
            out mousePos
        );
        rectTransform.anchoredPosition = mousePos;

        // 디버그 확인
    }

    public void Show(Sprite icon)
    {
        if (icon == null) return;
        isDragging = true;
        iconImage.sprite = icon;
        iconImage.enabled = true;
        Debug.Log("[DragIconUI] Show() 호출됨");
    }

    public void Hide()
    {
        isDragging = false;
        iconImage.sprite = null;
        iconImage.enabled = false;
        Debug.Log("[DragIconUI] Hide() 호출됨");
    }
}