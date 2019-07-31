using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RobotFloatingDelVehCtrl : MonoBehaviour
{
    public GameObject[] Robots;
    public GameObject[] PatrolPoints;
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        EventManager.StartListening<RobotDropOff, int>(new UnityEngine.Events.UnityAction<int>(dropOff));
    }

    void dropOff(int j)
    {
        Debug.Log("Robot Dropped");
        for (int i = 0; i < Robots.Length; ++i)
        {
            Robot_AI_Ctrl rob = Robots[i].gameObject.GetComponent<Robot_AI_Ctrl>();
            rob.player = this.Player;
            rob.patrol_points = this.PatrolPoints;
            rob.RobotInVehicle = false;
            Robots[i].transform.parent = null;
            //Debug.Log(a.transform);
            //a.SetActive(true);

            //Robots[i].transform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
