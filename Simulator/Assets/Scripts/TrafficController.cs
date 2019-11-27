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
    private List<BoatLight> boatlights; //Contains all boat lights in game
    private List<Barrier> barriers; //Contains all barriers in game
    private List<Sensor> sensors; //Contains all sensors in game
    private List<Bridge> bridges; //Contains all bridges in game

    public delegate void Publish(TopicInformation topic, string message);
    public static event Publish OnPublishToController;

    // Start is called before the first frame update
    void Start()
    {
        trafficlights = FindObjectsOfType<Trafficlight>().ToList();
        warninglights = FindObjectsOfType<WarningLight>().ToList();
        boatlights = FindObjectsOfType<BoatLight>().ToList();
        barriers = FindObjectsOfType<Barrier>().ToList();
        sensors = FindObjectsOfType<Sensor>().ToList();
        bridges = FindObjectsOfType<Bridge>().ToList();
        foreach (var sensor in sensors)
        {
            sensor.OnSensorTriggered += OnSensorTriggered;
        }
    }

    private void SetTrafficLightState(LaneTypes lanetype, int groupID, int componentID, TrafficLightState newState)
    {
        //Finds all traffic lights with certain properties
        List<Trafficlight> trafficlightList = trafficlights.FindAll(l => l.laneType == lanetype && l.GroupID == groupID && l.ComponentID == componentID);

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

    private void SetWarningLightState(LaneTypes lanetype, int groupID, int componentID, WarningLightState newState)
    {
        //Finds all warning lights with certain properties
        List<WarningLight> warninglightList = warninglights.FindAll(l => l.laneType == lanetype && l.GroupID == groupID && l.ComponentID == componentID);

        //No warning light found
        if (warninglightList == null)
        {
            Debug.LogError($"ERROR: Warninglight with groupID: {groupID} and componentID: {componentID} not found");
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

    private void SetBoatLightState(LaneTypes lanetype, int groupID, int componentID, BoatLightState newState)
    {
        //Finds all warning lights with certain properties
        List<BoatLight> boatlightList = boatlights.FindAll(l => l.laneType == lanetype && l.GroupID == groupID && l.ComponentID == componentID);

        //No warning light found
        if (boatlightList == null)
        {
            Debug.LogError($"ERROR: Boatlight with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        foreach (var light in boatlightList)
        {
            if (newState != light.state)
            {
                light.state = newState;
            }
        }
    }

    private void SetBarrierState(LaneTypes lanetype, int groupID, int componentID, BarrierState newState)
    {
        //Finds all barriers with id
        List<Barrier> barrierList = barriers.FindAll(b => b.laneType == lanetype && b.GroupId == groupID && b.ComponentID == componentID);

        //No barrier found
        if (barrierList == null)
        {
            Debug.LogError($"ERROR: Barrier with groupID: {groupID} and componentID: {componentID} not found");
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

    private void SetDeckState(LaneTypes lanetype, int groupID, int componentID, DeckState newState)
    {
        //Finds all decks with id
        List<Bridge> bridgeList = bridges.FindAll(b => b.laneType == lanetype && b.GroupId == groupID && b.ComponentID == componentID);

        //No barrier found
        if (bridgeList == null)
        {
            Debug.LogError($"ERROR: Bridge with groupID: {groupID} and componentID: {componentID} not found");
            return;
        }

        foreach (var bridge in bridgeList)
        {
            if (newState != bridge.state)
            {
                bridge.IsChangingStates = true;
            }
        }
    }

    //Event listener to sensor trigger
    private void OnSensorTriggered(LaneTypes laneType, int groupId, int componentId, bool isTriggered)
    {
        TopicInformation info = new TopicInformation(laneType, groupId, ComponentTypes.sensor, componentId);
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
                    SetTrafficLightState(info.laneType, info.groupID, info.componentID, newState);
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
                    SetWarningLightState(info.laneType, info.groupID, info.componentID, newState);
                }
                catch
                {
                    Debug.LogError($"ERROR: Tried setting invalid warning light state: '{message}' for warning light with GroupID: {info.groupID} and ComponentID: {info.componentID}");
                }
                break;

            case ComponentTypes.boat_light:
                //Handle boat lights
                //Try setting new boat light state
                try
                {
                    BoatLightState newState = (BoatLightState)System.Enum.Parse(typeof(BoatLightState), message);
                    SetBoatLightState(info.laneType, info.groupID, info.componentID, newState);
                }
                catch
                {
                    Debug.LogError($"ERROR: Tried setting invalid boat light state: '{message}' for boat light with GroupID: {info.groupID} and ComponentID: {info.componentID}");
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
                    SetBarrierState(info.laneType, info.groupID, info.componentID, newState);
                }
                catch
                {
                    Debug.LogError($"ERROR: Tried setting invalid barrier state: '{message}' for barrier with GroupID: {info.groupID} and ComponentID: {info.componentID}");
                }
                break;

            case ComponentTypes.deck:
                //Handle deck(bridge)
                //Try setting new deck(bridge) state
                try
                {
                    DeckState newState = (DeckState)System.Enum.Parse(typeof(DeckState), message);
                    SetDeckState(info.laneType, info.groupID, info.componentID, newState);
                }
                catch
                {
                    Debug.LogError($"ERROR: Tried setting invalid deck(bridge) state: '{message}' for deck with GroupID: {info.groupID} and ComponentID: {info.componentID}");
                }
                break;
        }
    }

    private TopicInformation ParseTopic(string topic)
    {
        string[] topicInfo = topic.Split('/');
       
        return new TopicInformation(topicInfo);
    }
}
