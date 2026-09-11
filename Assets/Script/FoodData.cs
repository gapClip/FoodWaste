using UnityEngine;

public enum FoodCategory
{
    肉,
    果実,
    魚
}

public enum FoodTexture
{
    ドロドロ,
    パサパサ,
    ゴツゴツ
}

// 動物ごとの好み（◎○△×）。先頭の 普通 が初期値
public enum FoodPreference
{
    普通,
    好き,
    大好き,
    大嫌い
}

[CreateAssetMenu(menuName = "Food/FoodData")]
public class FoodData : ScriptableObject
{
    public string foodName;
    public Sprite foodSprite;

    [Header("食べ物の属性")]
    public FoodCategory category;
    public FoodTexture texture;

    // 満腹度はカテゴリと食感から算出する（SPEC 5.1）。値の調整は GameBalance で行う
    public int amount => GameBalance.Instance.GetFullness(category, texture);
}