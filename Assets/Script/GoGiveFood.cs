using UnityEngine;
using UnityEngine.SceneManagement;


public class GoGiveFood : ChangeScene
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    override public void ChangeSceneTo()
    {
        ResultData.ResetScene();
      SceneManager.LoadScene(sceneName); // シーン名で指定
    }
}
