using UnityEngine;
using UnityEngine.SceneManagement;


public class GoGiveFood : ChangeScene
{
    override public void ChangeSceneTo()
    {
        // ゴミ処理が終わったので次のターンへ進む
        GameState.AdvanceTurn();

        SceneManager.LoadScene(sceneName); // シーン名で指定
    }
}
