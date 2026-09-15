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

    // 今ターンに使い切らなかった食糧庫のゴミ（Phase 3 で設定する）
    public static int turnUnusedTrash;

    private static readonly Dictionary<MonsterData, MonsterState> monsterStates =
        new Dictionary<MonsterData, MonsterState>();

    // 今ターンの総ゴミ
    public static int TurnTrash => turnFeedTrash + turnUnusedTrash;

    public static int Co2Stage => GameBalance.Instance.GetCo2Stage(totalCo2);

    public static float Pollution => GameBalance.Instance.GetPollution(totalCo2);

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

    // ゴミ処理が終わって次のターンへ進む
    // （満足度への煙のペナルティと成長判定は Phase 3 でここに追加する）
    public static void AdvanceTurn()
    {
        // ごみの量 ＝ 二酸化炭素の量
        totalCo2 += TurnTrash;

        turnFeedTrash = 0;
        turnUnusedTrash = 0;

        foreach (MonsterState state in monsterStates.Values)
        {
            state.eatenThisTurn = 0;
        }

        turn++;
    }
}
