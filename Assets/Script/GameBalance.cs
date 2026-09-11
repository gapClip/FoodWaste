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
    [Tooltip("容量・見た目のサイズ倍率。要素0がLv1")]
    public float[] sizeMultipliers = { 1.00f, 1.25f, 1.50f, 1.75f, 2.00f };
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

    public int GetSmokeParticleCount(int turnTrash)
    {
        return Mathf.Min(smokeParticleBase + turnTrash * smokeParticlePerTrash, smokeParticleMax);
    }

    // 煙の高さ（画面高に対する割合）
    public float GetSmokeHeightRate(int turnTrash)
    {
        return Mathf.Clamp(smokeHeightBase + turnTrash * smokeHeightPerTrash, smokeHeightMin, smokeHeightMax);
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
