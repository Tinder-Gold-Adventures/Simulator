using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainLight : MonoBehaviour
{
    //Component Information
    public LaneTypes laneType;
    public int GroupID;
    public int ComponentID;
    [HideInInspector]
    public ComponentTypes componentType = ComponentTypes.train_light;
    public BoatAndTrainLightState state = BoatAndTrainLightState.Red;

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

                if (state == BoatAndTrainLightState.Red) { state = BoatAndTrainLightState.Green; }
                else if (state == BoatAndTrainLightState.Green) { state = BoatAndTrainLightState.Red; }
            }

            //Set new random interval
            timeTillChange = Random.Range(4f, 18f);
        }

        bool barrierActive = barrier.IsActive;
        if (state == BoatAndTrainLightState.Red && !barrierActive)
        {
            //Light is red, activate the barrier
            barrier.IsActive = true;
        }
        else if (state == BoatAndTrainLightState.Green && barrierActive)
        {
            //Light is green, deactivate the barrier
            barrier.IsActive = false;
        }
    }

    private void OnDrawGizmos()
    {
        switch (state)
        {
            case BoatAndTrainLightState.Red:
                Gizmos.color = Color.red;
                break;
            case BoatAndTrainLightState.Green:
                Gizmos.color = Color.green;
                break;
        }

        Gizmos.DrawCube(transform.position + new Vector3(0, 9, 0), new Vector3(2f, 2f, 2f));
    }
}
