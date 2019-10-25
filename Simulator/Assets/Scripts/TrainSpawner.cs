using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSpawner : MonoBehaviour
{
    public GameObject TrainPrefab; //The train prefab that will be instantiated
    public float SpawntimeInSec = 10f; //Time is takes for a train to spawn
    public Spawner[] Spawners; //List of spawners
    public RoutesList[] Routes; //List of possible routes
    public GameObject TrainParent;

    private float timePassed = 0f;

    // Update is called once per frame
    void Update()
    {
        if (timePassed >= SpawntimeInSec)
        {
            //Spawn car at random Spawner position 
            int randomIndex = UnityEngine.Random.Range(0, Spawners.Length);
            Spawner randomSpawner = Spawners[randomIndex];
            TrainController spawnedTrain = Instantiate(TrainPrefab, randomSpawner.Object.transform.position, Quaternion.identity).GetComponent<TrainController>();
            spawnedTrain.transform.parent = TrainParent.transform;
            spawnedTrain.spawnLocation = randomSpawner.Location;

            //Reset timer
            timePassed -= SpawntimeInSec;
        }

        //Timer tick
        timePassed += Time.deltaTime;
    }
}
