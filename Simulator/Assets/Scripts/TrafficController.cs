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
    private List<Sensor> sensors; //Contains all sensors in game

    public delegate void Publish(TopicInformation topic, string message);
    public static event Publish OnPublishToController;

    // Start is called before the first frame update
    void Start()
    {
        trafficlights = FindObjectsOfType<Trafficlight>().ToList();
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

    //Event listener to sensor trigger
    private void OnSensorTriggered(LaneTypes laneType, int groupId, int subgroupId, int componentId, bool isTriggered)
    {
        TopicInformation info = new TopicInformation(laneType, groupId, subgroupId, ComponentTypes.Sensor, componentId);
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

        if (info.componentType == ComponentTypes.TrafficLight)
        {
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
            else //With subgroup ("team_id/lane_type/group_id/subgroup_id/component_type/component_id")
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
        Motorised,
        Cycle,
        Foot,
        Vessel,
        Track
    }

    public enum ComponentTypes
    {
        TrafficLight,
        WarningLight,
        Sensor,
        Barrier
    }
    #endregion
}
