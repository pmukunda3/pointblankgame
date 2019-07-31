using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameQuitter : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("clicked");
        Application.Quit();
       
    }
}
