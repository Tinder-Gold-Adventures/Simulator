using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrafficController;

public class Barrier : MonoBehaviour
{
    //Component Information
    public LaneTypes laneType;
    public int GroupID;
    public int SubgroupID = -1;
    public int ComponentID;
    [HideInInspector]
    public ComponentTypes componentType = ComponentTypes.barrier;
    public BarrierState state = BarrierState.Open;

    //Public settable fields
    public float RotationSpeed = 1f;

    //Private behavoir information
    private const float barrierOpenRotation = 90f;
    private const float barrierClosedRotation = 0f;
    private float rotationProgress = 0f;
    [HideInInspector]
    public bool IsChangingStates = false;

    // Update is called once per frame
    void Update()
    {
        if (IsChangingStates)
        {
            UpdateBarrierStatus();
        }
    }

    public void UpdateBarrierStatus()
    {        
        if (rotationProgress <= 1f)
        {
            rotationProgress += RotationSpeed * Time.deltaTime;
        }

        Quaternion objRotation = transform.rotation;        

        Quaternion targetRotation = Quaternion.Euler(objRotation.eulerAngles.x, objRotation.eulerAngles.y, 0f + (state == BarrierState.Open ? barrierClosedRotation : barrierOpenRotation));
        transform.rotation = Quaternion.Lerp(objRotation, targetRotation, rotationProgress);        

        if(rotationProgress >= 1f)
        {
            IsChangingStates = false;
            state = state == BarrierState.Open ? BarrierState.Closed : BarrierState.Open;
            rotationProgress = 0f;
        }
    }

    //Changes barrier's state from Open->Closed or Closed->Open
    public void ChangeBarrierState()
    {
        IsChangingStates = true;
        rotationProgress = 0f;
    }
}
