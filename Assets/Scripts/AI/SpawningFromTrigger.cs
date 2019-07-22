using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningFromTrigger : MonoBehaviour
{
    public GameObject ninjaPrefab;
    public BoxCollider col;
    private Transform[] SpawnPoints;
    



    private void Start()
    {
        col = gameObject.GetComponent<BoxCollider>();
        Transform[] temp = gameObject.GetComponentsInChildren<Transform>();
        this.SpawnPoints = new Transform[temp.Length - 1];
        for(int i = 0; i < this.SpawnPoints.Length; ++i)
        {
            this.SpawnPoints[i] = temp[i + 1];
        }

    }
    private void SpawnNinja(Vector3 position)
    {
        GameObject a = Instantiate(ninjaPrefab) as GameObject;
        a.transform.position = position;

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Player Character"))
        {
            Debug.Log("Player enter");
            foreach(Transform t in this.SpawnPoints)
            {
                SpawnNinja(t.position);
            }
        }
    }
}
