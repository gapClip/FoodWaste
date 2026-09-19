using UnityEngine;

// 動物1体のターンをまたぐ状態。GameState が保持する
public class MonsterState
{
    public readonly MonsterData data;

    // 累計満足度ポイント（小数で内部保持し、表示時に丸める）
    public float satisfaction;

    // 累計成長ポイント（ターンをまたいで加算され、リセットされない）
    public float growthPoints;

    // 今ターンに食べた量
    public int eatenThisTurn;

    // このターンが始まったときの成長段階（GameState.AdvanceTurn で更新する）
    public int levelAtTurnStart = 1;

    public MonsterState(MonsterData data)
    {
        this.data = data;
    }

    // 基礎容量（Lv1 の容量。SPEC 4.1）。エンディングなど、読む側が amountEat の意味を知らなくて済むように
    public int BaseCapacity => data.amountEat;

    // 累計成長ポイントから決まる、いまの成長段階
    public int Level => GameBalance.Instance.GetLevel(data.amountEat, growthPoints);

    // いまの成長段階でのサイズ倍率（容量・お肉の量の倍率。1.00〜2.00。SPEC 4.1）
    public float SizeMultiplier => GameBalance.Instance.GetSizeMultiplier(Level);

    // いまの成長段階での容量（次のターンの容量）
    public int Capacity => GameBalance.Instance.GetCapacity(data.amountEat, Level);

    // このターンの容量。ターン中に成長しても変わらない（SPEC 4.1）
    public int CapacityAtTurnStart => GameBalance.Instance.GetCapacity(data.amountEat, levelAtTurnStart);

    // このターンの見た目の大きさ。シーンに置いた大きさ（Lv5）を 1 とした縦・横の倍率
    public float PlacedScaleAtTurnStart => GameBalance.Instance.GetPlacedScale(levelAtTurnStart);

    // いまの成長段階での見た目の大きさ。シーンに置いた大きさ（Lv5）を 1 とした縦・横の倍率。
    // 最後のターンの AdvanceTurn() のあとに読めば、最終成長段階の大きさになる（SPEC 3.6.2）
    public float PlacedScale => GameBalance.Instance.GetPlacedScale(Level);

    // このターンに成長段階が上がったか（リザルトの「おおきくなった！」用）
    public bool LeveledUp => Level > levelAtTurnStart;

    // 満足度%（0〜100、切り捨て）
    public int SatisfactionPercent
    {
        get
        {
            int target = GameBalance.Instance.GetSatisfactionTarget(data.amountEat);

            return Mathf.Clamp(Mathf.FloorToInt(satisfaction / target * 100f), 0, 100);
        }
    }

    // 味ランク（★の数）
    public int TasteRank => GameBalance.Instance.GetTasteRank(SatisfactionPercent);

    // 今ターンの満腹度（0.0〜1.0）
    public float FullnessRate
    {
        get
        {
            int capacity = CapacityAtTurnStart;

            return capacity > 0 ? Mathf.Clamp01((float)eatenThisTurn / capacity) : 0f;
        }
    }
}
