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
        Debug.Log(ResultData.sceneLeftover);
        Debug.Log(ResultData.totalLeftover);
        Debug.Log(ResultData.monsterFullness.Count);

        foreach (var kvp in ResultData.monsterFullness)
        {
            Debug.Log($"Monster: {kvp.Key}, Fullness: {kvp.Value}");
        }

        string text = "";

        text += $"食べ残し（今回）：{ResultData.sceneLeftover}\n";
        text += $"食べ残し（累計）：{ResultData.totalLeftover}\n\n";

        text += "モンスターの満腹度\n";

        foreach (var kvp in ResultData.monsterFullness)
        {
            float fullnessPercent =
                kvp.Value / kvp.Key.amountEat * 100f;

            text += $"{kvp.Key.monsterName}：{fullnessPercent:F2}%\n";
        }

        resultText.text = text;

        // 円グラフを生成
        CreateMonsterGraphs();
    }


    private void CreateMonsterGraphs()
    {
        foreach (var kvp in ResultData.monsterFullness)
        {
            MonsterData monsterData = kvp.Key;
            float currentAmount = kvp.Value;

            // 0～1の割合に変換
            float fullnessRate =
                currentAmount / monsterData.amountEat;

            // 0～1の範囲に収める
            fullnessRate = Mathf.Clamp01(fullnessRate);

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
            circle.fillAmount = fullnessRate;

            // パーセントを表示
            percentageText.text =
                $"{fullnessRate * 100f:F1}%";
        }
    }
}