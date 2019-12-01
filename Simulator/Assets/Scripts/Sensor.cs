using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    //Component Information
    public LaneTypes LaneType;
    public int GroupID;
    public int ComponentID;

    [HideInInspector]
    public bool IsTriggered = false;
    private int vehicleCount = 0;

    //Event, subscribed to by TrafficController script
    public delegate void TriggeredSensor(LaneTypes laneType, int groupId, int componentId, bool isTriggered);
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
                    vehicleCount++;
                break;

            case LaneTypes.foot:
                if (other.gameObject.tag == "Pedestrian")
                    vehicleCount++;
                break;

            case LaneTypes.vessel:
                if(GroupID == 0 && ComponentID == 3) //Special sensor on top of bridge which should look for everything but boats
                {
                    if("Cyclist Pedestrian Car".Contains(other.gameObject.tag))
                    {
                        vehicleCount++;
                    }
                    break;
                }
                else if (other.gameObject.tag == "Boat")
                    vehicleCount++;
                    break;

            case LaneTypes.track:
                if (other.gameObject.tag == "Train")
                    vehicleCount++;
                    break;
        }
        
        //Sensor is activated by a vehicle
        if (!IsTriggered && vehicleCount > 0)
        {
            IsTriggered = true;
            OnSensorTriggered(LaneType, GroupID, ComponentID, IsTriggered);
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
                if (GroupID == 0 && ComponentID == 3) //Special sensor on top of bridge which should look for everything but boats
                {
                    if ("Cyclist Pedestrian Car".Contains(other.gameObject.tag))
                    {
                        vehicleCount--;
                    }
                    break;
                }
                else if (other.gameObject.tag == "Boat")
                    vehicleCount--;
                    break;

            case LaneTypes.track:
                if (other.gameObject.tag == "Train")
                    vehicleCount--;
                    break;
        }

        //Sensor no longer activated
        if (IsTriggered && vehicleCount == 0)
        {
            IsTriggered = false;
            OnSensorTriggered(LaneType, GroupID, ComponentID, IsTriggered);
        }
    }
}
