using UnityEngine;

public class Monster : MonoBehaviour
{
    private MonsterStatus monsterStatus;
    private float overflow = 0;
    private Animator animator;
    [SerializeField] private GameObject trash;

    [SerializeField] private float trashMinX = -1.5f;
    [SerializeField] private float trashMaxX = 1.5f;
    [SerializeField] private float trashMinY = -0.5f;
    [SerializeField] private float trashMaxY = 0.5f;

    void Awake()
    {
        monsterStatus = GetComponent<MonsterStatus>();
        animator = GetComponent<Animator>();
    }

    public void Feed(InventoryItem item)
    {
        if (item.selectedCount > 0)
        {
            if (monsterStatus.currentAmountEat >= monsterStatus.maxAmountEat)
            {
                Debug.Log("これ以上食べられません！");
                return;
            }

            Debug.Log(item.food.foodName + " を食べた！");
            PlayFoodReaction(item);
            monsterStatus.currentAmountEat += item.food.amount * item.selectedCount;

            if (monsterStatus.currentAmountEat > monsterStatus.maxAmountEat)
            {
                overflow = monsterStatus.currentAmountEat - monsterStatus.maxAmountEat;
                monsterStatus.currentAmountEat = monsterStatus.maxAmountEat;

                // 食べ残しが発生したらゴミを生成
                SpawnTrash();
            }

            Debug.Log("現在の食べた量: " + monsterStatus.currentAmountEat);
            Debug.Log("残した量: " + overflow);
            Debug.Log("満足度: " + MonsterStatus.satisfaction);
            Debug.Log("成長度: " + MonsterStatus.growth);

            ResultData.sceneLeftover += overflow;
            ResultData.totalLeftover += overflow;

            item.ownedCount -= item.selectedCount;
            item.selectedCount = 0;
        }
    }

    private void PlayFoodReaction(InventoryItem item)
    {
        FoodCategory category = item.food.category;
        FoodTexture texture = item.food.texture;

        bool categoryFavorite =
            category == monsterStatus.monsterData.favoriteCategory;

        bool categoryDislike =
            category == monsterStatus.monsterData.dislikeCategory;

        bool textureFavorite =
            texture == monsterStatus.monsterData.favoriteTexture;

        bool textureDislike =
            texture == monsterStatus.monsterData.dislikeTexture;


        // 大好き：好き × 好き
        if (categoryFavorite && textureFavorite)
        {
            animator.SetTrigger("love");
            Debug.Log("大好き！");
            MonsterStatus.satisfaction += 3*item.selectedCount;
            MonsterStatus.growth += item.food.amount*item.selectedCount*1.5f;
        }
        // 嫌い：嫌い × 嫌い
        else if (categoryDislike && textureDislike)
        {
            animator.SetTrigger("dislike");
            Debug.Log("嫌い！");
            MonsterStatus.satisfaction -= 2*item.selectedCount;
        }
        // 好き：好き × 普通、普通 × 好き
        else if ((categoryFavorite && !textureDislike) ||
                 (textureFavorite && !categoryDislike))
        {
            animator.SetTrigger("like");
            Debug.Log("好き！");
            MonsterStatus.satisfaction += 1*item.selectedCount;
            MonsterStatus.growth += item.food.amount*item.selectedCount;
            MonsterStatus.growth += item.food.amount*item.selectedCount*1.2f;
        }
        // その他
        else
        {
            animator.SetTrigger("normal");
            Debug.Log("普通！");
            MonsterStatus.satisfaction += 0*item.selectedCount;
            MonsterStatus.growth += item.food.amount*item.selectedCount;
        }
    }

    private void SpawnTrash()
    {
        if (trash == null)
        {
            Debug.LogWarning("trashプレハブが設定されていません！");
            return;
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(trashMinX, trashMaxX),
            -2,
            0f
        );

        Vector3 spawnPosition = transform.position + randomOffset;

        Instantiate(trash, spawnPosition, Quaternion.identity);
    }
}