using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameStarter : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("alpha_level0");
    }

    public void LoadStory()
    {
        SceneManager.LoadScene("StoryMenu");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("alpha_level0");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("GameStartMenu");
    }

    public void AboutGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
