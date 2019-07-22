using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenControl : MonoBehaviour {

    public PlayerControl.PlayerController player;

    private bool inGame;

    public void Start() {
        inGame = true;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (inGame) {
                PauseInGame();
            }
            else {
                ResumeInGame();
            }
        }

        //if (Input.GetKeyDown(KeyCode.Mouse0) && !player.screenControl) {
        //    Cursor.lockState = CursorLockMode.Locked;
        //    player.screenControl = true;
        //}
    }

    public void PauseInGame() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (player != null) player.screenControl = false;
        inGame = false;
    }

    public void ResumeInGame() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (player != null) player.screenControl = true;
        inGame = true;
    }
}
