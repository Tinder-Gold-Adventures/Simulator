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
    private Bridge bridge; //Contains bridge object

    public delegate void Publish(TopicInformation topic, string message);
    public static event Publish OnPublishToController;

    // Start is called before the first frame update
    void Start()
    {
        trafficlights = FindObjectsOfType<Trafficlight>().ToList();
        warninglights = FindObjectsOfType<WarningLight>().ToList();
        barriers = FindObjectsOfType<Barrier>().ToList();
        sensors = FindObjectsOfType<Sensor>().ToList();
        bridge = FindObjectOfType<Bridge>();
        foreach (var sensor in sensors)
        {
            sensor.OnSensorTriggered += OnSensorTriggered;
        }
    }

    void Update()
    {
        UpdateBridgePosition();
    }

    //Checks if all barriers are closed, if so -> close bridge. If 1 barrier opens -> open bridge back up
    private void UpdateBridgePosition()
    {
        List<Barrier> vesselBarriers = barriers.FindAll(b => b.laneType == LaneTypes.vessel);
        List<Barrier> closedVesselBarriers = vesselBarriers.Where(b => b.state == BarrierState.Closed).ToList();

        if (bridge.state == BarrierState.Closed)
        {
            if (vesselBarriers.Count == closedVesselBarriers.Count)
            {
                bridge.IsChangingStates = true;
            }
        }
        else
        {
            if (vesselBarriers.Count != closedVesselBarriers.Count)
            {
                bridge.IsChangingStates = true;
            }
        }
    }

    private void SetTrafficLightState(LaneTypes lanetype, int groupID, int subgroupId, int componentID, TrafficLightState newState)
    {
        //If traffic light has no subgroup it will be -1 by default, so this function works
        Trafficlight trafficlight = trafficlights.Find(l => l.laneType == lanetype && l.GroupID == groupID && l.SubgroupID == subgroupId && l.ComponentID == componentID);

        if (trafficlight == null)
        {
            Debug.LogError($"ERROR: CarTrafficlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        trafficlight.state = newState;
    }

    private void SetWarningLightState(LaneTypes lanetype, int groupID, int subgroupId, int componentID, WarningLightState newState)
    {
        //If warning light has no subgroup it will be -1 by default, so this function works
        WarningLight warninglight = warninglights.Find(l => l.laneType == lanetype && l.GroupID == groupID && l.SubgroupID == subgroupId && l.ComponentID == componentID);

        if (warninglight == null)
        {
            Debug.LogError($"ERROR: CarTrafficlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        warninglight.state = newState;
    }

    private void SetBarrierState(LaneTypes lanetype, int groupID, int subgroupId, int componentID, BarrierState newState)
    {
        //If traffic light has no subgroup it will be -1 by default, so this function works
        Barrier barrier = barriers.Find(b => b.laneType == lanetype && b.GroupIds.Contains(groupID) && b.SubgroupID == subgroupId && b.ComponentID == componentID);

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
                    SetTrafficLightState(info.laneType, info.groupID, info.subGroupID, info.componentID, newState);
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
                    SetWarningLightState(info.laneType, info.groupID, info.subGroupID, info.componentID, newState);
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
                    SetBarrierState(info.laneType, info.groupID, info.subGroupID, info.componentID, newState);
                }
                catch
                {
                    Debug.LogError($"ERROR: Tried setting invalid barrier state: '{message}' for barrier with GroupID: {info.groupID} and ComponentID: {info.componentID}");
                }
                break;
        }
    }

    private TopicInformation ParseTopic(string topic)
    {
        string[] topicInfo = topic.Split('/');
       
        return new TopicInformation(topicInfo); //The constructor knows how to handle whether a topic includes a subgroup or not
    }
}
