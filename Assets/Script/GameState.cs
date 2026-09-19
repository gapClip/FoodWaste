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

    // ショップの陳列（SPEC 6.2）。次のターンの抽選で「前のターンと同じ並び」を避けるために覚えておく
    private static readonly List<FoodData> shopLineup = new List<FoodData>();

    // shopLineup を抽選したターン（0 はまだ抽選していない）
    private static int shopLineupTurn;

    // 今ターンに買った数（食べ物ごと。SPEC 6.1 の購入上限の判定に使う）
    private static readonly Dictionary<FoodData, int> boughtThisTurn = new Dictionary<FoodData, int>();

    // 今ターンの総ゴミ
    public static int TurnTrash => turnFeedTrash + turnUnusedTrash;

    public static int Co2Stage => GameBalance.Instance.GetCo2Stage(totalCo2);

    public static float Pollution => GameBalance.Instance.GetPollution(totalCo2);

    // 今ターンの焼却でCO₂段階が上がったか（ゴミ処理画面の暗転＋テロップ用）
    public static bool Co2StageRaised => incineratedThisTurn && Co2Stage > co2StageBeforeIncineration;

    // ここから5つは、ゴミ処理画面の焚き火の演出に渡す値（SPEC 9.3）
    // どれも今ターンの総ゴミ TurnTrash から毎回求める（保存しない）
    // TurnTrash は AdvanceTurn() まで変わらないので、燃やす前も燃やし終えたあとも同じ値を返す

    // 焚き火ティア（0〜4）
    public static int BonfireTier => GameBalance.Instance.GetBonfireTier(TurnTrash);

    // 燃える演出の継続時間（秒）
    public static float BonfireDuration => GameBalance.Instance.GetBonfireDuration(TurnTrash);

    // 煙のパーティクルの数。ゴミ0でも式どおり基本の数を返すので、ゴミ0のときは NothingToBurn を見て演出を省ける
    public static int SmokeParticleCount => GameBalance.Instance.GetSmokeParticleCount(TurnTrash);

    // 煙の高さ（画面の高さに対する割合）
    public static float SmokeHeightRate => GameBalance.Instance.GetSmokeHeightRate(TurnTrash);

    // 今ターンは燃やすものが無い（ゴミ0。「今日は燃やすものがありません」の表示用）
    public static bool NothingToBurn => TurnTrash <= 0;

    public static bool IsLastTurn => turn >= GameBalance.Instance.turnCount;

    // 最後に抽選したショップの陳列
    public static IReadOnlyList<FoodData> ShopLineup => shopLineup;

    // 今ターンの陳列をもう抽選したか（再入荷は毎ターン1回。同じターンには引き直さない）
    public static bool HasShopLineupThisTurn => shopLineupTurn == turn;

    // 今ターンに買った数の合計
    public static int TotalBoughtThisTurn
    {
        get
        {
            int total = 0;

            foreach (int count in boughtThisTurn.Values)
            {
                total += count;
            }

            return total;
        }
    }

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
        shopLineup.Clear();
        shopLineupTurn = 0;
        boughtThisTurn.Clear();
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
    // 結果画面・エンディングは動物全員の分を出す（SPEC 3.6.1）ので、シーンからまだ登録されていない動物も初期状態で含める
    public static List<MonsterState> GetMonsterStates()
    {
        RegisterAllMonsters();

        List<MonsterState> states = new List<MonsterState>(monsterStates.Values);

        states.Sort((a, b) => a.data.resultOrder.CompareTo(b.data.resultOrder));

        return states;
    }

    // GameBalance.monsters（ゲームに登場する動物の全員）のうち、まだ登録されていない動物を初期状態で登録する
    // 登録済みの動物の状態は作り直さない。一覧が空（アセットに未設定）なら何もしない
    // NewGame() からは呼ばない（SubsystemRegistration の時点で Resources.Load をしないため）
    private static void RegisterAllMonsters()
    {
        GameBalance balance = GameBalance.Instance;

        if (balance == null || balance.monsters == null)
        {
            return;
        }

        foreach (MonsterData data in balance.monsters)
        {
            // 一覧の空欄は黙って飛ばす（GetMonsterState(null) の警告を、呼ぶたびに出さない）
            // 同じ動物が2回入っていても、GetMonsterState は登録済みのものを返すだけなので増えない
            if (data != null)
            {
                GetMonsterState(data);
            }
        }
    }

    // 給餌でこぼれたゴミを加算する
    public static void AddFeedTrash(int amount)
    {
        turnFeedTrash += amount;
    }

    // 今ターンのショップの陳列を記録する
    public static void SetShopLineup(List<FoodData> lineup)
    {
        shopLineup.Clear();
        shopLineup.AddRange(lineup);
        shopLineupTurn = turn;
    }

    // 今ターンにその食べ物を買った数
    public static int GetBoughtThisTurn(FoodData food)
    {
        int count;

        return food != null && boughtThisTurn.TryGetValue(food, out count) ? count : 0;
    }

    // 買った数を記録する
    public static void RecordPurchase(FoodData food, int count)
    {
        if (food == null || count <= 0)
        {
            return;
        }

        boughtThisTurn[food] = GetBoughtThisTurn(food) + count;
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

        // 購入上限は1ターンごと。陳列は次のターンの抽選で比べるので消さない
        boughtThisTurn.Clear();

        foreach (MonsterState state in monsterStates.Values)
        {
            state.eatenThisTurn = 0;

            // 成長段階はリザルトの時点で決まり、次のターンの容量・見た目に反映する
            state.levelAtTurnStart = state.Level;
        }

        turn++;
    }
}
