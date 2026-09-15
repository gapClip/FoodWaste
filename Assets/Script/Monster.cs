using UnityEngine;

public class Monster : MonoBehaviour
{
    private MonsterStatus monsterStatus;
    private Animator animator;
    [SerializeField] private GameObject trash;

    [SerializeField] private float trashMinX = -1.5f;
    [SerializeField] private float trashMaxX = 1.5f;
    [SerializeField] private float trashMinY = -0.5f;
    [SerializeField] private float trashMaxY = 0.5f;

    void Awake()
    {
        monsterStatus = GetComponent<MonsterStatus>();
        animator = GetComponent<Animator>();
    }

    // 食べ物を与える。選んだ個数を1個ずつ計算する（SPEC 8.1）
    public void Feed(InventoryItem item)
    {
        if (item.ownedCount <= 0)
        {
            return;
        }

        // ＋−で個数を選んでいなければ1個だけ与える
        int count = Mathf.Clamp(item.selectedCount, 1, item.ownedCount);

        // 好みは MonsterData の好み表（カテゴリ × 食感）で決まる
        FoodPreference preference =
            monsterStatus.monsterData.GetPreference(
                item.food.category, item.food.texture);

        MonsterState state = monsterStatus.State;

        int fedCount = 0;
        int trashAmount = 0;

        for (int i = 0; i < count; i++)
        {
            int remaining = monsterStatus.maxAmountEat - monsterStatus.currentAmountEat;

            // 残り容量が0のときは与えられない（残り1でも与えられる）
            if (remaining <= 0)
            {
                break;
            }

            FeedResult result = GameBalance.Instance.CalculateFeed(
                preference, item.food.amount, remaining);

            monsterStatus.currentAmountEat += result.eaten;
            // 小数の誤差で閾値（例：成長40）を取りこぼさないよう、足したあとも小数第1位で丸める
            state.satisfaction = GameBalance.RoundToTenth(state.satisfaction + result.satisfaction);
            state.growthPoints = GameBalance.RoundToTenth(state.growthPoints + result.growth);
            GameState.AddFeedTrash(result.trash);

            trashAmount += result.trash;
            fedCount++;
        }

        if (fedCount == 0)
        {
            Debug.Log("これ以上食べられません！");
            return;
        }

        PlayFoodReaction(preference);

        // 食べ残し（大嫌いでそのまま残した分も含む）が出たらゴミ袋を出す
        if (trashAmount > 0)
        {
            SpawnTrash();
        }

        // 与えられなかった分は食糧庫に残る
        item.ownedCount -= fedCount;
        item.selectedCount = 0;

        Debug.Log($"{item.food.foodName} を {fedCount} 個与えた（{preference}）");
        Debug.Log($"食べた量: {monsterStatus.currentAmountEat}/{monsterStatus.maxAmountEat}　ゴミ: {trashAmount}");
        Debug.Log($"満足度: {state.satisfaction}　成長ポイント: {state.growthPoints}");
    }

    // 好みに応じた食事アニメーション
    private void PlayFoodReaction(FoodPreference preference)
    {
        switch (preference)
        {
            case FoodPreference.大好き:
                animator.SetTrigger("love");
                break;
            case FoodPreference.好き:
                animator.SetTrigger("like");
                break;
            case FoodPreference.大嫌い:
                animator.SetTrigger("dislike");
                break;
            default:
                animator.SetTrigger("normal");
                break;
        }
    }

    private void SpawnTrash()
    {
        if (trash == null)
        {
            Debug.LogWarning("trashプレハブが設定されていません！");
            return;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(trashMinX, trashMaxX),
            -2,
            0f
        );

        Vector3 spawnPosition = transform.position + randomOffset;

        Instantiate(trash, spawnPosition, Quaternion.identity);
    }
}
