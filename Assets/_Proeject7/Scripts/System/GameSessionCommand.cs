using UnityEngine;
using UnityEngine.SceneManagement;

public interface IGameSessionCommand
{
    void Restart();
    void Quit();
}

public sealed class GameSessionCommand : IGameSessionCommand
{
    public void Restart()
    {
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
