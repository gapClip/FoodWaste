using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("モンスター満腹度")]
    [SerializeField] private GameObject monsterGraphPrefab;
    [SerializeField] private Transform monsterGraphParent;

    void Start()
    {
        List<MonsterState> states = GameState.GetMonsterStates();

        string text = "";

        text += $"ターン {GameState.turn} / {GameBalance.Instance.turnCount}\n";
        text += $"今ターンのゴミ：{GameState.TurnTrash}\n";
        text += $"累計CO₂：{GameState.totalCo2}\n\n";

        foreach (MonsterState state in states)
        {
            text += $"{state.data.monsterName}：" +
                    $"満腹 {state.eatenThisTurn}/{state.Capacity}　" +
                    $"満足度 {state.SatisfactionPercent}%　" +
                    $"Lv{state.Level}\n";
        }

        resultText.text = text;

        // 円グラフを生成
        CreateMonsterGraphs(states);
    }

    private void CreateMonsterGraphs(List<MonsterState> states)
    {
        foreach (MonsterState state in states)
        {
            // 円グラフPrefabを生成
            GameObject graph =
                Instantiate(monsterGraphPrefab, monsterGraphParent);

            // 円グラフのImageを取得
            Image circle =
                graph.transform.Find("Circle").GetComponent<Image>();

            // パーセント表示を取得
            TextMeshProUGUI percentageText =
                graph.transform.Find("PercentageText")
                .GetComponent<TextMeshProUGUI>();

            // 円グラフを設定
            circle.fillAmount = state.FullnessRate;

            // パーセントを表示
            percentageText.text =
                $"{state.FullnessRate * 100f:F1}%";
        }
    }
}
