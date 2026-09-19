using UnityEngine;

// メイン画面（GiveFood）の常時表示 HUD に出す値（SPEC 12「必須の常時表示情報」）
// ・食糧庫の残り個数（◯/12）… PantryCapacity / GetPantryCount
// ・選択中の動物の残り容量メーター … GetCapacity / GetRemainingCapacity（動物ごとの値。
//   どの動物が選択中かは呼ぶ側が決め、その動物の MonsterStatus を渡す）
// ここは値を返すだけ。テキストやメーターへの表示は、シーン・UI の担当が行う
// 毎フレーム呼ばれても例外を出さず、値も保存しない（呼ぶたびに今の状態から求める）
public static class HudValues
{
    // 食糧庫の容量（HUD の「◯/12」の 12。SPEC 6.1）
    public static int PantryCapacity => GameBalance.Instance.pantrySlots;

    // 食糧庫に今入っている食べ物の個数（HUD の「◯/12」の ◯）
    // 食糧庫は食べ物1種類＝1スロットなので、スロットの数ではなく各スロットの個数を合計する
    // ドラッグで持ち上げている1個は、持ち上げた時点でスロットの個数から引かれているので数えない（スロットの表示と同じ）
    // inventory や items が無いときは 0。items の中の null の要素と負の個数は、ふつう起きないが数えない
    public static int GetPantryCount(Inventory inventory)
    {
        if (inventory == null || inventory.items == null)
        {
            return 0;
        }

        int total = 0;

        foreach (InventoryItem item in inventory.items)
        {
            if (item != null && item.count > 0)
            {
                total += item.count;
            }
        }

        return total;
    }

    // 動物の今ターンの容量（残り容量メーターの分母）
    // ターン開始時の成長段階で決まり、ターン中は変わらない（SPEC 4.1）。成長しても次のターンから
    // MonsterStatus.Start() がターン開始時に maxAmountEat を入れる。それより前（シーンを読み込んだ最初のフレームの、
    // ほかのスクリプトの Awake / Start）に読むと、シーンに保存された値が返ることがある。Update からは正しい値
    // status が無いとき（破棄済みを含む）と、負の値（ふつう起きない）は 0
    public static int GetCapacity(MonsterStatus status)
    {
        if (status == null)
        {
            return 0;
        }

        return Mathf.Max(0, status.maxAmountEat);
    }

    // 動物の今ターンの残り容量（残り容量メーターの残り。SPEC 8.1 の R）
    // Monster.Feed が「与えられるか」の判定に使っている式と同じ（容量 − 今ターンに食べた量）なので、表示と判定が食い違わない。
    // 0 のときは与えられない（1 でも与えられる）
    // MonsterState からは求めない（eatenThisTurn は GoResult でリザルトへ移るときにしか写されず、餌やり中は 0 のまま）
    // MonsterStatus.Start() より前に読むと、GetCapacity と同じくシーンに保存された値から求めた値が返ることがある
    // status が無いとき（破棄済みを含む）と、負になるとき（食べた量が容量を超えている。ふつう起きない）は 0
    public static int GetRemainingCapacity(MonsterStatus status)
    {
        if (status == null)
        {
            return 0;
        }

        return Mathf.Max(0, status.maxAmountEat - status.currentAmountEat);
    }
}
