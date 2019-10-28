using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
    #region Singleton implementation
    private static TrafficController _instance;
    public static TrafficController Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private List<Trafficlight> trafficlights; //Contains all traffic lights in game
    private List<WarningLight> warninglights; //Contains all warning lights in game
    private List<Barrier> barriers; //Contains all barriers in game
    private List<Sensor> sensors; //Contains all sensors in game

    public delegate void Publish(TopicInformation topic, string message);
    public static event Publish OnPublishToController;

    // Start is called before the first frame update
    void Start()
    {
        trafficlights = FindObjectsOfType<Trafficlight>().ToList();
        warninglights = FindObjectsOfType<WarningLight>().ToList();
        barriers = FindObjectsOfType<Barrier>().ToList();
        sensors = FindObjectsOfType<Sensor>().ToList();
        foreach (var sensor in sensors)
        {
            sensor.OnSensorTriggered += OnSensorTriggered;
        }
    }

    //
    private void SetTrafficLightState(int groupID, int subgroupId, int componentID, TrafficLightState newState)
    {
        //If traffic light has no subgroup it will be -1 by default, so this function works
        Trafficlight trafficlight = trafficlights.Find(l => l.GroupID == groupID && l.SubgroupID == subgroupId && l.ComponentID == componentID);

        if (trafficlight == null)
        {
            Debug.LogError($"ERROR: CarTrafficlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        trafficlight.state = newState;
    }

    private void SetWarningLightState(int groupID, int subgroupId, int componentID, WarningLightState newState)
    {
        //If warning light has no subgroup it will be -1 by default, so this function works
        WarningLight warninglight = warninglights.Find(l => l.GroupID == groupID && l.SubgroupID == subgroupId && l.ComponentID == componentID);

        if (warninglight == null)
        {
            Debug.LogError($"ERROR: CarTrafficlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        warninglight.state = newState;
    }

    private void SetBarrierState(int groupID, int subgroupId, int componentID, BarrierState newState)
    {
        //If traffic light has no subgroup it will be -1 by default, so this function works
        Barrier barrier = barriers.Find(l => l.GroupIds.Contains(groupID) && l.SubgroupID == subgroupId && l.ComponentID == componentID);

        if (barrier == null)
        {
            Debug.LogError($"ERROR: CarTrafficlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        if(newState != barrier.state)
        {
            barrier.IsChangingStates = true;
        }
    }

    //Event listener to sensor trigger
    private void OnSensorTriggered(LaneTypes laneType, int groupId, int subgroupId, int componentId, bool isTriggered)
    {
        TopicInformation info = new TopicInformation(laneType, groupId, subgroupId, ComponentTypes.sensor, componentId);
        PublishToController(info, (isTriggered ? 1 : 0).ToString());
    }

    //Invokes 'publish to controller' event
    private void PublishToController(TopicInformation topic, string message)
    {
        OnPublishToController.Invoke(topic, message);
    }

    public void HandleMessage(string topic, string message)
    {
        //Parse and store topic information
        TopicInformation info = ParseTopic(topic);

        switch (info.componentType)
        {
            case ComponentTypes.traffic_light:
                //Handle traffic lights
                //Try setting new traffic light state
                try
                {
                    TrafficLightState newState = (TrafficLightState)System.Enum.Parse(typeof(TrafficLightState), message);
                    SetTrafficLightState(info.groupID, info.subGroupID, info.componentID, newState);
                }
                catch
                {
                    Debug.LogError($"ERROR: Tried setting invalid traffic light state: '{message}' for traffic light with GroupID: {info.groupID} and ComponentID: {info.componentID}");
                }
                break;

            case ComponentTypes.warning_light:
                //Handle warning lights
                //Try setting new warning light state
                try
                {
                    WarningLightState newState = (WarningLightState)System.Enum.Parse(typeof(WarningLightState), message);
                    SetWarningLightState(info.groupID, info.subGroupID, info.componentID, newState);
                }
                catch
                {
                    Debug.LogError($"ERROR: Tried setting invalid warning light state: '{message}' for warning light with GroupID: {info.groupID} and ComponentID: {info.componentID}");
                }
                break;

            case ComponentTypes.sensor: //Don't have to handle sensor data
                break;

            case ComponentTypes.barrier:
                //Handle barriers
                //Try setting new barrier state
                try
                {
                    BarrierState newState = (BarrierState)System.Enum.Parse(typeof(BarrierState), message);
                    SetBarrierState(info.groupID, info.subGroupID, info.componentID, newState);
                }
                catch
                {
                    Debug.LogError($"ERROR: Tried setting invalid barrier state: '{message}' for barrier with GroupID: {info.groupID} and ComponentID: {info.componentID}");
                }
                break;
        }
    }

    #region Parse Topic Logic
    private TopicInformation ParseTopic(string topic)
    {
        string[] topicInfo = topic.Split('/');
       
        return new TopicInformation(topicInfo); //The constructor knows how to handle whether a topic includes a subgroup or not
    }

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
            else if(topicInformation.Length == 6)//With subgroup ("team_id/lane_type/group_id/subgroup_id/component_type/component_id")
            {
                laneType = (LaneTypes)System.Enum.Parse(typeof(LaneTypes), topicInformation[1]);
                groupID = int.Parse(topicInformation[2]);
                subGroupID = int.Parse(topicInformation[3]);
                componentType = (ComponentTypes)System.Enum.Parse(typeof(ComponentTypes), topicInformation[4]);
                componentID = int.Parse(topicInformation[5]);
            }
        }
    }
    #endregion

    #region Public Enums
    public enum LaneTypes
    {
        motorised,
        cycle,
        foot,
        vessel,
        track
    }

    public enum ComponentTypes
    {
        traffic_light,
        warning_light,
        sensor,
        barrier
    }
    #endregion
}
