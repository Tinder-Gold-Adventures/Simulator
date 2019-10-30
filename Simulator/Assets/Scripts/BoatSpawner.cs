using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSpawner : MonoBehaviour
{
    public GameObject BoatPrefab; //The boat prefab that will be instantiated
    public float SpawntimeInSec = 25f; //Time is takes for a boat to spawn
    public Spawner[] Spawners; //List of spawners
    public RoutesList[] Routes; //List of possible routes
    public GameObject BoatParent;

    private float timePassed = 0f;

    // Update is called once per frame
    void Update()
    {
        if (timePassed >= SpawntimeInSec)
        {
            //Spawn car at random Spawner position 
            int randomIndex = UnityEngine.Random.Range(0, Spawners.Length);
            Spawner randomSpawner = Spawners[randomIndex];
            BoatController spawnedBoat = Instantiate(BoatPrefab, randomSpawner.Object.transform.position, Quaternion.identity).GetComponent<BoatController>();
            spawnedBoat.transform.parent = BoatParent.transform;
            spawnedBoat.spawnLocation = randomSpawner.Location;

            //Reset timer
            timePassed -= SpawntimeInSec;
        }

        //Timer tick
        timePassed += Time.deltaTime;
    }
}
