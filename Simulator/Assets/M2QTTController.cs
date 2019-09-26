using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class M2QTTController : MonoBehaviour
{
    private MqttClient client;

    // The connection information
    public string brokerHostname = "mqtt.flespi.io";
    public string clientId = "testVerbinding";
    public int brokerPort = 8883;
    public string userName = "test";
    public string password = "test";
    // listen on all the Topic
    static string subTopic = "groep24test/#";

    private string topic = "groep24test/test2";
    private string testMessage = "Dit is een test bericht";

    // Start is called before the first frame update
    void Start()
    {
        if(brokerHostname != null && clientId != null && userName != null && password != null)
        {
            Debug.Log("connecting to " + brokerHostname + ":" + brokerPort);
            Connect();
            client.MqttMsgPublishReceived += client_MqttMsgPublishedReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
            client.Subscribe(new string[] { subTopic }, qosLevels);
        }
    }

    private void Connect()
    {
        Debug.Log("about to connect on '" + brokerHostname + "'");

        client = new MqttClient(brokerHostname);

        Debug.Log("About to connect using '" + userName + "' / '" + password + "'");
        try
        {
            client.Connect(clientId, userName, password);
            Debug.Log("Connected to client: " + clientId);
        }
        catch(Exception e)
        {
            Debug.LogError("Connection error: " + e);
        }
    }

    private void Publish(string _topic, string msg)
    {
        client.Publish(
            _topic, Encoding.UTF8.GetBytes(msg),
            MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
    }

    private void client_MqttMsgPublishedReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string msg = Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received message from " + e.Topic + " : " + msg);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
