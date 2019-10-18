using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class M2QTTController : MonoBehaviour
{
    #region Singleton implementation
    private static M2QTTController _instance;
    public static M2QTTController Instance { get { return _instance; } }
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

    //Mqtt client handle
    private MqttClient client;

    #region Connection information
    public string BrokerHostname = "91.121.165.36";
    public int BrokerPort = 1883;
    public int TargetTeamID = 24;
    public bool ListenToController = true;
    private string clientId = "Groep24Simulator"; //Unique ID
    private byte qoSLevel = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
    #endregion

    //Topics to listen to
    public string[] subTopics = new string[] { "24/#" };

    // Start is called before the first frame update
    void Start()
    {
        if(TargetTeamID <= 0)
        {
            Debug.LogError($"ERROR: Tried connecting to a non-valid Team ID (TeamID: {TargetTeamID})");
            Application.Quit();
        }

        //Set team topics to listen to
        

        if (BrokerHostname != null && clientId != null)
        {
            Debug.Log($"Connecting to {BrokerHostname} : {BrokerPort}");
            Connect();
            client.MqttMsgPublishReceived += client_MqttMsgPublishedReceived; //Message handler
            foreach (var topic in subTopics)
            {
                client.Subscribe(new string[] { topic }, new byte[] { qoSLevel });
            }
        }

        //Add event listener
        TrafficController.OnPublishToController += Publish;
    }

    //Connects to MQTT broker
    private void Connect()
    {
        client = new MqttClient(BrokerHostname);
        try
        {
            client.Connect(clientId);
            Debug.Log($"Connected to {BrokerHostname}:{BrokerPort} as {clientId}");
            Debug.Log($"Listening to Team {TargetTeamID}");

            //Publish "established connection"
            var testMessage = $"Simulator with clientId: {clientId} established a connection and is listening on topic(s) {TargetTeamID}/#";
            Publish(testMessage);
        }
        catch(Exception e)
        {
            Debug.LogError($"Connection error: {e}");
        }
    }

    //Disconnects from MQTT broker
    private void Disconnect()
    {
        if(client != null)
        {
            client.Disconnect();
        }
    }

    //Published to the broker on a specified topic
    public void Publish(TrafficController.TopicInformation topic, string msg)
    {
        string subGroup = topic.subGroupID >= 0
            ? $"{topic.subGroupID}/{topic.componentType}/{topic.componentID}" //Contains subgroup
            : $"{topic.componentType}/{topic.componentID}"; //Does not contain subgroup

        string topicString = $"{TargetTeamID}/{topic.laneType}/{topic.groupID}/{subGroup}".ToLower();

        client.Publish(
            topicString, Encoding.UTF8.GetBytes(msg),
            qoSLevel, false);
        Debug.Log($"Published message on topic '{topicString}': '{msg}'");
    }

    //Published to the broker on main team topic
    public void Publish(string msg)
    {
        client.Publish(
            $"{TargetTeamID}", Encoding.UTF8.GetBytes(msg),
            qoSLevel, false);
        Debug.Log($"Published message on topic '{TargetTeamID}': '{msg}'");
    }

    //Message received handle function
    private void client_MqttMsgPublishedReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string msg = Encoding.UTF8.GetString(e.Message); //Decode message
        Debug.Log($"Received message from {e.Topic} : {msg}");

        //Send message to Trafficlightcontroller who handles all traffic logic
        TrafficController.Instance.HandleMessage(e.Topic, msg);
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}
