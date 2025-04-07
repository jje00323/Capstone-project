using UnityEngine;
using UnityEngine.UI;

public class DragIconUI : MonoBehaviour
{
    public static DragIconUI Instance { get; private set; }

    [SerializeField] private Image iconImage;
    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        Hide();
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        // 마우스 위치에 드래그 아이콘 따라가게
        Vector3 mousePos = Input.mousePosition;
        rectTransform.position = mousePos + new Vector3(32f, -32f, 0f);  // 아이콘이 마우스에서 살짝 오른쪽 아래
    }

    public void Show(Sprite icon)
    {
        Debug.Log("[DragIconUI] Show 호출됨");
        iconImage.sprite = icon;
        iconImage.enabled = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
        gameObject.SetActive(false);
    }
}