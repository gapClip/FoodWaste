using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/Data")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public float amountEat;
    public int resultOrder;

    [Header("食べ物の好み")]
    public FoodCategory favoriteCategory;
    public FoodCategory dislikeCategory;

    public FoodTexture favoriteTexture;
    public FoodTexture dislikeTexture;
}
