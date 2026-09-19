using UnityEngine;

public class RFETest : MonoBehaviour
{
    [Header("対象モンスター")]
    [SerializeField] private MonsterData targetMonster;

    [Header("テスト用設定")]
    [Range(1, 5)]
    [SerializeField] private int testLevel = 1;

    [Range(1, 5)]
    [SerializeField] private int testTasteRank = 1;
    [SerializeField] private int co2 = 0;

    private void Awake()
    {
        OverrideMonsterState();
    }


    private void OverrideMonsterState()
    {
        if (targetMonster == null)
        {
            Debug.LogWarning(
                "対象モンスターが設定されていません。"
            );

            return;
        }


        MonsterState state =
            GameState.GetMonsterState(targetMonster);


        if (state == null)
        {
            return;
        }


        // =========================
        // 成長レベルを上書き
        // =========================

        state.growthPoints =
            GetGrowthPointsForLevel(testLevel);


        // =========================
        // 味ランクを上書き
        // =========================

        state.satisfaction =
            GetSatisfactionForTasteRank(testTasteRank);

//　tatalco2を上書き
        GameState.totalCo2 = co2;


        // =========================
        // 確認
        // =========================

        Debug.Log(
            $"【RFETest】{targetMonster.name} " +
            $"Lv{state.Level} / ★{state.TasteRank} " +
            $"(Growth:{state.growthPoints:F2} / " +
            $"Satisfaction:{state.satisfaction:F2})"
        );
    }


    /// <summary>
    /// 指定したLvになる成長ポイントを取得
    /// </summary>
    private float GetGrowthPointsForLevel(int level)
    {
        level = Mathf.Clamp(
            level,
            1,
            GameBalance.Instance.MaxLevel
        );


        // Lv1
        if (level == 1)
        {
            return 0f;
        }


        // 指定Lvの閾値を少しだけ超える値を入れる
        // → 必ずそのLvになる
        return GameBalance.Instance.GetGrowthThreshold(
            targetMonster.amountEat,
            level
        ) + 0.01f;
    }


    /// <summary>
    /// 指定した★になる満足度を取得
    /// </summary>
    private float GetSatisfactionForTasteRank(int tasteRank)
    {
        tasteRank = Mathf.Clamp(
            tasteRank,
            1,
            5
        );


        int target =
            GameBalance.Instance.GetSatisfactionTarget(
                targetMonster.amountEat
            );


        // 境界値ではなく、各ランクの中間あたりを使用
        int percent;


        switch (tasteRank)
        {
            case 5:
                percent = 95;
                break;

            case 4:
                percent = 80;
                break;

            case 3:
                percent = 60;
                break;

            case 2:
                percent = 35;
                break;

            default:
                percent = 10;
                break;
        }


        return target * percent / 100f;
    }
}