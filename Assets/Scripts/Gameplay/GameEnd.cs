using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour {

    public LayerMask triggerLayer;
    public UnityEngine.UI.Image blackImage;
    public float fadeTime = 2.0f;
    private float elapsedTime = 0.0f;
    public bool fade = false;

    private void Start() {
    }

    public void Update() {
        if (fade) {
            if (elapsedTime < fadeTime) {
                blackImage.color = new Color(1.0f, 1.0f, 1.0f, elapsedTime / fadeTime);
            }
            else {
                UnityEngine.SceneManagement.SceneManager.LoadScene(3);
            }

            elapsedTime += Time.deltaTime;
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (fade == false && ((1 << other.gameObject.layer) & triggerLayer) != 0) {
            fade = true;
        }
    }
}
