using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerAroundMap : MonoBehaviour {

    public Transform[] teleportPoints;
    public Rigidbody playerRigidbody;

    public void Start() {
        Transform[] temp = gameObject.GetComponentsInChildren<Transform>();
        this.teleportPoints = new Transform[temp.Length - 1];
        for (int n = 0; n < this.teleportPoints.Length; ++n) {
            this.teleportPoints[n] = temp[n + 1];
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad1) && teleportPoints.Length > 0) playerRigidbody.position = teleportPoints[0].position;
        if (Input.GetKeyDown(KeyCode.Keypad2) && teleportPoints.Length > 1) playerRigidbody.position = teleportPoints[1].position;
        if (Input.GetKeyDown(KeyCode.Keypad3) && teleportPoints.Length > 2) playerRigidbody.position = teleportPoints[2].position;
        if (Input.GetKeyDown(KeyCode.Keypad4) && teleportPoints.Length > 3) playerRigidbody.position = teleportPoints[3].position;
        if (Input.GetKeyDown(KeyCode.Keypad5) && teleportPoints.Length > 4) playerRigidbody.position = teleportPoints[4].position;
        if (Input.GetKeyDown(KeyCode.Keypad6) && teleportPoints.Length > 5) playerRigidbody.position = teleportPoints[5].position;
        if (Input.GetKeyDown(KeyCode.Keypad7) && teleportPoints.Length > 6) playerRigidbody.position = teleportPoints[6].position;
        if (Input.GetKeyDown(KeyCode.Keypad8) && teleportPoints.Length > 7) playerRigidbody.position = teleportPoints[7].position;
        if (Input.GetKeyDown(KeyCode.Keypad9) && teleportPoints.Length > 8) playerRigidbody.position = teleportPoints[8].position;

        if (Input.GetKeyDown(KeyCode.Backspace)) Debug.Break();
    }
}
