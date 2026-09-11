using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/Data")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    [Tooltip("基礎容量（Lv1の容量）")]
    public int amountEat;
    public int resultOrder;

    [Header("好み表（カテゴリ × 食感）SPEC 5.3")]
    public PreferenceRow[] preferenceTable =
    {
        new PreferenceRow { category = FoodCategory.肉 },
        new PreferenceRow { category = FoodCategory.果実 },
        new PreferenceRow { category = FoodCategory.魚 },
    };

    public FoodPreference GetPreference(FoodCategory category, FoodTexture texture)
    {
        foreach (PreferenceRow row in preferenceTable)
        {
            if (row.category == category)
            {
                return row.Get(texture);
            }
        }

        Debug.LogWarning($"{monsterName} の好み表に {category} がありません");
        return FoodPreference.普通;
    }
}

// 好み表の1行（1カテゴリ分）
[System.Serializable]
public class PreferenceRow
{
    public FoodCategory category;
    public FoodPreference ドロドロ;
    public FoodPreference パサパサ;
    public FoodPreference ゴツゴツ;

    public FoodPreference Get(FoodTexture texture)
    {
        return texture switch
        {
            FoodTexture.ドロドロ => ドロドロ,
            FoodTexture.パサパサ => パサパサ,
            _ => ゴツゴツ,
        };
    }
}
