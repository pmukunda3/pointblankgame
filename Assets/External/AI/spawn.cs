using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn : MonoBehaviour
{

    public GameObject ninjaPrefab;

    public float respawnTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(spawnNinjaWave());

    }

    private void SpawnNinja()
    {
        GameObject a = Instantiate(ninjaPrefab) as GameObject;
        a.transform.position = this.gameObject.transform.position;
         
    }

    IEnumerator spawnNinjaWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);
            SpawnNinja();
        }



    }
}
