using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultForEnding : MonoBehaviour
{
    [Header("対象モンスター")]
    [SerializeField] private MonsterData targetMonster;

    [Header("成長段階スライダー")]
    [SerializeField] private Slider levelSlider;

    [Header("味ランクスライダー")]
    [SerializeField] private Slider tasteSlider;
    [Header("ごみ袋カウンター")]
    [SerializeField] private TextMeshProUGUI trashCountText;


    // ========================================
    // 計算済みの結果
    // ========================================

    private int resultLevel;
    private int resultTasteRank;
    private float resultLeftoverRate;

    private bool resultCalculated = false;
    public int trashCount;


    private void Start()
    {
        GetMonsterResult();
    }


    private void GetMonsterResult()
    {
       if (targetMonster == null)
    {
        Debug.LogWarning(
            "対象モンスターが設定されていません。"
        );

        return;
    }

    // 対象MonsterDataに対応するMonsterStatusを取得
    MonsterStatus[] monsters =
        FindObjectsByType<MonsterStatus>(
            FindObjectsSortMode.None
        );

    


    MonsterState state =
        GameState.GetMonsterState(targetMonster);

    if (state == null)
    {
        return;
    }


        // ========================================
        // ステータス取得
        // ========================================

        resultLevel = state.Level;
        resultTasteRank = state.TasteRank;


        // ========================================
        // 余り率をここで1回だけ決定
        // ========================================

        resultLeftoverRate =
            GetLeftoverRate(
                resultLevel,
                resultTasteRank
            );


        resultCalculated = true;


        Debug.Log(
            $"対象モンスター: {targetMonster.name}"
        );

        Debug.Log(
            $"成長段階: Lv{resultLevel}"
        );

        Debug.Log(
            $"味ランク: ★{resultTasteRank}"
        );

        Debug.Log(
            $"出荷時の余り率: " +
            $"{resultLeftoverRate * 100f:F1}%"
        );


        // ========================================
        // スライダー
        // ========================================

        SetLevelSlider(resultLevel);

        SetTasteSlider(
            resultLevel,
            resultTasteRank,
            resultLeftoverRate
        );
        UpdateTrashCountText();
    }


    // ========================================
    // 成長段階スライダー
    // ========================================

    private void SetLevelSlider(int level)
    {
        if (levelSlider == null)
        {
            Debug.LogWarning(
                "成長段階スライダーが設定されていません。"
            );

            return;
        }


        level = Mathf.Clamp(level, 1, 5);


        // Lv1 = 0.4
        // Lv2 = 0.55
        // Lv3 = 0.7
        // Lv4 = 0.85
        // Lv5 = 1.0

        float levelValue =
            Mathf.Lerp(
                0.4f,
                1.0f,
                (level - 1) / 4f
            );


        levelSlider.value = levelValue;
    }


    // ========================================
    // 味ランクスライダー
    // ========================================

    private void SetTasteSlider(
        int level,
        int tasteRank,
        float leftoverRate)
    {
        if (tasteSlider == null)
        {
            Debug.LogWarning(
                "味ランクスライダーが設定されていません。"
            );

            return;
        }


        level = Mathf.Clamp(level, 1, 5);
        tasteRank = Mathf.Clamp(tasteRank, 1, 5);


        // 余らなかった量
        float consumptionRate =
            1f - leftoverRate;


        // スライダーには消費率を表示
        tasteSlider.value =
            levelSlider.value * consumptionRate;


        Debug.Log(
            $"出荷時の消費率: " +
            $"{consumptionRate * 100f:F1}%"
        );
    }


    // ========================================
    // 出荷時の余り率を計算
    // ========================================

    private float GetLeftoverRate(
        int level,
        int tasteRank)
    {
        // 味ランクごとの基本余り率
        float baseRate = 0f;


        switch (tasteRank)
        {
            case 5:
                baseRate = 0.00f;
                break;

            case 4:
                baseRate = 0.10f;
                break;

            case 3:
                baseRate = 0.25f;
                break;

            case 2:
                baseRate = 0.35f;
                break;

            case 1:
                baseRate = 0.50f;
                break;
        }


        // 成長段階が高いほど少し余りやすくする
        float levelBonus =
            (level - 1) * 0.02f;


        // 味ランクが高いほど成長による余り補正を小さくする
        float tasteInfluence =
            (5 - tasteRank) / 4f;


        levelBonus *= tasteInfluence;


        float rate =
            baseRate + levelBonus;


        // ========================================
        // ランダム補正
        // ========================================

        // ★5は基本的に余らない
        // 10%の確率で0～10%の余り
        if (tasteRank == 5)
        {
            if (Random.value < 0.10f)
            {
                rate =
                    Random.Range(
                        0f,
                        0.10f
                    );

                Debug.Log(
                    $"★5の余りに揺らぎが発生！ " +
                    $"余り率: {rate * 100f:F1}%"
                );
            }
            else
            {
                rate = 0f;
            }
        }
        else
        {
            // ★1～★4は±10%の揺らぎ
            rate +=
                Random.Range(
                    -0.10f,
                    0.10f
                );
        }


        return Mathf.Clamp01(rate);
    }


    // ========================================
    // SNSResultDisplayから結果を取得
    // ========================================

    public bool TryGetResult(
        out int level,
        out int tasteRank,
        out float leftoverRate)
    {
        level = 0;
        tasteRank = 0;
        leftoverRate = 0f;


        // まだ計算されていなければ計算する
        if (!resultCalculated)
        {
            GetMonsterResult();
        }


        if (!resultCalculated)
        {
            return false;
        }


        level = resultLevel;
        tasteRank = resultTasteRank;
        leftoverRate = resultLeftoverRate;


        return true;
    }

public void UpdateTrashCountText()
{
    if (trashCountText == null)
    {
        Debug.LogWarning(
            "ごみ袋カウンターのテキストが設定されていません。"
        );

        return;
    }

    if (targetMonster == null)
    {
        Debug.LogWarning(
            "対象モンスターが設定されていません。"
        );

        return;
    }

    // 成長段階を1～5に制限
    int level = Mathf.Clamp(resultLevel, 1, 5);

    // Lv1 = 1.0
    // Lv2 = 1.25
    // Lv3 = 1.5
    // Lv4 = 1.75
    // Lv5 = 2.0
    float growthMultiplier =
        1.0f + (level - 1) * 0.25f;

    // ごみ袋数を計算
    float calculatedCount =
        targetMonster.amountEat
        * growthMultiplier
        * (1-tasteSlider.value)
        / 2f;

    // 小数点以下があれば1繰り上げ
    trashCount =
        Mathf.CeilToInt(calculatedCount);

    trashCountText.text =
        $"ごみ袋: {trashCount}";
        FinalIncineration.finalTrashCount += trashCount;
        Debug.Log($"FinalIncineration.finalTrashCount: {FinalIncineration.finalTrashCount}");
}

}