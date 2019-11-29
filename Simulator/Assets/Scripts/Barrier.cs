using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrafficController;

public class Barrier : MonoBehaviour
{
    //Component Information
    public LaneTypes laneType;
    public int GroupId;
    public int ComponentID;
    [HideInInspector]
    public ComponentTypes componentType = ComponentTypes.barrier;
    public BarrierState state = BarrierState.Open;

    //Public settable fields
    public float RotationSpeed = 30f;

    //Private behavoir information
    private const float barrierOpenRotation = 90f;
    private const float barrierClosedRotation = 0f;
    [HideInInspector]
    private Trafficlight_Barrier barrier;

    void Start()
    {
        barrier = gameObject.GetComponent<Trafficlight_Barrier>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBarrierStatus();

        barrier.IsActive = state == BarrierState.Open ? false : true;
    }

    public void UpdateBarrierStatus()
    {        
        Quaternion objRotation = transform.rotation;        
        Quaternion targetRotation = Quaternion.Euler(objRotation.eulerAngles.x, objRotation.eulerAngles.y, 0f + (state == BarrierState.Open ? barrierOpenRotation : barrierClosedRotation));
        
        if(objRotation != targetRotation)
        {
            transform.rotation = Quaternion.RotateTowards(objRotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }
}
