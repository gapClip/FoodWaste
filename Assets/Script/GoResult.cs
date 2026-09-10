using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GoResult : ChangeScene
{
    override public void ChangeSceneTo()
    {
        MonsterStatus[] monsters =
            FindObjectsByType<MonsterStatus>(
                FindObjectsSortMode.None);

        // 固定順に並べる
        monsters = monsters
            .OrderBy(monster => monster.monsterData.resultOrder)
            .ToArray();

        ResultData.monsterFullness.Clear();

        foreach (MonsterStatus monster in monsters)
        {
            ResultData.monsterFullness[monster.monsterData] =
                monster.currentAmountEat;
        }

        SceneManager.LoadScene(sceneName);
    }
}