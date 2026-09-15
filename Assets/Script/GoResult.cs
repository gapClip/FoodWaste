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

        // 食糧庫に残った食べ物は、使わなかった餌としてゴミになる（SPEC 8.2）
        Inventory inventory = FindFirstObjectByType<Inventory>();
        GameState.turnUnusedTrash = inventory != null ? inventory.RemainingFullness() : 0;

        SceneManager.LoadScene(sceneName);
    }
}
