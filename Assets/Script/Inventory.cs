using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Transform content;
    public GameObject foodSlotPrefab;

    public List<InventoryItem> items = new();

    private Dictionary<InventoryItem, FoodSlotUI> slotUIs = new();


    void Start()
    {
        CreateUI();
    }

    void CreateUI()
    {
        foreach (InventoryItem item in items)
        {
            GameObject obj = Instantiate(foodSlotPrefab, content);

            obj.GetComponent<FoodSlotUI>().Setup(item);
        }
    }
    public void AddFood(FoodData food)
    {
        InventoryItem item = items.Find(x => x.food == food);

        if (item != null)
        {
            item.ownedCount++;
            slotUIs[item].Refresh();
        }
        else
        {
            item = new InventoryItem();
            item.food = food;
            item.ownedCount = 1;
            item.selectedCount = 0;

            items.Add(item);

            GameObject obj = Instantiate(foodSlotPrefab, content);

            FoodSlotUI slot = obj.GetComponent<FoodSlotUI>();

            slot.Setup(item);

            slotUIs.Add(item, slot);
        }
    }
}