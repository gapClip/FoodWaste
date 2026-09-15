using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 食糧庫のスロットから食べ物を1個持ち上げて、動物にドロップすると与える
// ・持ち上げたとき：スロットの個数が1つ減り、アイコンがカーソルについてくる
// ・動物に与えられたとき：個数は減ったまま（0になったらスロットを消す）
// ・動物以外に落とした／満腹で与えられなかったとき：個数を1つ戻す
public class FoodDrag : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private FoodSlotUI slotUI;
    private Canvas canvas;

    // 持ち上げている1個（カーソルについてくるアイコン）
    private RectTransform dragIcon;

    void Awake()
    {
        slotUI = GetComponent<FoodSlotUI>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 左ボタンのドラッグだけ受け付ける
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (slotUI == null || slotUI.item == null || slotUI.item.count <= 0)
        {
            return;
        }

        // 1個持ち上げる
        slotUI.item.count--;
        slotUI.Refresh();

        dragIcon = CreateDragIcon();
        MoveDragIcon(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            MoveDragIcon(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
        {
            return;
        }

        Destroy(dragIcon.gameObject);
        dragIcon = null;

        if (TryFeed(eventData.position))
        {
            // 与えた（大嫌いで残された場合も含む）。最後の1個ならスロットを消す
            if (slotUI.item.count <= 0)
            {
                slotUI.inventory.RemoveItem(slotUI.item);
            }

            return;
        }

        // 与えられなかったので食糧庫に戻す
        slotUI.item.count++;
        slotUI.Refresh();
    }

    // ドロップした場所にいる動物に1個与える。与えられたら true
    private bool TryFeed(Vector2 screenPosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit == null)
        {
            return false;
        }

        Monster monster = hit.GetComponent<Monster>();

        return monster != null && monster.Feed(slotUI.item.food);
    }

    // カーソルについてくるアイコンを作る（クリック判定を邪魔しないように raycastTarget は切る）
    private RectTransform CreateDragIcon()
    {
        GameObject obj = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(canvas.rootCanvas.transform, false);
        obj.transform.SetAsLastSibling();

        Image image = obj.GetComponent<Image>();
        image.sprite = slotUI.icon.sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = slotUI.icon.rectTransform.rect.size;

        return rect;
    }

    private void MoveDragIcon(PointerEventData eventData)
    {
        RectTransform canvasRect = (RectTransform)canvas.rootCanvas.transform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, canvas.rootCanvas.worldCamera, out Vector2 localPoint);

        dragIcon.localPosition = localPoint;
    }
}
