using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject CarPrefab; //The car prefab that will be instantiated
    public float SpawntimeInSec = 1f; //Time is takes for a car to spawn
    public Spawner[] Spawners; //List of spawners
    public RoutesList[] Routes; //List of possible routes

    private float timePassed = 0f;

    // Update is called once per frame
    void Update()
    {
        if(timePassed >= SpawntimeInSec)
        {           
            //Spawn car at random Spawner position 
            int randomIndex = UnityEngine.Random.Range(0, Spawners.Length);
            Spawner randomSpawner = Spawners[randomIndex];
            CarController spawnedCar = Instantiate(CarPrefab, randomSpawner.Object.transform.position, Quaternion.identity).GetComponent<CarController>();
            spawnedCar.spawnLocation = randomSpawner.Location;

            //Reset timer
            timePassed -= SpawntimeInSec;
        }

        //Timer tick
        timePassed += Time.deltaTime;
    }
}

//A class that contains a route
[Serializable]
public class RoutesList
{
    public string Name;
    public Location From;
    public Location To;
    public GameObject[] Route;
}

//A class that contains a spawn object with a location
[Serializable]
public class Spawner
{
    public Location Location;
    public GameObject Object; //The actual spawner GameObject
}

//Possible spawn locations
public enum Location
{
    North,
    East,
    South,
    West
}
