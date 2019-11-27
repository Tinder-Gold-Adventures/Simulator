using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatLight : MonoBehaviour
{
    //Component Information
    public LaneTypes laneType;
    public int GroupID;
    public int ComponentID;
    [HideInInspector]
    public ComponentTypes componentType = ComponentTypes.traffic_light;
    public BoatLightState state = BoatLightState.Red;

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

                if (state == BoatLightState.Red) { state = BoatLightState.Green; }
                else if (state == BoatLightState.Green) { state = BoatLightState.Red; }
            }

            //Set new random interval
            timeTillChange = Random.Range(4f, 18f);
        }

        bool barrierActive = barrier.IsActive;
        if (state == BoatLightState.Red && !barrierActive)
        {
            //Light is red, activate the barrier
            barrier.IsActive = true;
        }
        else if (state == BoatLightState.Green && barrierActive)
        {
            //Light is green, deactivate the barrier
            barrier.IsActive = false;
        }
    }

    private void OnDrawGizmos()
    {
        switch (state)
        {
            case BoatLightState.Red:
                Gizmos.color = Color.red;
                break;
            case BoatLightState.Green:
                Gizmos.color = Color.green;
                break;
        }

        Gizmos.DrawCube(transform.position + new Vector3(0, 9, 0), new Vector3(2f, 2f, 2f));
    }
}