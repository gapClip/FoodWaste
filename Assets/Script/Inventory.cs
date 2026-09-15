using System.Collections.Generic;
using UnityEngine;

// 食糧庫。食べ物1種類につき1スロットを作り、個数をまとめて持つ
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

    // 食べ物を1個追加する。同じ食べ物のスロットがあれば個数を増やす
    public void AddFood(FoodData food)
    {
        InventoryItem item = items.Find(x => x.food == food);

        if (item != null)
        {
            item.count++;
            slotUIs[item].Refresh();
            return;
        }

        item = new InventoryItem();
        item.food = food;
        item.count = 1;

        items.Add(item);
        CreateSlot(item);
    }

    // スロットを取り除く（個数が0になったとき）
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
