using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trafficlight : MonoBehaviour
{
    public int ID;
    private GameObject barrier;
    public TrafficLightState state = TrafficLightState.Red;

    private float timeTillChange = 5f;
    private float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Finds the barrier which is a child of this trafficlight object
        barrier = new List<GameObject>(GameObject.FindGameObjectsWithTag("Trafficlight_Barrier")).Find(g => g.transform.IsChildOf(transform));

        timeTillChange = UnityEngine.Random.Range(4f, 9f);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime >= timeTillChange)
        {
            currentTime -= timeTillChange;

            if (state == TrafficLightState.Red) { state = TrafficLightState.Green; }
            else if (state == TrafficLightState.Green) { state = TrafficLightState.Red; }
        }

        bool barrierActive = barrier.activeSelf;
        if(state == TrafficLightState.Red && !barrierActive)
        {            
            //Activate the barrier
            barrier.SetActive(true);
        }
        else if(state == TrafficLightState.Green && barrierActive)
        {
            barrier.SetActive(false);
        }
    }
}

public enum TrafficLightState
{
    Red = 0,
    Yellow = 1,
    Green = 2,
    Disabled = 3
}
