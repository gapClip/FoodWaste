using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
    public MonsterData monsterData;
    public  float currentAmountEat;
    public  float maxAmountEat;
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
