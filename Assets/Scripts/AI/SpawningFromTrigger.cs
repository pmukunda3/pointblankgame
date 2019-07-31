﻿using System.Collections;
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
        this.SpawnPoints = temp;
        //this.SpawnPoints = new Transform[temp.Length - 1];
        //for(int i = 0; i < this.SpawnPoints.Length; ++i)
        //{
        //    this.SpawnPoints[i] = temp[i + 1];
        //}

    }
    private void SpawnNinja(Transform t)
    {
        Debug.Log("spawning");
        GameObject a = Instantiate(ninjaPrefab,t) as GameObject;
        //a.transform.position = position;  
        //a.transform.localPosition = position;

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Player Character"))
        {
            Debug.Log("Player enter");
            foreach(Transform t in this.SpawnPoints)
            {
                for (int i = 0; i < 5; i++)
                {
                    SpawnNinja(t);
                }
            }
            col.enabled = false;
        }
    }
}
