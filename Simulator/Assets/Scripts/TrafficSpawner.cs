using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject CarPrefab; //The car prefab that will be instantiated
    public GameObject TrainPrefab; //The train prefab that will be instantiated
    public GameObject BoatPrefab; //The boat prefab that will be instantiated
    public GameObject PedestrianPrefab; //The pedestrian prefab that will be instantiated
    public GameObject CyclistPrefab; //The cyclist prefab that will be instantiated

    [Header("SpawnTimes")]
    public float CarSpawntimeInSec = 1f; //Time is takes for a car to spawn
    public float TrainSpawntimeInSec = 12f; //Time is takes for a train to spawn
    public float BoatSpawntimeInSec = 4f; //Time is takes for a boat to spawn
    public float PedestrianSpawntimeInSec = 4f; //Time is takes for a pedestrian to spawn
    public float CyclistSpawntimeInSec = 5f; //Time is takes for a cyclist to spawn

    [Header("Spawners")]
    public Spawner[] CarSpawners; //List of spawners
    public Spawner[] TrainSpawners; //List of spawners
    public Spawner[] BoatSpawners; //List of spawners
    public Spawner[] PedestrianSpawners; //List of spawners
    public Spawner[] CyclistSpawners; //List of spawners

    [Header("Routes")]
    public RoutesList[] CarRoutes; //List of possible routes
    public RoutesList[] TrainRoutes; //List of possible routes
    public RoutesList[] BoatRoutes; //List of possible routes
    public RoutesList[] PedestrianRoutes; //List of possible routes
    public RoutesList[] CyclistRoutes; //List of possible routes

    [Header("Parents")]
    public GameObject CarParent;
    public GameObject TrainParent;
    public GameObject BoatParent;
    public GameObject PedestrianParent;
    public GameObject CyclistParent;

    private float carTimer = 0f;
    private float trainTimer = 0f;
    private float boatTimer = 0f;
    private float pedestrianTimer = 0f;
    private float cyclistTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        if(carTimer >= CarSpawntimeInSec)
        {
            SpawnVehicle(TrafficType.Car);
            carTimer -= CarSpawntimeInSec;
        }
        if (trainTimer >= TrainSpawntimeInSec)
        {
            if(GameObject.FindGameObjectsWithTag("Train").Length == 0)
            {
                SpawnVehicle(TrafficType.Train);
            }
            trainTimer -= TrainSpawntimeInSec;
        }
        if (boatTimer >= BoatSpawntimeInSec)
        {
            SpawnVehicle(TrafficType.Boat);
            boatTimer -= BoatSpawntimeInSec;
        }
        if (pedestrianTimer >= PedestrianSpawntimeInSec)
        {
            SpawnVehicle(TrafficType.Pedestrian);
            pedestrianTimer -= PedestrianSpawntimeInSec;
        }
        if (cyclistTimer >= CyclistSpawntimeInSec)
        {
            SpawnVehicle(TrafficType.Bicycle);
            cyclistTimer -= CyclistSpawntimeInSec;
        }

        //Timer tick
        carTimer += Time.deltaTime;
        trainTimer += Time.deltaTime;
        boatTimer += Time.deltaTime;
        pedestrianTimer += Time.deltaTime;
        cyclistTimer += Time.deltaTime;
    }

    void SpawnVehicle(TrafficType type)
    {
        int randomIndex = -1;
        Spawner randomSpawner = null;
        WaypointMovementController spawnedVehicle = null;

        switch (type)
        {
            case TrafficType.Car:
                //Spawn car at random Spawner position 
                randomIndex = UnityEngine.Random.Range(0, CarSpawners.Length);
                randomSpawner = CarSpawners[randomIndex];
                spawnedVehicle = Instantiate(CarPrefab, randomSpawner.Object.transform.position, Quaternion.identity).GetComponent<WaypointMovementController>();
                spawnedVehicle.transform.parent = CarParent.transform;
                spawnedVehicle.spawnLocation = randomSpawner.Location;
                break;

            case TrafficType.Train:
                //Spawn train at random Spawner position 
                randomIndex = UnityEngine.Random.Range(0, TrainSpawners.Length);
                randomSpawner = TrainSpawners[randomIndex];
                spawnedVehicle = Instantiate(TrainPrefab, randomSpawner.Object.transform.position, Quaternion.identity).GetComponent<WaypointMovementController>();
                spawnedVehicle.transform.parent = TrainParent.transform;
                spawnedVehicle.spawnLocation = randomSpawner.Location;
                break;

            case TrafficType.Boat:
                //Spawn boat at random Spawner position 
                randomIndex = UnityEngine.Random.Range(0, BoatSpawners.Length);
                randomSpawner = BoatSpawners[randomIndex];
                spawnedVehicle = Instantiate(BoatPrefab, randomSpawner.Object.transform.position, Quaternion.identity).GetComponent<WaypointMovementController>();
                spawnedVehicle.transform.parent = BoatParent.transform;
                spawnedVehicle.spawnLocation = randomSpawner.Location;
                break;

            case TrafficType.Bicycle:
                //Spawn cyclist at random Spawner position 
                randomIndex = UnityEngine.Random.Range(0, CyclistSpawners.Length);
                randomSpawner = CyclistSpawners[randomIndex];
                spawnedVehicle = Instantiate(CyclistPrefab, randomSpawner.Object.transform.position, Quaternion.identity).GetComponent<WaypointMovementController>();
                spawnedVehicle.transform.parent = CyclistParent.transform;
                spawnedVehicle.spawnLocation = randomSpawner.Location;
                break;

            case TrafficType.Pedestrian:
                //Spawn pedestrian at random Spawner position 
                randomIndex = UnityEngine.Random.Range(0, PedestrianSpawners.Length);
                randomSpawner = PedestrianSpawners[randomIndex];
                spawnedVehicle = Instantiate(PedestrianPrefab, randomSpawner.Object.transform.position, Quaternion.identity).GetComponent<WaypointMovementController>();
                spawnedVehicle.transform.parent = PedestrianParent.transform;
                spawnedVehicle.spawnLocation = randomSpawner.Location;
                break;
        }
    }
}

