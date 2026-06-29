using UnityEngine;

public class Monster : MonoBehaviour
{
    private MonsterStatus monsterStatus;
    private float overflow = 0;
    void Awake()
    {
        monsterStatus = GetComponent<MonsterStatus>();
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
            monsterStatus.currentAmountEat += item.food.amount * item.selectedCount;
            if (monsterStatus.currentAmountEat > monsterStatus.maxAmountEat)
            {
                overflow = monsterStatus.currentAmountEat - monsterStatus.maxAmountEat;
                monsterStatus.currentAmountEat = monsterStatus.maxAmountEat;
            }
            Debug.Log("現在の食べた量: " + monsterStatus.currentAmountEat);
            Debug.Log("残した量: " + overflow);
            ResultData.sceneLeftover += overflow;
            ResultData.totalLeftover += overflow;

            item.ownedCount -= item.selectedCount;
            item.selectedCount = 0;
        }
    }
}