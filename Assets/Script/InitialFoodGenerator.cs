using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialFoodGenerator : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Inventory inventory;

    [Header("初期食料候補")]
    [SerializeField] private FoodData[] foodCandidates;

    [Header("食べ物の種類数")]
    [SerializeField] private int minFoodTypes = 4;
    [Header("食料の数補正係数")]
    [SerializeField] private float foodCountCorrection = 1.0f;

    private void Start()
    {
        StartCoroutine(GenerateAfterMonsterInitialize());
    }

    // MonsterStatusのStart()が終わってから初期食料を生成する
    private IEnumerator GenerateAfterMonsterInitialize()
    {
        // 1フレーム待つ
        yield return null;

        GenerateInitialFood();
    }

    public void GenerateInitialFood()
    {
        if (inventory == null)
        {
            Debug.LogError(
                "InitialFoodGenerator：Inventoryが設定されていません。"
            );
            return;
        }

        if (foodCandidates == null ||
            foodCandidates.Length < minFoodTypes)
        {
            Debug.LogError(
                "InitialFoodGenerator：FoodDataの候補数が足りません。"
            );
            return;
        }

        // ----------------------------------------
        // 全モンスターのmaxAmountEatを取得
        // ----------------------------------------

        MonsterStatus[] monsters =
            FindObjectsByType<MonsterStatus>(
                FindObjectsSortMode.None
            );

        int targetFullness = 0;

        foreach (MonsterStatus monster in monsters)
        {
            targetFullness += monster.maxAmountEat;
        }

        // 食料の生成量を係数で調整
        targetFullness = Mathf.RoundToInt(
            targetFullness * foodCountCorrection
        );


        Debug.Log(
            "モンスターのmaxAmountEat合計：" +
            targetFullness
        );

        Debug.Log(
    "係数適用後の目標満腹度：" +
    targetFullness
);

        if (targetFullness <= 0)
        {
            Debug.LogWarning(
                "maxAmountEatの合計が0以下です。"
            );
            return;
        }

        // ----------------------------------------
        // 4種類以上の食べ物をランダム選択
        // ----------------------------------------

        int maxFoodTypes =
            foodCandidates.Length;

        int foodTypeCount = Random.Range(
            minFoodTypes,
            maxFoodTypes + 1
        );

        List<FoodData> candidates =
            new List<FoodData>(foodCandidates);

        List<FoodData> selectedFoods =
            new List<FoodData>();

        for (int i = 0; i < foodTypeCount; i++)
        {
            int index = Random.Range(
                0,
                candidates.Count
            );

            selectedFoods.Add(
                candidates[index]
            );

            // 同じ食べ物がもう一度選ばれないようにする
            candidates.RemoveAt(index);
        }

        // ----------------------------------------
        // 選ばれた食べ物に個数を配分
        // ----------------------------------------

        int remainingFullness =
            targetFullness;

        for (int i = 0;
             i < selectedFoods.Count;
             i++)
        {
            FoodData food =
                selectedFoods[i];

            if (food == null)
            {
                continue;
            }

            if (food.amount <= 0)
            {
                Debug.LogWarning(
                    food.foodName +
                    " のamountが0以下です。"
                );

                continue;
            }

            int count;

            // 最後の食べ物で残りを調整
            if (i == selectedFoods.Count - 1)
            {
                count = Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        (float)remainingFullness /
                        food.amount
                    )
                );
            }
            else
            {
                int remainingTypes =
                    selectedFoods.Count - i;

                int target =
                    Mathf.CeilToInt(
                        (float)remainingFullness /
                        remainingTypes
                    );

                count = Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        (float)target /
                        food.amount
                    )
                );
            }

            // Inventoryに追加
            for (int j = 0; j < count; j++)
            {
                inventory.AddFood(food);
            }

            int addedFullness =
                food.amount * count;

            remainingFullness -=
                addedFullness;

            remainingFullness =
                Mathf.Max(
                    0,
                    remainingFullness
                );

            // Debug.Log(
            //     "初期食料：" +
            //     food.foodName +
            //     " × " +
            //     count +
            //     "個" +
            //     "（満腹度 " +
            //     addedFullness +
            //     "）"
            // );
        }

        // ----------------------------------------
        // 結果確認
        // ----------------------------------------

        Debug.Log(
            "初期食料生成完了。" +
            "目標：" +
            targetFullness +
            " / 実際：" +
            inventory.RemainingFullness()
        );
    }
}