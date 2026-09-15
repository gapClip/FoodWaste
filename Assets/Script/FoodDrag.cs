using UnityEngine;
using UnityEngine.EventSystems;

// 食糧庫のスロットをドラッグして、動物にドロップすると1個与える
[RequireComponent(typeof(CanvasGroup))]
public class FoodDrag : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
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

        if (hit != null && slotUI != null && slotUI.item != null)
        {
            Monster monster = hit.GetComponent<Monster>();

            // 与えられたら（大嫌いで残された場合も含む）食糧庫から1個減らす
            if (monster != null && monster.Feed(slotUI.item.food))
            {
                slotUI.inventory.RemoveItem(slotUI.item);
                return;
            }
        }

        // 与えられなかったときは元の位置に戻す
        rectTransform.anchoredPosition = startPos;
    }
}
