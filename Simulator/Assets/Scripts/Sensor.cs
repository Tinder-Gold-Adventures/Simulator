using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrafficController;

public class Sensor : MonoBehaviour
{
    public LaneTypes LaneType;
    public int GroupID;
    public int SubgroupID = -1;
    public int ComponentID;

    [HideInInspector]
    public bool IsTriggered = false;
    private int vehicleCount = 0;

    public delegate void TriggeredSensor(LaneTypes laneType, int groupId, int subgroupId, int componentId, bool isTriggered);
    public event TriggeredSensor OnSensorTriggered;

    private void OnTriggerEnter(Collider other)
    {
        switch (LaneType)
        {
            case LaneTypes.motorised:
                if (other.gameObject.tag == "Car")
                    vehicleCount++;
                    break;

            case LaneTypes.cycle:
                if (other.gameObject.tag == "Cyclist")
                    //if (other.GetComponent<WaypointMovementController>().isInFrontOfRedLight) //Only count if target is waiting for red light (so it doesn't trigger when other cyclist drive over sensor
                    vehicleCount++;
                break;

            case LaneTypes.foot:
                if (other.gameObject.tag == "Pedestrian")
                    //if (other.GetComponent<WaypointMovementController>().isInFrontOfRedLight) //Only count if target is waiting for red light (so it doesn't trigger when other pedestrians walk over sensor
                    vehicleCount++;
                break;

            case LaneTypes.vessel:
                if (other.gameObject.tag == "Boat")
                    vehicleCount++;
                    break;

            case LaneTypes.track:
                if (other.gameObject.tag == "Train")
                    vehicleCount++;
                    break;
        }
        
        if (!IsTriggered && vehicleCount > 0)
        {
            IsTriggered = true;
            OnSensorTriggered(LaneType, GroupID, SubgroupID, ComponentID, IsTriggered);
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        switch (LaneType)
        {
            case LaneTypes.motorised:
                if (other.gameObject.tag == "Car")
                    vehicleCount--;
                    break;

            case LaneTypes.cycle:
                if (other.gameObject.tag == "Cyclist")
                    vehicleCount--;
                break;

            case LaneTypes.foot:
                if (other.gameObject.tag == "Pedestrian")
                    vehicleCount--;
                break;

            case LaneTypes.vessel:
                if (other.gameObject.tag == "Boat")
                    vehicleCount--;
                    break;

            case LaneTypes.track:
                if (other.gameObject.tag == "Train")
                    vehicleCount--;
                    break;
        }

        if (IsTriggered && vehicleCount == 0)
        {
            IsTriggered = false;
            OnSensorTriggered(LaneType, GroupID, SubgroupID, ComponentID, IsTriggered);
        }
    }
}
