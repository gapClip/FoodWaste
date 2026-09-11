using UnityEngine;
using UnityEngine.SceneManagement;

public class GoResult : ChangeScene
{
    override public void ChangeSceneTo()
    {
        // シーン上で食べた量を、ターンをまたぐ状態へ反映する
        MonsterStatus[] monsters =
            FindObjectsByType<MonsterStatus>(
                FindObjectsSortMode.None);

        foreach (MonsterStatus monster in monsters)
        {
            if (monster.monsterData == null)
            {
                continue;
            }

            GameState.GetMonsterState(monster.monsterData).eatenThisTurn =
                monster.currentAmountEat;
        }

        // 未給餌ゴミ（食糧庫の残り）の集計は Phase 3 で追加する

        SceneManager.LoadScene(sceneName);
    }
}
