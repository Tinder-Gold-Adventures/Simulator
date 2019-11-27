using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaypointMovementController : MonoBehaviour
{
    public TrafficType VehicleType;
    public float MovementSpeed = 5f;
    public float RotationSpeed = 10f;
    public float MinDistanceToWaypoint = 2f;
    public string[] CollidesWithTags;

    [HideInInspector]
    //Route & spawn information
    public Location spawnLocation;
    private RoutesList route;
    private int waypointIndex = 0;
    private GameObject nextWaypoint;

    //Behavior information
    public bool isInFrontOfRedLight = false;
    private Trafficlight_Barrier trafficLightBarrier; //Traffic light barrier that vehicle is waiting for
    private bool isBehindOtherVehicle = false;
    [HideInInspector]
    public WaypointMovementController otherVehicle; //Vehicle that is in front of us
    private bool hasPriority = false;
    private bool hasAnimations = false;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //Find all possible routes from current spawn location
        List<RoutesList> possibleRoutes = null;
        switch (VehicleType)
        {
            case TrafficType.Car:
                possibleRoutes = FindObjectOfType<TrafficSpawner>().CarRoutes.Where(r => r.From == spawnLocation).ToList();
                break;

            case TrafficType.Train:
                possibleRoutes = FindObjectOfType<TrafficSpawner>().TrainRoutes.Where(r => r.From == spawnLocation).ToList();
                break;

            case TrafficType.Boat:
                possibleRoutes = FindObjectOfType<TrafficSpawner>().BoatRoutes.Where(r => r.From == spawnLocation).ToList();
                break;

            case TrafficType.Bicycle:
                possibleRoutes = FindObjectOfType<TrafficSpawner>().CyclistRoutes.Where(r => r.From == spawnLocation).ToList();
                hasAnimations = true;
                animator = GetComponent<Animator>();
                break;

            case TrafficType.Pedestrian:
                possibleRoutes = FindObjectOfType<TrafficSpawner>().PedestrianRoutes.Where(r => r.From == spawnLocation).ToList();
                hasAnimations = true;
                animator = GetComponent<Animator>();
                break;
        }

        if(possibleRoutes != null)
        {
            var index = UnityEngine.Random.Range(0, possibleRoutes.Count - 1);
            route = possibleRoutes[index];
        }
        else
        {
            Debug.LogError($"ERROR: Failed to create route for vehicle type: {VehicleType.ToString()}");
            Destroy(this);
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
        if(distanceToWaypoint > MinDistanceToWaypoint)
        {
            //Check for obstacles
            if (PathIsClear()) 
            {
                //Move to next waypoint if not yet there
                MoveToNextWaypoint();
            }
        }
        else
        {
            waypointIndex++;
            if(waypointIndex >= route.Route.Length)
            {
                //Reached destination
                nextWaypoint = null;
                return; 
            }

            //Set next waypoint
            nextWaypoint = route.Route[waypointIndex];
        }

        if (hasAnimations)
        {
            animator.SetBool("waitingForRedLight", isInFrontOfRedLight);
        }
    }

    //Calculates destination and slowly moves & rotates towards that destination
    private void MoveToNextWaypoint()
    {
        Quaternion targetRotation = Quaternion.LookRotation(nextWaypoint.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        transform.position += transform.forward * MovementSpeed * Time.deltaTime;
    }

    //Returns true if not waiting for red light and not in front of another vehicle
    private bool PathIsClear()
    {
        isInFrontOfRedLight = TrafficLightIsRed();
        if (trafficLightBarrier != null && !isInFrontOfRedLight)
        {
            //No longer waiting for red light
            trafficLightBarrier = null;
        }

        return !isInFrontOfRedLight && (!isBehindOtherVehicle || hasPriority);
    }

    //Fires when colliding with another collider
    private void OnTriggerEnter(Collider other)
    {
        //Trains don't collide with anything
        if (VehicleType == TrafficType.Train)
            return;

        //Traffic light in-sight is Red, don't proceed
        if (other.transform.tag == "Trafficlight_Barrier")
        {
            float dotForward = Vector3.Dot(other.transform.forward, transform.forward);
            
            if(dotForward > -0.8f){
                return; //Barrier facing away too far from this unit, barrier probably not meant for this unit. Ignore it
            }

            Trafficlight_Barrier barrier = other.GetComponent<Trafficlight_Barrier>();
            if (barrier.IsActive)
            {
                isInFrontOfRedLight = true;
                trafficLightBarrier = barrier;
            }
        }

        //In front of other (waiting) vehicle
        if (CollidesWithTags.Contains(other.transform.tag))
        {
            isBehindOtherVehicle = true;
            otherVehicle = other.GetComponentInParent<WaypointMovementController>();

            //If the other vehicle also collides with this vehicle, give this one priority to move first
            if (otherVehicle.otherVehicle == this)
            {
                if (!otherVehicle.hasPriority)
                {
                    hasPriority = true;
                }
            }
        }
    }

    //Fires when no longer colliding with another collider
    private void OnTriggerExit(Collider other)
    {
        //Trains don't collide with anything
        if (VehicleType == TrafficType.Train)
            return;

        if (CollidesWithTags.Contains(other.transform.tag))
        {
            isBehindOtherVehicle = false;
            otherVehicle = null;
            hasPriority = false;
        }
    }

    private bool TrafficLightIsRed()
    {
        if(trafficLightBarrier == null)
        {
            return false;
        }

        return trafficLightBarrier.IsActive;
    }
}
