using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningRobot : MonoBehaviour
{
    public BoxCollider col;
    public Transform[] SpawnPoints;
    public GameObject Robot;
    public GameObject[] patrol_points;
    public GameObject player;
         




    private void Start()
    {
        col = gameObject.GetComponent<BoxCollider>();
      //  Transform[] temp = gameObject.GetComponentsInChildren<Transform>();
        //this.SpawnPoints = temp;
      //  this.SpawnPoints = new Transform[temp.Length - 1];
      //  for(int i = 0; i < this.SpawnPoints.Length; ++i)
      //  {
      //      this.SpawnPoints[i] = temp[i + 1];
      //  }

    }
    private void SpawnNinja(Vector3 psition)
    {
        Debug.Log("spawning");
        GameObject a = Instantiate(Robot) as GameObject;
        Debug.Log(psition);
        psition.x = psition.x - 2.1f;
        psition.z = psition.z - 1.12f;
        Robot_AI_Ctrl rob = a.gameObject.GetComponent<Robot_AI_Ctrl>();
        rob.player = this.player;
        rob.patrol_points = this.patrol_points;
        a.transform.position = psition;
        a.transform.localPosition = psition;
        a.transform.parent = null;
        Debug.Log(a.transform);
        a.SetActive(true);


    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Player Character"))
        {
            Debug.Log("Player enter");
            foreach (Transform t in this.SpawnPoints)
            {
                SpawnNinja(t.position);
            }
            col.enabled = false;
        }
    }
}
