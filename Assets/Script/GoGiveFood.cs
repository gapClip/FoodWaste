using UnityEngine;
using UnityEngine.SceneManagement;


public class GoGiveFood : ChangeScene
{
    [Header("最後のターンが終わったあとに読み込むシーン（エンディングの最初のシーン）。空なら sceneName へ")]
    public string endingSceneName;

    override public void ChangeSceneTo()
    {
        // 最後のターンかどうかは、ターンを進める前に見る（進めたあとは turn が1つ増えている）
        bool goEnding = GameState.IsLastTurn && !string.IsNullOrEmpty(endingSceneName);

        // ゴミ処理が終わったので次のターンへ進む
        // 最後のターンでも呼ぶ（焼却の反映と、最終の成長段階の確定をここで行う）
        GameState.AdvanceTurn();

        SceneManager.LoadScene(goEnding ? endingSceneName : sceneName); // シーン名で指定
    }
}
