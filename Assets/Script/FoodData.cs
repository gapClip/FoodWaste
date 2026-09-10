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

[CreateAssetMenu(menuName = "Food/FoodData")]
public class FoodData : ScriptableObject
{
    public string foodName;
    public Sprite foodSprite;
    public float amount;

    [Header("食べ物の属性")]
    public FoodCategory category;
    public FoodTexture texture;
}