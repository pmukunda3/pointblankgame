using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverText : MonoBehaviour
{
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.enabled = false;
        EventManager.StartListening<PlayerControl.PlayerDeathEvent>(new UnityEngine.Events.UnityAction(EnableText));
    }

    // Update is called once per frame
    void EnableText()
    {
        text.enabled = true;
    }
}
