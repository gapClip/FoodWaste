using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
    public MonsterData monsterData;
    public  float currentAmountEat;
    public  float maxAmountEat;

    [Header("エンディングまで累積するステータス")]
    public static int satisfaction=0;
    public static float growth=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxAmountEat = monsterData.amountEat;
        currentAmountEat = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
