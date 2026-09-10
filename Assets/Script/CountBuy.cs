using UnityEngine;
using TMPro;

public class CountBuy : MonoBehaviour
{
    // 現在購入する個数
    [SerializeField] private int buyCount = 0;
    [SerializeField] private TMP_Text countText;
    // ＋ボタン
    public void AddCount()
    {
        buyCount++;
        countText.text = buyCount.ToString();
    }

    // －ボタン
    public void SubtractCount()
    {
        // 0未満にならないようにする
        if (buyCount > 0)
        {
            buyCount--;
            countText.text = buyCount.ToString();
        }
    }
     // Buyから購入数を取得する
    public int GetBuyCount()
    {
        return buyCount;
    }

    // 購入後に0へ戻す
    public void ResetCount()
    {
        buyCount = 0;
        countText.text = buyCount.ToString();
    }
}