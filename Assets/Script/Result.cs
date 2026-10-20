using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI directorText;

    [Header("モンスター満腹度")]
    [SerializeField] private GameObject monsterGraphPrefab;
    [SerializeField] private Transform monsterGraphParent;

    void Start()
    {
        List<MonsterState> states = GameState.GetMonsterStates();

        string text = "";

       // text += $"ターン {GameState.turn} / {GameBalance.Instance.turnCount}\n";
        text += $"×{GameBalance.Instance.GetTrashBagCount(GameState.TurnTrash)}";
        //text += $"累計CO2：{GameState.totalCo2}\n\n";

        // foreach (MonsterState state in states)
        // {
        //     text += $"{state.data.monsterName}：" +
        //             $"満腹 {state.eatenThisTurn}/{state.CapacityAtTurnStart}　" +
        //             $"満足度 {state.SatisfactionPercent}%　" +
        //             $"Lv{state.Level}";

        //     // 成長段階が上がったとき（演出は UI 担当と相談）
        //     if (state.LeveledUp)
        //     {
        //         text += "　おおきくなった！";
        //     }

        //     text += "\n";
        // }

        resultText.text = text;

        if (GameBalance.Instance.GetTrashBagCount(GameState.TurnTrash)==0)
        {
            directorText.text = "ごみは0。よくやった！\n次も期待しているよ。";
        }
        else if (GameBalance.Instance.GetTrashBagCount(GameState.TurnTrash)>0&&GameBalance.Instance.GetTrashBagCount(GameState.TurnTrash)<3)
        {
            directorText.text = "ごみが出てしまったか。\n今日よりももっと減らせるよう\n頑張ってくれ。";
        }
        else
        {
            directorText.text = "これはまずいな。\nミスは誰にでもあるが、\n私が責任をとれる範囲でお願いするよ";
        }

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
            circle.fillAmount = state.growthRate;

            // パーセントを表示
            percentageText.text =
                $"{state.growthRate * 100f:F1}%";
        }
    }
}
