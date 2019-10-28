using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrafficController;

public class Trafficlight : MonoBehaviour
{
    //Component Information
    public LaneTypes laneType;
    public int GroupID;
    public int SubgroupID = -1;
    public int ComponentID;
    [HideInInspector]
    public ComponentTypes componentType = ComponentTypes.traffic_light;
    public TrafficLightState state = TrafficLightState.Red;

    //Private behavoir information
    private Trafficlight_Barrier barrier;

    //Optional, only for testing purposes
    private float timeTillChange = 5f;
    private float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Finds the barrier which is a child of this trafficlight object
        barrier = GetComponentInChildren<Trafficlight_Barrier>();

        timeTillChange = Random.Range(4f, 18f);
    }

    // Update is called once per frame
    void Update()
    {
        //Switch between states at random intervals if not listening to controller
        if (!M2QTTController.Instance.ListenToController)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= timeTillChange)
            {
                currentTime -= timeTillChange;

                if (state == TrafficLightState.Red) { state = TrafficLightState.Green; }
                else if (state == TrafficLightState.Green) { state = TrafficLightState.Red; }
            }

            //Set new random interval
            timeTillChange = Random.Range(4f, 18f);
        }

        bool barrierActive = barrier.IsActive;
        if(state == TrafficLightState.Red && !barrierActive)
        {            
            //Light is red, activate the barrier
            barrier.IsActive = true;
        }
        else if(state == TrafficLightState.Green && barrierActive)
        {
            //Light is green, deactivate the barrier
            barrier.IsActive = false;
        }
    }

    private void OnDrawGizmos()
    {
        switch (state)
        {
            case TrafficLightState.Red:
                Gizmos.color = Color.red;
                break;
            case TrafficLightState.Yellow:
                Gizmos.color = Color.yellow;
                break;
            case TrafficLightState.Green:
                Gizmos.color = Color.green;
                break;
        }

        Gizmos.DrawCube(transform.position + new Vector3(0, 9, 0), new Vector3(2f, 2f, 2f));
    }
}

#region Public Enums
public enum TrafficLightState
{
    Red = 0,
    Yellow = 1,
    Green = 2,
    Disabled = 3
}
public enum WarningLightState
{
    Off = 0,
    On = 1
}
public enum BarrierState
{
    Open = 0,
    Closed = 1
}
#endregion