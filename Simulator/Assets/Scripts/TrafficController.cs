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

    //Checks if all barriers are closed, if so -> close bridge. If all barriers open -> open bridge back up
    private void UpdateBridgePosition()
    {
        List<Barrier> vesselBarriers = barriers.FindAll(b => b.laneType == LaneTypes.vessel);
        List<Barrier> closedVesselBarriers = vesselBarriers.Where(b => b.state == BarrierState.Closed).ToList();

        //Open bridge
        if (bridge.state == BarrierState.Closed)
        {
            if (vesselBarriers.Count == closedVesselBarriers.Count) //All vessel barriers are closed
            {
                bridge.IsChangingStates = true;
            }
        }
        else //Close bridge
        {
            List<Barrier> openVesselBarriers = vesselBarriers.Where(b => b.state == BarrierState.Open).ToList();

            if (vesselBarriers.Count == openVesselBarriers.Count) //All vessel barriers are open again
            {
                bridge.IsChangingStates = true;
            }
        }
    }

    private void SetTrafficLightState(LaneTypes lanetype, int groupID, int subgroupId, int componentID, TrafficLightState newState)
    {
        //Finds all traffic lights with certain properties
        //If traffic light has no subgroup it will be -1 by default, so this function works
        List<Trafficlight> trafficlightList = trafficlights.FindAll(l => l.laneType == lanetype && l.GroupID == groupID && l.SubgroupID == subgroupId && l.ComponentID == componentID);

        //No traffic light found
        if (trafficlightList == null)
        {
            Debug.LogError($"ERROR: CarTrafficlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        foreach (var light in trafficlightList)
        {
            if(newState != light.state)
            {
                light.state = newState;
            }
        }

    }

    private void SetWarningLightState(LaneTypes lanetype, int groupID, int subgroupId, int componentID, WarningLightState newState)
    {
        //Finds all warning lights with certain properties
        //If warning light has no subgroup it will be -1 by default, so this function works
        List<WarningLight> warninglightList = warninglights.FindAll(l => l.laneType == lanetype && l.GroupID == groupID && l.SubgroupID == subgroupId && l.ComponentID == componentID);

        //No warning light found
        if (warninglights == null)
        {
            Debug.LogError($"ERROR: CarTrafficlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        foreach (var light in warninglightList)
        {
            if(newState != light.state)
            {
                light.state = newState;
            }
        }
    }

    private void SetBarrierState(LaneTypes lanetype, int groupID, int subgroupId, int componentID, BarrierState newState)
    {
        //Finds all barriers with id
        //If barrier has no subgroup it will be -1 by default, so this function works
        List<Barrier> barrierList = barriers.FindAll(b => b.laneType == lanetype && b.GroupIds.Contains(groupID) && b.SubgroupID == subgroupId && b.ComponentID == componentID);

        //No barrier found
        if (barrierList == null)
        {
            Debug.LogError($"ERROR: CarTrafficlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        foreach (var barrier in barrierList)
        {
            if (newState != barrier.state)
            {
                barrier.IsChangingStates = true;
            }
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
