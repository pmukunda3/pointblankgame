using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeUI : MonoBehaviour
{
    private Text message;

    void Start()
    {
        message = GetComponentInChildren<Text>();
    }

    public void DisplayMessage(string messageToDisplay)
    {
        message.text = messageToDisplay;
    }
}
