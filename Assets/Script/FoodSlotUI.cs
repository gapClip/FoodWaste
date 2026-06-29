using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text countText;

    public Button plusButton;
    public Button minusButton;

    public InventoryItem item;
    public GameObject inventory;

    public void Setup(InventoryItem inventoryItem)
    {
        item = inventoryItem;

        icon.sprite = item.food.foodSprite;
        GetComponent<FoodDrag>().item = item;

        Refresh();

        plusButton.onClick.AddListener(Add);
        minusButton.onClick.AddListener(Remove);
    }

    void Add()
    {
        if (item.selectedCount < item.ownedCount)
        {
            item.selectedCount++;
            Refresh();
        }
    }

    void Remove()
    {
        if (item.selectedCount > 0)
        {
            item.selectedCount--;
            Refresh();
        }
    }

    public void Refresh()
    {
        countText.text = $"{item.selectedCount}/{item.ownedCount}";
    }
}