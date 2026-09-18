using System.Collections.Generic;
using UnityEngine;

// ショップの陳列抽選（SPEC 6.2）
public static class ShopLottery
{
    // 前のターンと同じ並びが続いたときに引き直す回数の上限（無限ループ防止）
    private const int MaxRedraw = 100;

    // 今ターンに陳列する食べ物を count 種類返す。同じターンに何度呼んでも同じ並びを返す
    public static List<FoodData> Draw(FoodData[] allFoods, int count)
    {
        if (GameState.HasShopLineupThisTurn && GameState.ShopLineup.Count == count)
        {
            return new List<FoodData>(GameState.ShopLineup);
        }

        if (count != GameBalance.Instance.shopDisplayCount)
        {
            Debug.LogWarning($"ショップの枠の数（{count}）が GameBalance.shopDisplayCount（{GameBalance.Instance.shopDisplayCount}）と違います");
        }

        List<FoodData> lineup = Draw(allFoods, FindMonsters(), GameState.ShopLineup, count);

        GameState.SetShopLineup(lineup);

        return lineup;
    }

    // 抽選の本体。previous は前のターンの陳列（無ければ空）
    public static List<FoodData> Draw(
        IList<FoodData> allFoods, IList<MonsterData> monsters, IReadOnlyList<FoodData> previous, int count)
    {
        List<FoodData> lineup = DrawOnce(allFoods, monsters, count);

        // ③ 前のターンとまったく同じ種類になったら引き直す
        for (int i = 0; i < MaxRedraw && IsSameSet(lineup, previous); i++)
        {
            lineup = DrawOnce(allFoods, monsters, count);
        }

        return lineup;
    }

    private static List<FoodData> DrawOnce(IList<FoodData> allFoods, IList<MonsterData> monsters, int count)
    {
        List<FoodData> pool = new List<FoodData>(allFoods);

        count = Mathf.Min(count, pool.Count);

        // ① 重複なしで count 種類を引く（先頭 count 個だけシャッフルする）
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(i, pool.Count);

            FoodData temp = pool[i];
            pool[i] = pool[randomIndex];
            pool[randomIndex] = temp;
        }

        List<FoodData> lineup = pool.GetRange(0, count);

        // ② どの動物の大好き（◎）も並ばなかったら、1枠をまだ選ばれていない ◎ の食べ物に差し替える
        if (count > 0 && !lineup.Exists(food => IsLovedByAny(food, monsters)))
        {
            List<FoodData> loved = pool.GetRange(count, pool.Count - count)
                .FindAll(food => IsLovedByAny(food, monsters));

            if (loved.Count > 0)
            {
                lineup[Random.Range(0, lineup.Count)] = loved[Random.Range(0, loved.Count)];
            }
        }

        return lineup;
    }

    // いずれかの動物の大好き（◎）か。好みは MonsterData の好み表で決まる
    private static bool IsLovedByAny(FoodData food, IList<MonsterData> monsters)
    {
        if (food == null)
        {
            return false;
        }

        foreach (MonsterData monster in monsters)
        {
            if (monster != null &&
                monster.GetPreference(food.category, food.texture) == FoodPreference.大好き)
            {
                return true;
            }
        }

        return false;
    }

    // 並び順は無視して、同じ種類の組み合わせか
    private static bool IsSameSet(List<FoodData> lineup, IReadOnlyList<FoodData> previous)
    {
        if (previous == null || previous.Count == 0 || previous.Count != lineup.Count)
        {
            return false;
        }

        foreach (FoodData food in previous)
        {
            if (!lineup.Contains(food))
            {
                return false;
            }
        }

        return true;
    }

    // シーンにいる動物の MonsterData を集める（Start の実行順に左右されないよう、シーンから直接探す）
    private static List<MonsterData> FindMonsters()
    {
        List<MonsterData> monsters = new List<MonsterData>();

        foreach (MonsterStatus status in Object.FindObjectsByType<MonsterStatus>(FindObjectsSortMode.None))
        {
            if (status.monsterData != null && !monsters.Contains(status.monsterData))
            {
                monsters.Add(status.monsterData);
            }
        }

        if (monsters.Count == 0)
        {
            Debug.LogWarning("シーンに動物がいないので、大好きな食べ物の保証（SPEC 6.2 ②）を行いません");
        }

        return monsters;
    }
}
