using UnityEngine;
using System.Collections.Generic;

public static class ResultData
{
    // そのシーンの食べ残し合計
    public static float sceneLeftover = 0;

    // ゲーム全体の食べ残し累計
    public static float totalLeftover = 0;

    // その日の各モンスターの満腹度
    public static Dictionary<MonsterData, float> monsterFullness
            = new Dictionary<MonsterData, float>();
    public static void ResetScene()
    {
        sceneLeftover = 0;
        monsterFullness.Clear();
    }
}
