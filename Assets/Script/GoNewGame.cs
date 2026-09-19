using UnityEngine.SceneManagement;

// 新しいゲームを始めてからシーンを移る（SPEC 2・3.6.5：エンディング → タイトル → 2周目）
// GameState.NewGame() が自動で呼ばれるのはアプリ（プレイモード）の起動時だけで、シーンを移っても呼ばれない
// これを通らずに2周目へ入ると、ターン数・累計CO₂・動物の成長が前回のまま始まってしまう
// 付ける場所：2周目が始まる前に必ず通るボタン。タイトルの開始ボタンがおすすめ
// （どの道でタイトルに来ても初期化される）。使い方は ChangeScene と同じで、sceneName に行き先を書く
public class GoNewGame : ChangeScene
{
    override public void ChangeSceneTo()
    {
        // 初期化が先。次のシーンの Start()（動物の容量と大きさ、ショップの抽選）が、初期化のあとの値を読むように
        GameState.NewGame();

        SceneManager.LoadScene(sceneName); // シーン名で指定
    }
}
