using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public float Speed = 10f;
    public float RotationSpeed = 5f;
    public float MinDistance = .5f;

    [HideInInspector]
    //Route & spawn information
    public Location spawnLocation;
    private RoutesList route;
    private int waypointIndex = 0;
    private GameObject nextWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        //Find all possible routes from current spawn location
        List<RoutesList> possibleRoutes = FindObjectOfType<TrainSpawner>().Routes.Where(r => r.From == spawnLocation).ToList();
        if (possibleRoutes != null)
        {
            var index = UnityEngine.Random.Range(0, possibleRoutes.Count);
            route = possibleRoutes[index];
        }

        //Set first waypoint
        nextWaypoint = route.Route[waypointIndex];

        //Set correct rotation facing the target location
        Quaternion targetRotation = Quaternion.LookRotation(nextWaypoint.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        if (nextWaypoint == null)
        {
            //Got to the final waypoint, destroy self
            Destroy(gameObject);
            return;
        }

        float distanceToWaypoint = Vector3.Distance(transform.position, nextWaypoint.transform.position);
        if (distanceToWaypoint > MinDistance)
        {
            //Move to next waypoint if not yet there
            MoveToNextWaypoint();
        }
        else
        {
            waypointIndex++;
            if (waypointIndex >= route.Route.Length)
            {
                //Reached destination
                nextWaypoint = null;
                return;
            }

            //Set next waypoint
            nextWaypoint = route.Route[waypointIndex];
        }
    }

    //Calculates destination and slowly moves & rotates towards that destination
    private void MoveToNextWaypoint()
    {
        Quaternion targetRotation = Quaternion.LookRotation(nextWaypoint.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    //Fires when colliding with another collider
    private void OnTriggerEnter(Collider other)
    {
       
    }

    //Fires when no longer colliding with another collider
    private void OnTriggerExit(Collider other)
    {

    }
}
