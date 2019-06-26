using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public float speed;
    public GameObject Character;
    public GameObject Camera;

    public float X_Sensitivity;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = Character.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Rotate(0, X_Sensitivity * Input.GetAxis("Mouse X"), 0);
        transform.Translate(speed * Time.deltaTime * Input.GetAxis("Horizontal"), 0, speed * Time.deltaTime * Input.GetAxis("Vertical"));
        anim.SetFloat("VelX", Input.GetAxis("Horizontal"));
        anim.SetFloat("VelY", Input.GetAxis("Vertical"));
        if (Input.GetButtonDown("Toggle Run"))
        {
            anim.SetBool("Run", !anim.GetBool("Run"));
        }
    }

}
