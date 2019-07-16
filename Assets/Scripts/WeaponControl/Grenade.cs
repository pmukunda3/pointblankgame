using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float timeout;

    private float clock;

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if (clock >= timeout)
        {
            gameObject.GetComponent<Exploder>().Explode();
        }
    }
}
