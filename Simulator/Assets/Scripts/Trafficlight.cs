using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trafficlight : MonoBehaviour
{
    public int GroupID;
    public int SubgroupID = -1;
    public int ComponentID;
    private GameObject barrier;
    public TrafficLightState state = TrafficLightState.Red;

    private float timeTillChange = 5f;
    private float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Finds the barrier which is a child of this trafficlight object
        barrier = new List<GameObject>(GameObject.FindGameObjectsWithTag("Trafficlight_Barrier")).Find(g => g.transform.IsChildOf(transform));

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

        bool barrierActive = barrier.activeSelf;
        if(state == TrafficLightState.Red && !barrierActive)
        {            
            //Light is red, activate the barrier
            barrier.SetActive(true);
        }
        else if(state == TrafficLightState.Green && barrierActive)
        {
            //Light is green, deactivate the barrier
            barrier.SetActive(false);
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
#endregion