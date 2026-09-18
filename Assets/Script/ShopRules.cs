using UnityEngine;

// ショップの購入上限（SPEC 6.1）：1種類あたり maxBuyPerFood 個、1ターンの合計 maxBuyPerTurn 個
// ショップは常に開いていて1ターンに何度でも買えるので、「今ターンに買った数」も合わせて数える
public static class ShopRules
{
    // その食べ物をあと何個選べるか
    // selectedOfThisFood：いま選択中（まだ買っていない）のその食べ物の数
    // selectedTotal：いま選択中の全商品の合計
    public static int GetRemaining(FoodData food, int selectedOfThisFood, int selectedTotal)
    {
        GameBalance balance = GameBalance.Instance;

        int perFood = balance.maxBuyPerFood - GameState.GetBoughtThisTurn(food) - selectedOfThisFood;
        int perTurn = balance.maxBuyPerTurn - GameState.TotalBoughtThisTurn - selectedTotal;

        return Mathf.Max(0, Mathf.Min(perFood, perTurn));
    }

    // ショップの商品スロットで、あと何個選べるか（「あと◯個」の表示にも使える）
    public static int GetRemaining(CountBuy slot)
    {
        TestAdd testAdd = slot.GetComponent<TestAdd>();
        FoodData food = testAdd != null ? testAdd.GetFood() : null;

        return GetRemaining(food, slot.GetBuyCount(), GetSelectedTotal(slot));
    }

    // ショップの商品スロットで、＋をもう1回押せるか
    public static bool CanAddOne(CountBuy slot)
    {
        return GetRemaining(slot) > 0;
    }

    // 買った数を記録する（購入のたびに呼ぶ）
    public static void RecordPurchase(FoodData food, int count)
    {
        GameState.RecordPurchase(food, count);
    }

    // 同じショップに並んでいる商品スロットの、選択中の数の合計
    private static int GetSelectedTotal(CountBuy slot)
    {
        Transform shop = slot.transform.parent;

        if (shop == null)
        {
            return slot.GetBuyCount();
        }

        int total = 0;

        foreach (CountBuy other in shop.GetComponentsInChildren<CountBuy>())
        {
            total += other.GetBuyCount();
        }

        return total;
    }
}
