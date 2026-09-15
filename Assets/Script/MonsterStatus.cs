using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
    public MonsterData monsterData;

    // 今ターンに食べた量
    public int currentAmountEat;

    // 今ターンの容量。ターン開始時の成長段階で決まり、ターン中は変わらない
    public int maxAmountEat;

    // ターンをまたぐ累計（満足度・成長ポイント・Lv）は GameState が持つ
    public MonsterState State => GameState.GetMonsterState(monsterData);

    // シーンに置いたときの大きさ（Lv1 の大きさ）
    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Start()
    {
        maxAmountEat = State.CapacityAtTurnStart;
        currentAmountEat = 0;

        // 見た目の大きさも容量と同じサイズ倍率でスケールする（SPEC 4.1）
        transform.localScale = baseScale * State.SizeMultiplierAtTurnStart;
    }
}
