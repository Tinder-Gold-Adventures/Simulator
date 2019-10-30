using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float DriveSpeed = 1f;
    public float RotationSpeed = 1f;
    public float MinDistance = .8f;

    [HideInInspector]
    //Route & spawn information
    public Location spawnLocation;
    private RoutesList route;
    private int waypointIndex = 0;
    private GameObject nextWaypoint;

    //Behavior information
    private bool isInFrontOfRedLight = false;
    private Trafficlight_Barrier trafficLightBarrier; //Traffic light barrier that boat is waiting for
    private bool isBehindOtherBoat = false;
    [HideInInspector]
    public BoatController otherBoat; //Boat that is in front of us
    private bool hasPriority = false;

    // Start is called before the first frame update
    void Start()
    {
        //Find all possible routes from current spawn location
        List<RoutesList> possibleRoutes = FindObjectOfType<BoatSpawner>().Routes.Where(r => r.From == spawnLocation).ToList();
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

        transform.position += transform.forward * DriveSpeed * Time.deltaTime;
    }

    //Returns true if not waiting for red light and not in front of another boat
    private bool PathIsClear()
    {
        isInFrontOfRedLight = TrafficLightIsRed();
        if (trafficLightBarrier != null && !isInFrontOfRedLight)
        {
            //No longer waiting for red light
            trafficLightBarrier = null;
        }

        return !isInFrontOfRedLight && (!isBehindOtherBoat || hasPriority);
    }

    //Fires when colliding with another collider
    private void OnTriggerEnter(Collider other)
    {
        //Traffic light in-sight is Red, don't proceed
        if (other.transform.tag == "Trafficlight_Barrier")
        {
            Trafficlight_Barrier barrier = other.GetComponent<Trafficlight_Barrier>();
            if (barrier.IsActive)
            {
                isInFrontOfRedLight = true;
                trafficLightBarrier = barrier;
            }
        }

        //In front of other (waiting) boat
        if (other.transform.tag == "Boat_Model")
        {
            isBehindOtherBoat = true;
            otherBoat = other.GetComponentInParent<BoatController>();
            
            //If the other boat also collides with this boat, give this one priority to move first
            if(otherBoat.otherBoat == this)
            {
                if (!otherBoat.hasPriority)
                {
                    hasPriority = true;
                }
            }
        }
    }

    //Fires when no longer colliding with another collider
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Boat_Model")
        {
            isBehindOtherBoat = false;
            otherBoat = null;
            hasPriority = false;
        }
    }

    private bool TrafficLightIsRed()
    {
        if (trafficLightBarrier == null)
        {
            return false;
        }

        return trafficLightBarrier.IsActive;
    }
}
