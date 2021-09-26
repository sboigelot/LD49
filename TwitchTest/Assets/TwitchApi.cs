using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchLib.Unity;
using UnityEngine;

namespace Assets
{
	public class TwitchApi : MonoBehaviour
	{
		public Api Api;

		private TwitchClient twitchClient;

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
			if (Input.GetKey(KeyCode.Alpha2))
			{
				Api.Invoke(
					Api.Undocumented.GetChattersAsync(twitchClient.GetJoinedChannel().Channel),
					OnGetChattersCompleted);
			}
		}

		private void OnGetChattersCompleted(List<ChatterFormatted> chatters)
		{
			Debug.Log("List of " + chatters.Count + " viewers: ");
			foreach (var chatter in chatters)
			{
				Debug.Log(chatter.Username);
			}
		}
	}
}
