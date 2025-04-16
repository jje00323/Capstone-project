using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RectTransform background;
    [SerializeField] private Vector2 tooltipOffset = new Vector2(12f, 12f);
    private CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }
    void Start()
    {
        // 툴팁 전체의 Raycast를 막음
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void Update()
    {
        if (canvasGroup.alpha > 0)
        {
            Vector2 mousePos = Input.mousePosition;
            transform.position = (Vector3)(mousePos + tooltipOffset);
        }
    }

    public void Show(string title, string description)
    {
        nameText.text = title;
        descriptionText.text = description;
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }
}
