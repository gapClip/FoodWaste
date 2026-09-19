using UnityEngine;
using UnityEngine.UI;

public class Buy : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    private GameObject[] foodObjects;
   [SerializeField] private ShopPanelMove shopPanelMove;
        [SerializeField] private Button buyButton;

    // PrefabGridSpawnerから商品一覧を受け取る
    public void SetFoodObjects(GameObject[] objects)
    {
        foodObjects = objects;
    }

public bool CanPurchase()
{
    if (foodObjects == null)
    {
        return false;
    }

    foreach (GameObject food in foodObjects)
    {
        if (food == null)
        {
            continue;
        }

        CountBuy count = food.GetComponent<CountBuy>();

        if (count != null && count.GetBuyCount() > 0)
        {
            return true;
        }
    }

    return false;
}   

 public void Purchase()
    {
        if (foodObjects == null)
        {
            Debug.LogError("商品一覧が設定されていません");
            return;
        }
        if (!CanPurchase())
        return;

        foreach (GameObject foodObject in foodObjects)
        {
            CountBuy countBuy = foodObject.GetComponent<CountBuy>();
            TestAdd testAdd = foodObject.GetComponent<TestAdd>();

            if (countBuy == null || testAdd == null)
            {
                continue;
            }

            int count = countBuy.GetBuyCount();

            // 1個以上購入する場合
            if (count > 0)
            {
                FoodData food = testAdd.GetFood();

                if (food == null)
                {
                    continue;
                }

                for (int i = 0; i < count; i++)
                {
                    inventory.AddFood(food);
                }

                // 今ターンに買った数を記録する（購入上限の判定用。SPEC 6.1）
                ShopRules.RecordPurchase(food, count);

                // 購入後に0へ戻す
               countBuy.ResetCount();
                
            }
        }
        shopPanelMove.PanelClose();
    }
}