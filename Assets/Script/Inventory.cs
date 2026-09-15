using System.Collections.Generic;
using UnityEngine;

// 食糧庫。食べ物1個につき1スロットを作る
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
            if (!slotUIs.ContainsKey(item))
            {
                CreateSlot(item);
            }
        }
    }

    // 食べ物を1個追加する（同じ食べ物でもまとめず、1個ずつスロットを作る）
    public void AddFood(FoodData food)
    {
        InventoryItem item = new InventoryItem();
        item.food = food;

        items.Add(item);
        CreateSlot(item);
    }

    // 食べ物を1個取り除く（動物に与えたとき）
    public void RemoveItem(InventoryItem item)
    {
        items.Remove(item);

        if (slotUIs.TryGetValue(item, out FoodSlotUI slot))
        {
            slotUIs.Remove(item);
            Destroy(slot.gameObject);
        }
    }

    private void CreateSlot(InventoryItem item)
    {
        GameObject obj = Instantiate(foodSlotPrefab, content);

        FoodSlotUI slot = obj.GetComponent<FoodSlotUI>();

        slot.Setup(item, this);

        slotUIs.Add(item, slot);
    }
}
