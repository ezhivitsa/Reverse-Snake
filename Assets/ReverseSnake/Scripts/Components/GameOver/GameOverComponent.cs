using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverComponent : MonoBehaviour
{
    public string mainMenuScene;

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
