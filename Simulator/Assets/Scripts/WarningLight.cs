using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrafficController;

public class WarningLight : MonoBehaviour
{
    //Component Information
    public LaneTypes laneType;
    public int GroupID;
    public int SubgroupID = -1;
    public int ComponentID;
    [HideInInspector]
    public ComponentTypes componentType = ComponentTypes.warning_light;
    public WarningLightState state = WarningLightState.Off;

    //private behavior information
    private Trafficlight_Barrier barrier;

    //Optional, only for testing purposes
    private float timeTillChange = 5f;
    private float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        barrier = GetComponentInChildren<Trafficlight_Barrier>();
        if(barrier == null)
        {
            Debug.LogError("ERROR: No trafficlight_barrier found in warninglight object");
        }

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

                if (state == WarningLightState.On) { state = WarningLightState.Off; }
                else if (state == WarningLightState.Off) { state = WarningLightState.On; }
            }

            //Set new random interval
            timeTillChange = Random.Range(4f, 18f);
        }

        bool barrierActive = barrier.IsActive;
        if (state == WarningLightState.On && !barrierActive)
        {
            //Light is red, activate the barrier
            barrier.IsActive = true;
        }
        else if (state == WarningLightState.Off && barrierActive)
        {
            //Light is green, deactivate the barrier
            barrier.IsActive = false;
        }
    }

    private void OnDrawGizmos()
    {
        switch (state)
        {
            case WarningLightState.On:
                Gizmos.color = Color.red;
                break;
            case WarningLightState.Off:
                Gizmos.color = Color.green;
                break;
        }

        Gizmos.DrawCube(transform.position + new Vector3(0, 9, 0), new Vector3(2f, 2f, 2f));
    }
}
