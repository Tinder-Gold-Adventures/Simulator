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

    public delegate void TriggeredSensor(LaneTypes laneType, int groupId, int subgroupId, int componentId, bool isTriggered);
    public event TriggeredSensor OnSensorTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Car")
        {
            IsTriggered = true;
            OnSensorTriggered(LaneType, GroupID, SubgroupID, ComponentID, IsTriggered);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            IsTriggered = false;
            OnSensorTriggered(LaneType, GroupID, SubgroupID, ComponentID, IsTriggered);
        }
    }
}
