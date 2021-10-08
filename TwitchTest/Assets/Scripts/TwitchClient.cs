using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using TwitchLib.Client.Events;
using System;
using System.Linq;

public class TwitchClient : MonoBehaviour
{
    private Client client;

    public string ChannelName = "tobugis";
    public string ChannelId = "622500224";

    private Dictionary<string, Action<ChatCommand>> ChatCommandLiseners =
        new Dictionary<string, Action<ChatCommand>>();

    public List<string> PendingMessages = new List<string>();

    public float DelayBetweenMessages = 1f;
    public float LastMessageSentTime;

    public void Start() 
    {
        Application.runInBackground = true;
        
        ConnectionCredentials credentials = new ConnectionCredentials(Secret.BotName, Secret.BotAccessToken);
        client = new Client();
        client.Initialize(credentials, ChannelName);

        //client.OnMessageReceived += OnMessageReceived;
        client.OnChatCommandReceived += OnChatCommandReceived;
        client.OnConnected += OnClientConnected;
        client.Connect();
    }

	private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        //SendBotChatMessage(Secret.BotName + " received the command '" +
        //    e.Command.CommandText + " " +
        //    e.Command.ArgumentsAsString +
        //    "' from " + e.Command.ChatMessage.DisplayName);

        if (ChatCommandLiseners.ContainsKey(e.Command.CommandText))
        {
            ChatCommandLiseners[e.Command.CommandText](e.Command);
        }
    }

	public void RegisterCommandHandler(string commandKey, Action<ChatCommand> action)
	{
        ChatCommandLiseners[commandKey] = action;
	}

	private void OnClientConnected(object sender, OnConnectedArgs e)
    {
        client.SendMessage(client.JoinedChannels[0], Secret.BotName + " is now connected to this channel");
    }

	public void Update() 
    {
        if (!PendingMessages.Any())
        {
            return;
        }

        if (LastMessageSentTime + DelayBetweenMessages < Time.time)
        {
            var msg = PendingMessages[0];
            SendBotChatMessageDirect(msg);
            PendingMessages.RemoveAt(0);
            LastMessageSentTime = Time.time;
        }
	}

    public JoinedChannel GetJoinedChannel()
    {
        return client.JoinedChannels[0];
    }

    public void SendBotChatMessage(string message)
    {
        PendingMessages.Add(message);
    }

    public void SendWhisper(string user, string message)
    {
        client.SendWhisper(user, message);
    }

    private void SendBotChatMessageDirect(string message)
    {
        client.SendMessage(client.JoinedChannels[0], message);
    }
}
