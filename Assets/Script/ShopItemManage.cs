using System.Collections.Generic;
using UnityEngine;

public class PrefabGridSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [Header("並べるFoodData")]
    [SerializeField] private FoodData[] foodData;

    [Header("購入ボタン")]
    [SerializeField] private Buy buy;

    private GameObject[] foodObjects;

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        float[] xPositions = { -600f, -100f };
        float[] yPositions = { 250f, -250f };

        // 配置するFoodDataの数
        int spawnCount = xPositions.Length * yPositions.Length;

        // FoodDataが足りない場合
        if (foodData.Length < spawnCount)
        {
            Debug.LogError("FoodDataの数が足りません");
            return;
        }

        foodObjects = new GameObject[spawnCount];

        // 並べるFoodDataを抽選する（SPEC 6.2。ルールは ShopLottery）
        List<FoodData> randomFoods = ShopLottery.Draw(foodData, spawnCount);

        int index = 0;

        for (int y = 0; y < yPositions.Length; y++)
        {
            for (int x = 0; x < xPositions.Length; x++)
            {
                GameObject obj = Instantiate(prefab, transform);

                RectTransform rect = obj.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(
                    xPositions[x],
                    yPositions[y]
                );

                // ランダムなFoodDataを設定
                TestAdd testAdd = obj.GetComponent<TestAdd>();

                if (testAdd != null)
                {
                    testAdd.SetFood(randomFoods[index]);
                }

                // Buy用に保存
                foodObjects[index] = obj;

                index++;
            }
        }

        // Buyに生成した商品一覧を渡す
        buy.SetFoodObjects(foodObjects);
    }
}