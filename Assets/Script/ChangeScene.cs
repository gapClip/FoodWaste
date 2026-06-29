using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;
    virtual public void ChangeSceneTo()
    {
      SceneManager.LoadScene(sceneName); // シーン名で指定
    }
}
