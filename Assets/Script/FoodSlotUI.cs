using UnityEngine;
using UnityEngine.UI;

// 食糧庫の1スロット。食べ物のアイコンだけを表示する
public class FoodSlotUI : MonoBehaviour
{
    public Image icon;

    // このスロットの食べ物（1個）と、持ち主の食糧庫
    [System.NonSerialized] public InventoryItem item;
    [System.NonSerialized] public Inventory inventory;

    public void Setup(InventoryItem inventoryItem, Inventory owner)
    {
        item = inventoryItem;
        inventory = owner;

        icon.sprite = item.food.foodSprite;
    }
}
