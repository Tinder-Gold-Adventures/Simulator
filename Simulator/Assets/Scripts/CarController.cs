using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float DriveSpeed = 5f;
    public float RotationSpeed = 10f;
    public float MinDistance = 2f;

    [HideInInspector]
    public Location spawnLocation;
    private RoutesList route;
    private int waypointIndex = 0;
    private GameObject nextWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        //Find all possible routes from current spawn location
        List<RoutesList> possibleRoutes = FindObjectOfType<CarSpawner>().Routes.Where(r => r.From == spawnLocation).ToList();
        if(possibleRoutes != null)
        {
            route = possibleRoutes[Random.Range(0, possibleRoutes.Count)];
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
        float distanceToWaypoint = Vector3.Distance(transform.position, nextWaypoint.transform.position);

        if(distanceToWaypoint > MinDistance)
        {
            //Move to next waypoint if not yet there
            MoveToNextWaypoint();
        }
        else
        {
            waypointIndex++;
            if(waypointIndex < route.Route.Length)
            {
                //Get next waypoint
                nextWaypoint = route.Route[waypointIndex];
            }
            else
            {
                //Got to the final waypoint

            }
        }
    }

    private void MoveToNextWaypoint()
    {
        Quaternion targetRotation = Quaternion.LookRotation(nextWaypoint.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        transform.position += transform.forward * DriveSpeed * Time.deltaTime;
    }
}
