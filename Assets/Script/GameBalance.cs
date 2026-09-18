using UnityEngine;

// ゲーム全体の数値バランス（SPEC 付録A）
// 数値の調整は Assets/Resources/GameBalance.asset の値だけを変更する
[CreateAssetMenu(fileName = "GameBalance", menuName = "Game/GameBalance")]
public class GameBalance : ScriptableObject
{
    private static GameBalance instance;

    // Resources/GameBalance.asset を読み込んで返す
    public static GameBalance Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameBalance>("GameBalance");

                if (instance == null)
                {
                    Debug.LogError("Resources/GameBalance.asset が見つかりません");
                }
            }

            return instance;
        }
    }

    [Header("ターン")]
    public int turnCount = 10;

    [Header("満腹度 = ベース + カテゴリ補正 + 食感補正")]
    public int fullnessBase = 4;
    public int meatBonus = 1;
    public int fruitBonus = -1;
    public int fishBonus = 0;
    public int dorodoroBonus = 0;
    public int pasapasaBonus = 1;
    public int gotsugotsuBonus = 2;

    [Header("成長（Lv1〜Lv5）")]
    [Tooltip("容量のサイズ倍率（お肉の量にも使う）。要素0がLv1")]
    public float[] sizeMultipliers = { 1.00f, 1.25f, 1.50f, 1.75f, 2.00f };
    [Tooltip("見た目の縦・横それぞれの倍率。面積がサイズ倍率とほぼ同じになるよう √サイズ倍率 を小数第1位に丸めた値。要素0がLv1")]
    public float[] visualScaleMultipliers = { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f };
    [Tooltip("そのLvに上がる累計成長ポイント（× 基礎容量）。要素0がLv2")]
    public float[] growthThresholdMultipliers = { 2.5f, 5.5f, 9.0f, 13.0f };

    [Header("成長係数（実食量にかける）")]
    public float loveGrowth = 1.5f;
    public float likeGrowth = 1.2f;
    public float normalGrowth = 1.0f;

    [Header("満足係数（実食量にかける。大嫌いのみ満腹度にかける）")]
    public float loveSatisfaction = 3f;
    public float likeSatisfaction = 1f;
    public float normalSatisfaction = 0f;
    public float hateSatisfaction = -2f;

    [Header("満足度")]
    [Tooltip("給餌でこぼれたゴミ1につき")]
    public float spillPenalty = -1f;
    [Tooltip("ターンの総ゴミ1につき、全動物に")]
    public float smokePenalty = -0.5f;
    [Tooltip("満足度の目標値 = 基礎容量 × この値")]
    public int satisfactionTargetMultiplier = 16;
    [Tooltip("味ランクの閾値（満足度%）。★5, ★4, ★3, ★2 の順")]
    public int[] tasteRankThresholds = { 90, 70, 50, 25 };

    [Header("ショップ")]
    public int shopDisplayCount = 4;
    public int maxBuyPerFood = 4;
    public int maxBuyPerTurn = 12;
    public int pantrySlots = 12;

    [Header("CO₂")]
    [Tooltip("段階1〜5に上がる累計CO₂")]
    public int[] co2StageThresholds = { 10, 25, 45, 70, 100 };
    [Tooltip("汚れ度 t = 累計CO₂ ÷ この値")]
    public int pollutionDenominator = 100;
    [Tooltip("段階が上がったときの暗転時間（秒）")]
    public float stageUpBlackoutSeconds = 0.5f;

    [Header("焚き火（そのターンのゴミ量）")]
    [Tooltip("ティア1〜4に上がるゴミ量。ゴミ0はティア0")]
    public int[] bonfireTierThresholds = { 1, 5, 10, 20 };
    [Tooltip("ティア0〜4の演出の継続時間（秒）")]
    public float[] bonfireDurations = { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f };
    public int smokeParticleBase = 20;
    public int smokeParticlePerTrash = 6;
    public int smokeParticleMax = 200;
    [Tooltip("煙の高さ = 画面高 × clamp(基本 + ゴミ量 × 係数, 最小, 最大)")]
    public float smokeHeightBase = 0.2f;
    public float smokeHeightPerTrash = 0.04f;
    public float smokeHeightMin = 0.2f;
    public float smokeHeightMax = 1.0f;

    [Header("ゴミ処理画面のゴミ袋")]
    [Tooltip("ゴミ袋1個あたりのゴミ量。袋の数 = そのターンのゴミ ÷ この値 の切り上げ（ゴミ0なら0個）")]
    public int trashPerBag = 5;

    public int MaxLevel => sizeMultipliers.Length;

    // 満腹度（SPEC 5.1）
    public int GetFullness(FoodCategory category, FoodTexture texture)
    {
        int categoryBonus = category switch
        {
            FoodCategory.肉 => meatBonus,
            FoodCategory.果実 => fruitBonus,
            _ => fishBonus,
        };

        int textureBonus = texture switch
        {
            FoodTexture.ドロドロ => dorodoroBonus,
            FoodTexture.パサパサ => pasapasaBonus,
            _ => gotsugotsuBonus,
        };

        return fullnessBase + categoryBonus + textureBonus;
    }

    public float GetSizeMultiplier(int level)
    {
        return sizeMultipliers[Mathf.Clamp(level, 1, MaxLevel) - 1];
    }

    // 見た目の縦・横の倍率（Lv1 を 1 とする）
    public float GetVisualScale(int level)
    {
        return visualScaleMultipliers[Mathf.Clamp(level, 1, visualScaleMultipliers.Length) - 1];
    }

    // シーンに置いた大きさ（最大の Lv）を 1 としたときの、見た目の縦・横の倍率
    public float GetPlacedScale(int level)
    {
        return GetVisualScale(level) / visualScaleMultipliers[visualScaleMultipliers.Length - 1];
    }

    // 容量 = 基礎容量 × サイズ倍率
    public int GetCapacity(int baseCapacity, int level)
    {
        return Mathf.RoundToInt(baseCapacity * GetSizeMultiplier(level));
    }

    // level（2〜MaxLevel）に上がるのに必要な累計成長ポイント
    public float GetGrowthThreshold(int baseCapacity, int level)
    {
        return baseCapacity * growthThresholdMultipliers[level - 2];
    }

    // 累計成長ポイントから現在のLvを求める（1ターンで複数段階上がってもよい）
    public int GetLevel(int baseCapacity, float growthPoints)
    {
        int level = 1;

        while (level < MaxLevel &&
               growthPoints >= GetGrowthThreshold(baseCapacity, level + 1))
        {
            level++;
        }

        return level;
    }

    public int GetSatisfactionTarget(int baseCapacity)
    {
        return baseCapacity * satisfactionTargetMultiplier;
    }

    public float GetGrowthCoefficient(FoodPreference preference)
    {
        return preference switch
        {
            FoodPreference.大好き => loveGrowth,
            FoodPreference.好き => likeGrowth,
            FoodPreference.普通 => normalGrowth,
            _ => 0f,
        };
    }

    public float GetSatisfactionCoefficient(FoodPreference preference)
    {
        return preference switch
        {
            FoodPreference.大好き => loveSatisfaction,
            FoodPreference.好き => likeSatisfaction,
            FoodPreference.普通 => normalSatisfaction,
            _ => hateSatisfaction,
        };
    }

    // 1個与えたときの結果（SPEC 8.1）
    // remainingCapacity：その動物の残り容量（0 のときは与えられないので呼ばない）
    public FeedResult CalculateFeed(FoodPreference preference, int fullness, int remainingCapacity)
    {
        FeedResult result = new FeedResult();
        result.preference = preference;

        // 大嫌い：食べずにそのまま残す（満腹度がまるごとゴミ）
        if (preference == FoodPreference.大嫌い)
        {
            result.eaten = 0;
            result.trash = fullness;
            result.satisfaction = RoundToTenth(hateSatisfaction * fullness);
            result.growth = 0f;
            return result;
        }

        // 食べている途中で満腹になった分はゴミになる
        result.eaten = Mathf.Min(fullness, Mathf.Max(remainingCapacity, 0));
        result.trash = fullness - result.eaten;
        result.satisfaction = RoundToTenth(
            GetSatisfactionCoefficient(preference) * result.eaten + spillPenalty * result.trash);
        result.growth = RoundToTenth(GetGrowthCoefficient(preference) * result.eaten);

        return result;
    }

    // 満足度・成長ポイントは小数第1位まで保持する（SPEC 8.4）
    public static float RoundToTenth(float value)
    {
        return Mathf.Round(value * 10f) / 10f;
    }

    // 味ランク（★の数 1〜5）
    public int GetTasteRank(int satisfactionPercent)
    {
        for (int i = 0; i < tasteRankThresholds.Length; i++)
        {
            if (satisfactionPercent >= tasteRankThresholds[i])
            {
                return tasteRankThresholds.Length + 1 - i;
            }
        }

        return 1;
    }

    // CO₂段階（0〜5）
    public int GetCo2Stage(int totalCo2)
    {
        return CountReached(co2StageThresholds, totalCo2);
    }

    // 汚れ度 t（0.0〜1.0）
    public float GetPollution(int totalCo2)
    {
        return Mathf.Clamp01((float)totalCo2 / pollutionDenominator);
    }

    // 焚き火ティア（0〜4）
    public int GetBonfireTier(int turnTrash)
    {
        return CountReached(bonfireTierThresholds, turnTrash);
    }

    // 焚き火の継続時間（秒）。SPEC 9.3
    // Inspector で配列の長さを変えられても例外にしない（足りないティアは最後の値を使う）
    // 配列が空なら 0 を返す。呼ぶ側は割り算に使う前に 0 かどうかを確かめること
    public float GetBonfireDuration(int turnTrash)
    {
        if (bonfireDurations == null || bonfireDurations.Length == 0)
        {
            return 0f;
        }

        int tier = GetBonfireTier(turnTrash);

        return bonfireDurations[Mathf.Clamp(tier, 0, bonfireDurations.Length - 1)];
    }

    public int GetSmokeParticleCount(int turnTrash)
    {
        return Mathf.Min(smokeParticleBase + turnTrash * smokeParticlePerTrash, smokeParticleMax);
    }

    // 煙の高さ（画面高に対する割合）
    public float GetSmokeHeightRate(int turnTrash)
    {
        return Mathf.Clamp(smokeHeightBase + turnTrash * smokeHeightPerTrash, smokeHeightMin, smokeHeightMax);
    }

    // ゴミ処理画面に出すゴミ袋の数。切り上げなので、ゴミが1でもあれば1個出る（ゴミ0なら0個）
    public int GetTrashBagCount(int turnTrash)
    {
        if (turnTrash <= 0)
        {
            return 0;
        }

        int perBag = Mathf.Max(trashPerBag, 1);

        return (turnTrash + perBag - 1) / perBag;
    }

    // 昇順の閾値のうち、value が到達している個数
    private static int CountReached(int[] thresholds, int value)
    {
        int count = 0;

        foreach (int threshold in thresholds)
        {
            if (value >= threshold)
            {
                count++;
            }
        }

        return count;
    }
}

// 食べ物を1個与えたときの計算結果
public struct FeedResult
{
    public FoodPreference preference;

    // 実際に食べた量
    public int eaten;

    // ゴミになった量
    public int trash;

    // 満足度の増減
    public float satisfaction;

    // 成長ポイントの増加
    public float growth;
}
