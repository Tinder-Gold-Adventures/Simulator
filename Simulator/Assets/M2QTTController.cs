using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class M2QTTController : MonoBehaviour
{
    //Mqtt client handle
    private MqttClient client;

    // The connection information
    public string brokerHostname = "91.121.165.36";
    public int brokerPort = 1883;
    private string clientId = "Groep24Simulator"; //Unique ID
    private byte qoSLevel = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
    // listen on all the Topic
    private string[] subTopics = { "24/#" };

    // Start is called before the first frame update
    void Start()
    {
        if(brokerHostname != null && clientId != null)
        {
            Debug.Log("connecting to " + brokerHostname + ":" + brokerPort);
            Connect();
            client.MqttMsgPublishReceived += client_MqttMsgPublishedReceived; //Message handler
            client.Subscribe(subTopics, new byte[] { qoSLevel });
        }
    }

    //Connects to MQTT broker
    private void Connect()
    {
        Debug.Log("about to connect on '" + brokerHostname + "'");
        client = new MqttClient(brokerHostname);
        try
        {
            client.Connect(clientId);
            Debug.Log("Connected to client: " + clientId);

            //Publish "established connection"
            var testMessage = $"{clientId} established a connection!";
            Publish("24", testMessage);
        }
        catch(Exception e)
        {
            Debug.LogError("Connection error: " + e);
        }
    }

    //Published to the broker on a specified topic
    private void Publish(string topic, string msg)
    {
        client.Publish(
            topic, Encoding.UTF8.GetBytes(msg),
            qoSLevel, false);
    }

    //Message received handle function
    private void client_MqttMsgPublishedReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string msg = Encoding.UTF8.GetString(e.Message); //Decode message
        Debug.Log("Received message from " + e.Topic + " : " + msg);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
