using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
    public MonsterData monsterData;

    // 今ターンに食べた量
    public int currentAmountEat;

    // 今ターンの容量。ターン開始時の成長段階で決まり、ターン中は変わらない
    public int maxAmountEat;

     // 食欲が0になって眠っているか
    public bool isSleeping;//9/25追加

    [SerializeField] private Animator animator;//9/25追加


    // ターンをまたぐ累計（満足度・成長ポイント・Lv）は GameState が持つ
    public MonsterState State => GameState.GetMonsterState(monsterData);

    // シーンに置いたときの大きさ（Lv5＝いちばん大きいときの大きさ）
    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Start()
    {
        maxAmountEat = State.CapacityAtTurnStart;
        currentAmountEat = 0;
          isSleeping = false;//9/25追加


        // 見た目は縦・横それぞれ「見た目の倍率」でスケールする（SPEC 4.1）
        // シーンには Lv5 の大きさで置いてあるので、Lv1 は縦横 1/1.4（面積で約1/2）になる
        transform.localScale = baseScale * State.PlacedScaleAtTurnStart;
    }

    public void SetSleeping(bool sleeping)//9/25追加
{
    isSleeping = sleeping;

    if (animator != null)
    {
        animator.SetBool("isSleeping", sleeping);
    }
}
}
