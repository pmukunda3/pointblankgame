using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameStarter : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Prototype");
    }
}
