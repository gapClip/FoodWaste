// メイン画面（GiveFood）の常時表示 HUD に出す値（SPEC 12「必須の常時表示情報」）
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
}
