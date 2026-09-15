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

    public MonsterState(MonsterData data)
    {
        this.data = data;
    }

    // 累計成長ポイントから決まる成長段階
    // ターン中の容量は MonsterStatus がターン開始時にコピーして固定する
    public int Level => GameBalance.Instance.GetLevel(data.amountEat, growthPoints);

    // 今の成長段階での容量
    public int Capacity => GameBalance.Instance.GetCapacity(data.amountEat, Level);

    // 見た目のサイズ倍率
    public float SizeMultiplier => GameBalance.Instance.GetSizeMultiplier(Level);

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
            int capacity = Capacity;

            return capacity > 0 ? Mathf.Clamp01((float)eatenThisTurn / capacity) : 0f;
        }
    }
}
