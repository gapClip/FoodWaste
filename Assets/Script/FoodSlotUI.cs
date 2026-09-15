using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 食糧庫の1スロット。食べ物のアイコンと、右下に個数を表示する
public class FoodSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;

    // このスロットの食べ物と、持ち主の食糧庫
    [System.NonSerialized] public InventoryItem item;
    [System.NonSerialized] public Inventory inventory;

    public void Setup(InventoryItem inventoryItem, Inventory owner)
    {
        item = inventoryItem;
        inventory = owner;

        icon.sprite = item.food.foodSprite;

        Refresh();
    }

    // 個数の表示を更新する。最後の1個を持ち上げている間（0個）はアイコンを薄くする
    public void Refresh()
    {
        countText.text = item.count.ToString();
        icon.color = item.count > 0 ? Color.white : new Color(1f, 1f, 1f, 0.4f);
    }
}
