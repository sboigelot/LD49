using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using TwitchLib.Client.Events;
using System;

public class TwitchClient : MonoBehaviour
{
    private Client client;

    public string ChannelName = "tobugis";
    public string ChannelId = "622500224";

    public void Start() 
    {
        Application.runInBackground = true;
        
        ConnectionCredentials credentials = new ConnectionCredentials(Secret.BotName, Secret.BotAccessToken);
        client = new Client();
        client.Initialize(credentials, ChannelName);

        client.OnMessageReceived += OnMessageReceived;
        client.OnConnected += OnClientConnected;
        client.Connect();
    }

	private void OnClientConnected(object sender, OnConnectedArgs e)
    {
        client.SendMessage(client.JoinedChannels[0], Secret.BotName + " is now connected to this channel");
    }

	public void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            client.SendMessage(client.JoinedChannels[0], "This is a message from the bot");
        }
    }

    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        Debug.Log(e.ChatMessage.Username + ": " + e.ChatMessage.Message);
    }

    public JoinedChannel GetJoinedChannel()
    {
        return client.JoinedChannels[0];
    }
}
