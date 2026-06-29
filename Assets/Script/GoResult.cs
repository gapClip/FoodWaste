using UnityEngine;
using UnityEngine.SceneManagement;

public class GoResult : ChangeScene
{
    MonsterData monsterData;
    override public void ChangeSceneTo()
    {
        MonsterStatus[] monsters = FindObjectsByType<MonsterStatus>(
            FindObjectsSortMode.None);

        ResultData.monsterFullness.Clear();

        foreach (MonsterStatus monster in monsters)
        {
            ResultData.monsterFullness[monster.monsterData] =
                monster.currentAmountEat;
        }

        SceneManager.LoadScene(sceneName);
    }
}
