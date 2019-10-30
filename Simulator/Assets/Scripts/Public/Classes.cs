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
    public int subGroupID;
    public ComponentTypes componentType;
    public int componentID;

    public TopicInformation(LaneTypes lanetype, int groupid, int subgroupid, ComponentTypes componenttype, int componentid)
    {
        laneType = lanetype;
        groupID = groupid;
        subGroupID = subgroupid;
        componentType = componenttype;
        componentID = componentid;
    }

    public TopicInformation(string[] topicInformation)
    {
        if (topicInformation.Length == 5) //Without subgroup ("team_id/lane_type/group_id/component_type/component_id")
        {
            laneType = (LaneTypes)System.Enum.Parse(typeof(LaneTypes), topicInformation[1]);
            groupID = int.Parse(topicInformation[2]);
            subGroupID = -1;
            componentType = (ComponentTypes)System.Enum.Parse(typeof(ComponentTypes), topicInformation[3]);
            componentID = int.Parse(topicInformation[4]);
        }
        else if (topicInformation.Length == 6)//With subgroup ("team_id/lane_type/group_id/subgroup_id/component_type/component_id")
        {
            laneType = (LaneTypes)System.Enum.Parse(typeof(LaneTypes), topicInformation[1]);
            groupID = int.Parse(topicInformation[2]);
            subGroupID = int.Parse(topicInformation[3]);
            componentType = (ComponentTypes)System.Enum.Parse(typeof(ComponentTypes), topicInformation[4]);
            componentID = int.Parse(topicInformation[5]);
        }
    }
}
