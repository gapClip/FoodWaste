using System.Collections.Generic;
using UnityEngine;

// ターンをまたいで保持する状態（ターン数・累計CO₂・動物ごとの累計）
// シーンを切り替えるとシーン上のオブジェクトは消えるので、ここに集約する
public static class GameState
{
    // 現在のターン（1〜GameBalance.turnCount）
    public static int turn;

    // 累計CO₂（＝これまでに燃やしたゴミの合計）
    public static int totalCo2;

    // 今ターンの給餌で出たゴミ
    public static int turnFeedTrash;

    // 今ターンに使い切らなかった食糧庫のゴミ（餌やり終了時に GoResult が設定する）
    public static int turnUnusedTrash;

    // 今ターンのゴミをもう燃やしたか（焼却の処理を1ターンに1回だけにする）
    public static bool incineratedThisTurn;

    // 焼却する前のCO₂段階（段階が上がったかの判定に使う）
    public static int co2StageBeforeIncineration;

    private static readonly Dictionary<MonsterData, MonsterState> monsterStates =
        new Dictionary<MonsterData, MonsterState>();

    // 今ターンの総ゴミ
    public static int TurnTrash => turnFeedTrash + turnUnusedTrash;

    public static int Co2Stage => GameBalance.Instance.GetCo2Stage(totalCo2);

    public static float Pollution => GameBalance.Instance.GetPollution(totalCo2);

    // 今ターンの焼却でCO₂段階が上がったか（ゴミ処理画面の暗転＋テロップ用）
    public static bool Co2StageRaised => incineratedThisTurn && Co2Stage > co2StageBeforeIncineration;

    public static bool IsLastTurn => turn >= GameBalance.Instance.turnCount;

    // プレイモード開始時に必ず初期化する
    // （Enter Play Mode Options でドメインリロードを切っていても値が残らないように）
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void NewGame()
    {
        turn = 1;
        totalCo2 = 0;
        turnFeedTrash = 0;
        turnUnusedTrash = 0;
        incineratedThisTurn = false;
        co2StageBeforeIncineration = 0;
        monsterStates.Clear();
    }

    // 動物の状態を取得する（無ければ作る）
    public static MonsterState GetMonsterState(MonsterData data)
    {
        if (data == null)
        {
            Debug.LogWarning("MonsterData が設定されていません");
            return null;
        }

        MonsterState state;

        if (!monsterStates.TryGetValue(data, out state))
        {
            state = new MonsterState(data);
            monsterStates.Add(data, state);
        }

        return state;
    }

    // 表示用。MonsterData.resultOrder の順に並べて返す
    public static List<MonsterState> GetMonsterStates()
    {
        List<MonsterState> states = new List<MonsterState>(monsterStates.Values);

        states.Sort((a, b) => a.data.resultOrder.CompareTo(b.data.resultOrder));

        return states;
    }

    // 給餌でこぼれたゴミを加算する
    public static void AddFeedTrash(int amount)
    {
        turnFeedTrash += amount;
    }

    // 今ターンのゴミを燃やす（SPEC 8.2）。ゴミを燃やし終えたときに呼ぶ。2回目以降は何もしない
    public static void Incinerate()
    {
        if (incineratedThisTurn)
        {
            return;
        }

        incineratedThisTurn = true;
        co2StageBeforeIncineration = Co2Stage;

        int trash = TurnTrash;

        // ごみの量 ＝ 二酸化炭素の量
        totalCo2 += trash;

        // 焚き火の煙で全動物の満足度が下がる。満足度は0未満にしない
        float penalty = GameBalance.Instance.smokePenalty * trash;

        foreach (MonsterState state in monsterStates.Values)
        {
            state.satisfaction = GameBalance.RoundToTenth(Mathf.Max(state.satisfaction + penalty, 0f));
        }
    }

    // ゴミ処理が終わって次のターンへ進む
    public static void AdvanceTurn()
    {
        // 燃やす処理を通らずに進んだ場合も、ここで必ず行う
        Incinerate();

        turnFeedTrash = 0;
        turnUnusedTrash = 0;
        incineratedThisTurn = false;

        foreach (MonsterState state in monsterStates.Values)
        {
            state.eatenThisTurn = 0;

            // 成長段階はリザルトの時点で決まり、次のターンの容量・見た目に反映する
            state.levelAtTurnStart = state.Level;
        }

        turn++;
    }
}
