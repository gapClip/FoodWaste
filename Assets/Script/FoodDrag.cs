using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class FoodDrag : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public InventoryItem item;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 startPos;
    private FoodSlotUI slotUI;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        slotUI = GetComponent<FoodSlotUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Vector2 worldPos =
            Camera.main.ScreenToWorldPoint(eventData.position);

        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null)
        {
            Monster monster = hit.GetComponent<Monster>();

            if (monster != null)
            {
                monster.Feed(item);
                slotUI.Refresh();
            }
        }

        rectTransform.anchoredPosition = startPos;
    }
}