using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFloatingDelVehCtrl : MonoBehaviour
{
    public GameObject[] Robots;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        EventManager.StartListening<RobotDropOff, GameObject>(new UnityEngine.Events.UnityAction<GameObject>(dropOff));
    }

    void dropOff(GameObject obj)
    {
        for (int i = 0; i < Robots.Length; ++i)
        {
            Robots[i].transform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
