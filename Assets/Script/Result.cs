using UnityEngine;
using TMPro;

public class Result : MonoBehaviour
{
     [SerializeField] private TextMeshProUGUI resultText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            text += $"{kvp.Key.monsterName}：{kvp.Value}\n";
        }

        resultText.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
