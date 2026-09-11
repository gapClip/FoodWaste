using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
    public MonsterData monsterData;

    // 今ターンに食べた量
    public int currentAmountEat;

    // 今ターンの容量。ターン開始時に成長段階から決まり、ターン中は変わらない
    public int maxAmountEat;

    // ターンをまたぐ累計（満足度・成長ポイント・Lv）は GameState が持つ
    public MonsterState State => GameState.GetMonsterState(monsterData);

    void Start()
    {
        maxAmountEat = State.Capacity;
        currentAmountEat = 0;
    }
}
