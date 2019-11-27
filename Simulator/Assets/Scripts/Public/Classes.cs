using System;
using UnityEngine;

//Contains a route including from->to locations and a list of waypoints
[Serializable]
public class RoutesList
{
    public string Name;
    public Location From;
    public Location To;
    public GameObject[] Route;
}

//Contains a spawner object with a specified location
[Serializable]
public class Spawner
{
    public Location Location;
    public GameObject Object; //The actual spawner GameObject
}

//Contains information about a parsed topic. Can be constructed using a string array or by each field seperate
public class TopicInformation
{
    public LaneTypes laneType;
    public int groupID;
    public ComponentTypes componentType;
    public int componentID;

    public TopicInformation(LaneTypes lanetype, int groupid, ComponentTypes componenttype, int componentid)
    {
        laneType = lanetype;
        groupID = groupid;
        componentType = componenttype;
        componentID = componentid;
    }

    public TopicInformation(string[] topicInformation)
    {
        laneType = (LaneTypes)System.Enum.Parse(typeof(LaneTypes), topicInformation[1]);
        groupID = int.Parse(topicInformation[2]);
        componentType = (ComponentTypes)System.Enum.Parse(typeof(ComponentTypes), topicInformation[3]);
        componentID = int.Parse(topicInformation[4]);
    }
}
