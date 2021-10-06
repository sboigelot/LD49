using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Unity;
using UnityEngine;

public class TwitchApi : MonoBehaviour
{
	public Api Api;

	private TwitchClient twitchClient;

	public List<ChatterFormatted> Chatters;

	public List<ChannelFollow> Followers = new List<ChannelFollow>();

	public void Start()
	{
		Application.runInBackground = true;
		Api = new Api();
		Api.Settings.AccessToken = Secret.BotAccessToken;
		Api.Settings.ClientId = Secret.ClientId;
	}

	public void Awake()
	{
		twitchClient = FindObjectOfType<TwitchClient>().GetComponent<TwitchClient>();
		if (twitchClient == null)
		{
			Debug.LogError("TwitchApi didn't find a TwitchClient");
		}
	}

	public void Update()
	{
		if (Input.GetKey(KeyCode.KeypadEnter))
		{
			//RefreshFollowerList(null);
		}
	}

	public void GetChatters()
	{
		Api.Invoke(
			Api.Undocumented.GetChattersAsync(twitchClient.GetJoinedChannel().Channel),
			OnGetChattersCompleted);
	} 

	private void OnGetChattersCompleted(List<ChatterFormatted> chatters)
	{
		Chatters = chatters;
		Debug.Log("List of " + chatters.Count + " viewers: ");
		foreach (var chatter in chatters)
		{
			Debug.Log(chatter.Username);
		}
	}

	public bool IsFollower(string twitchDisplayName)
	{
		if (Followers == null)
		{
			return false;
		}

		return Followers.Any(f => f.User.DisplayName.ToLowerInvariant() == twitchDisplayName.ToLowerInvariant());
	}


	public void RefreshFollowerList(Action onCompleted)
	{
		Api.Invoke(Api.V5.Channels.GetAllFollowersAsync(twitchClient.ChannelId),
			(List<ChannelFollow> channelFollows) =>
			{
				OnGetAllFollowersCompleted(channelFollows);
				if (onCompleted != null)
				{
					onCompleted();
				}
			});
	}

	private void OnGetAllFollowersCompleted(List<ChannelFollow> channelFollows)
	{
		Followers = channelFollows;
	}
}