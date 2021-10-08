using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage queue of GameSubmission in twitch chat
/// 
/// viewer commands
/// 
/// !submit [gamelink]			add gamelink for viewer in the queue if a queue is open
/// !queue						show a list of the first 4 game in queue + position in queue?
/// !game						show link of current game
/// !streamergame				show link of streamer game
/// !join						join the current raffle
/// 
/// streamer commands
/// 
/// !open streamName			Create a new GameJamStream
/// !close						Close the current GameJamStream
/// !next [prio]				Take the next item in queue and set it as current + show message in chat (only take vip/sub/bit/mod if prio tag enabled)
/// !pick name					Streamer pick a specific item in queue and make it current
/// !requeue name				Streamer requeue a specific game
/// !delete name				Streamer delete a specific item in queue
/// !playmygame					Set own game as current game
/// 
/// !raffle						reset the current raffle
/// !endraffle					choose the next player in the raffle
/// !setopt [opt] [value]		set current stream option [opt] to [value]
/// 
/// </summary>
public class QueueManager : MonoBehaviour
{
	public TwitchClient TwitchClient;
	public TwitchApi TwitchApi;

	public GameJamStream CurrentStream;
	public GameJamSubmission CurrentGame;

	public Transform NowPlayingPanel;
	public QueueGirdItemController NowPlayingItem;
	public Transform NowPlayingSeparator;
	public GameObject QueueGridItemPrefab;
	public Transform QueueGridPlaceholder;
	public GameObject QueueClosedScreen;
	public Text QueueRaffleText;

	public GameJamSubmission StreamerGame = new GameJamSubmission
	{
		ChatterTwitchName = "Tobugis",
		GameLink = "https://ldjam.com/events/ludum-dare/49/the-train-station-mage"
	};

	public List<GameJamSubmission> GameSubmissions;

	[Header("Save/Load")]
	public float SaveFrequency = 30f;
	public float LastSaveGameTime = 0f;

	[Header("Raffle")]
	public GameObject RafflePanel;
	public Text RaffleRemainingTimeText;
	public Slider RaffleRemainingTimeSlider;
	public Gradient RaffleRemainingTimeSliderColors;
	public List<GameJamSubmission> RaffleParticipants;
	public float RaffleDefaultDuration = 120f;
	public float RaffleRemainingTime;

	public void RebuildUi()
	{
		QueueClosedScreen.SetActive(CurrentStream == null || !CurrentStream.IsOngoing);

		NowPlayingPanel.gameObject.SetActive(CurrentGame != null);
		NowPlayingSeparator.gameObject.SetActive(CurrentGame != null);
		if (CurrentGame != null)
		{
			NowPlayingItem.Setup(true, 0, CurrentGame.ChatterTwitchName,
				IsPriority(CurrentGame), CurrentGame.IsFollower, false);
		}
		RebuildQueueGrid(false, RaffleParticipants != null);
	}

	public void RebuildQueueGrid(bool priority, bool raffleMode)
	{
		QueueRaffleText.text = !raffleMode ? "Submitted Games" : "The Raffle is open, type <b>!join</b> now!";
		QueueGridPlaceholder.ClearChildren();

		var query = raffleMode ?
			RaffleParticipants.
				Where(r=>r.RafflePoints > 0).
				OrderByDescending(r=>r.RafflePoints).
				ToList() :
				NextInLine(priority, true).
				ToList();

		int numberOfSubToShow = CurrentGame != null ? 9 : 11;
		foreach (var submission in query.Take(numberOfSubToShow))
		{
			var queueItem = Instantiate(QueueGridItemPrefab, QueueGridPlaceholder);
			queueItem.GetComponent<QueueGirdItemController>().
				Setup(false,
				submission.RafflePoints,
				submission.ChatterTwitchName,
				IsPriority(submission),
				submission.IsFollower,
				CurrentStream.GoldenTicketPrice > 0 && submission.RafflePoints >= CurrentStream.GoldenTicketPrice);
		}

		var rest = query.Skip(numberOfSubToShow);
		if (rest.Any())
		{
			var queueItem = Instantiate(QueueGridItemPrefab, QueueGridPlaceholder);
			queueItem.GetComponent<QueueGirdItemController>().
				Setup(true,
				rest.Sum(q => q.RafflePoints),
				"And " + rest.Count() + " more submissions",
				rest.Any(q => IsPriority(q)),
				rest.Any(q=>q.IsFollower),
				false);
		}
	}

	public void Start()
	{
		GameSubmissions = new List<GameJamSubmission>();
		LoadDatabase();

		TwitchApi.RefreshFollowerList(() => {
			foreach (var subm in GameSubmissions)
			{
				subm.IsFollower = TwitchApi.IsFollower(subm.ChatterTwitchName);
				SaveDatabase();
			}
			RebuildUi();
		});
		RebuildUi();

		//viewer commands
		TwitchClient.RegisterCommandHandler("submit", OnSubmitCommand);
		TwitchClient.RegisterCommandHandler("queue", cmd => TimedCommand(cmd, 30, OnQueueCommand));
		TwitchClient.RegisterCommandHandler("game", cmd => TimedCommand(cmd, 15, OnGameCommand));
		TwitchClient.RegisterCommandHandler("streamergame", OnStreamerGameCommand);
		TwitchClient.RegisterCommandHandler("join", OnJoinCommand);

		//mod commands
		TwitchClient.RegisterCommandHandler("open", cmd => ProtectedCommand(cmd, OnOpenCommand));
		TwitchClient.RegisterCommandHandler("close", cmd => ProtectedCommand(cmd, OnCloseCommand));
		TwitchClient.RegisterCommandHandler("next", cmd => ProtectedCommand(cmd, OnNextCommand));
		TwitchClient.RegisterCommandHandler("pick", cmd => ProtectedCommand(cmd, OnPickCommand));
		TwitchClient.RegisterCommandHandler("requeue", cmd => ProtectedCommand(cmd, OnRequeueCommand));
		TwitchClient.RegisterCommandHandler("delete", cmd => ProtectedCommand(cmd, OnDeleteCommand));
		TwitchClient.RegisterCommandHandler("playmygame", cmd => ProtectedCommand(cmd, OnPayMyGameCommand));
		TwitchClient.RegisterCommandHandler("raffle", cmd => ProtectedCommand(cmd, OnRaffleCommand));
		TwitchClient.RegisterCommandHandler("endraffle", cmd => ProtectedCommand(cmd, OnEndRaffleCommand));
		TwitchClient.RegisterCommandHandler("setopt", cmd => ProtectedCommand(cmd, OnSetOptCommand));
		TwitchClient.RegisterCommandHandler("joinfor", cmd => ProtectedCommand(cmd, OnJoinForCommand));
	}

	public void Update()
	{
		RafflePanel.gameObject.SetActive(false);
		if (RaffleParticipants != null)
		{
			RaffleRemainingTime -= Time.deltaTime;
			RaffleRemainingTimeText.text = "" + (int)Math.Ceiling(RaffleRemainingTime);
			RafflePanel.gameObject.SetActive(true);

			RaffleRemainingTimeSlider.maxValue = RaffleDefaultDuration;
			RaffleRemainingTimeSlider.value = RaffleRemainingTime;
			RaffleRemainingTimeSlider.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color =
				RaffleRemainingTimeSliderColors.Evaluate(RaffleRemainingTime / RaffleDefaultDuration);

			if (RaffleRemainingTime <= 0)
			{
				RafflePanel.gameObject.SetActive(false);
				RaffleRemainingTimeText.text = "Raffle ended!";
				CloseRaffle();
				RaffleRemainingTime = 0;
			}
		}
	}

	private void OnPayMyGameCommand(ChatCommand obj)
	{
		CurrentGame = StreamerGame;
		OnGameCommand(obj);
	}

	private void OnGameCommand(ChatCommand obj)
	{
		if (CurrentGame == null)
		{
			TwitchClient.SendBotChatMessage("There is no game being played from the queue.");
			return;
		}

		TwitchClient.SendBotChatMessage("We are now playing " + CurrentGame.GameLink + " from @" + CurrentGame.ChatterTwitchName+".");
	}

	private void OnStreamerGameCommand(ChatCommand obj)
	{
		TwitchClient.SendBotChatMessage("The streamer game is: "+ StreamerGame.GameLink);
	}


	private void OnRaffleCommand(ChatCommand cmd)
	{
		if (CurrentStream == null || !CurrentStream.IsOngoing)
		{
			TwitchClient.SendBotChatMessage("There is no ongoing stream queue open.");
			return;
		}

		var raffleDuration = RaffleDefaultDuration;
		if (cmd.ArgumentsAsList.Count > 0)
		{
			float.TryParse(cmd.ArgumentsAsList[0], out raffleDuration);
		}
		RaffleRemainingTime = raffleDuration;

		TwitchApi.RefreshFollowerList(() => RebuildUi());
		RaffleParticipants = new List<GameJamSubmission>();
		TwitchClient.SendBotChatMessage("A Raffle started! type \"!join\" in chat to participate! You must have use the \"!submit [link]\" command beforehand");

		RebuildUi();
	}


	private void OnSetOptCommand(ChatCommand cmd)
	{
		if (CurrentStream == null || !CurrentStream.IsOngoing)
		{
			TwitchClient.SendBotChatMessage("There is no ongoing stream queue open.");
			return;
		}

		if (cmd.ArgumentsAsList == null || cmd.ArgumentsAsList.Count != 2)
		{
			TwitchClient.SendBotChatMessage("To use the setop command, type \"!setopt [key] [value]\" in chat.");
			return;
		}

		var key = cmd.ArgumentsAsList[0];
		switch (key)
		{
			case "gold":
				var argValue = cmd.ArgumentsAsList[1];
				int value = -1;
				int.TryParse(argValue, out value);
				CurrentStream.GoldenTicketPrice = value;
				TwitchClient.SendBotChatMessage("Option \"" + key + "\" set to: "+ value);
				SaveDatabase();
				RebuildUi();
				break;

			default:
				TwitchClient.SendBotChatMessage("Setop: unknown [key] \""+ key + "\"");
				break;
		}
	}

	private void OnJoinForCommand(ChatCommand cmd)
	{
		if (CurrentStream == null || !CurrentStream.IsOngoing)
		{
			TwitchClient.SendBotChatMessage("There is no ongoing stream queue open.");
			return;
		}

		if (cmd.ArgumentsAsList == null || cmd.ArgumentsAsList.Count != 1)
		{
			TwitchClient.SendBotChatMessage("To use the joinfor command, type \"!joinfor [username]\" in chat.");
			return;
		}

		var chatterTwitchName = cmd.ArgumentsAsList[0];
		var chatterGame = NextInLine(false, true).FirstOrDefault(s => s.ChatterTwitchName == chatterTwitchName);

		if (chatterGame != null)
		{
			//update the status of priority in case it changed since last !submit
			chatterGame.IsSub = cmd.ChatMessage.IsSubscriber;
			chatterGame.SubscribedMonthCount = cmd.ChatMessage.SubscribedMonthCount;
			chatterGame.IsVIP = cmd.ChatMessage.IsVip;
			chatterGame.IsModerator = cmd.ChatMessage.IsModerator;
			chatterGame.BitDonatedThisStream = cmd.ChatMessage.Bits;
			JoinRaffle(chatterGame);
		}
		else
		{
			TwitchClient.SendBotChatMessage("@" + cmd.ChatMessage.DisplayName + ", I didn't find "+ chatterTwitchName + " submission.");
		}
	}

	private void OnJoinCommand(ChatCommand cmd)
	{
		if (CurrentStream == null || !CurrentStream.IsOngoing)
		{
			TwitchClient.SendBotChatMessage("@" + cmd.ChatMessage.DisplayName + ", There is no ongoing stream queue open.");
			return;
		}

		if (RaffleParticipants == null || !CurrentStream.IsOngoing)
		{
			TwitchClient.SendBotChatMessage("@" + cmd.ChatMessage.DisplayName + ", There is no raffle open now.");
			return;
		}
		
		var chatterTwitchName = cmd.ChatMessage.DisplayName;
		var chatterGame = NextInLine(false, true).FirstOrDefault(s =>  s.ChatterTwitchName == chatterTwitchName);

		if (chatterGame != null)
		{
			//update the status of priority in case it changed since last !submit
			chatterGame.IsSub = cmd.ChatMessage.IsSubscriber;
			chatterGame.SubscribedMonthCount = cmd.ChatMessage.SubscribedMonthCount;
			chatterGame.IsVIP = cmd.ChatMessage.IsVip;
			chatterGame.IsModerator = cmd.ChatMessage.IsModerator;
			chatterGame.BitDonatedThisStream = cmd.ChatMessage.Bits;
			JoinRaffle(chatterGame);
		}
		else
		{
			TwitchClient.SendBotChatMessage("@" + cmd.ChatMessage.DisplayName + ", I didn't find your submission. You must have use the \"!submit [link]\" command beforehand joining a raffle");
		}
	}

	private void JoinRaffle(GameJamSubmission chatterGame)
	{
		if (RaffleParticipants.Contains(chatterGame))
		{
			return;
		}

		chatterGame.IsFollower = TwitchApi.IsFollower(chatterGame.ChatterTwitchName);
		chatterGame.RafflePoints += 1;
		if (chatterGame.IsSub || chatterGame.BitDonatedThisStream >= 500 || chatterGame.IsModerator)
		{
			chatterGame.RafflePoints += 2;
		}
		else if (chatterGame.IsVIP || chatterGame.IsFollower)
		{
			chatterGame.RafflePoints += 1;
		}
		RaffleParticipants.Add(chatterGame);

		RebuildUi();
	}

	private void OnSubmitCommand(ChatCommand cmd)
	{
		if (CurrentStream == null || !CurrentStream.IsOngoing)
		{
			TwitchClient.SendBotChatMessage("There is no ongoing stream queue open.");
			return;
		}

		if (cmd.ArgumentsAsList == null || cmd.ArgumentsAsList.Count != 1)
		{
			TwitchClient.SendBotChatMessage("To use the submit command, type \"!submit [link]\" in chat.");
			return;
		}

		//check entry is URL

		var previousGame = GameSubmissions.FirstOrDefault(s => s.ChatterTwitchName == cmd.ChatMessage.DisplayName);
		if (previousGame != null)
		{
			TwitchClient.SendBotChatMessage("@"+cmd.ChatMessage.DisplayName + ", you are already in queue. Use !queue for more info");
			return;
		}

		
		var submission = new GameJamSubmission
		{
			ChatterTwitchName = cmd.ChatMessage.DisplayName,
			GameLink = cmd.ArgumentsAsList[0],
			GameJamStreamId = CurrentStream.Id,
			IsSub = cmd.ChatMessage.IsSubscriber,
			IsFollower = TwitchApi.IsFollower(cmd.ChatMessage.DisplayName),
			SubscribedMonthCount = cmd.ChatMessage.SubscribedMonthCount,
			IsVIP = cmd.ChatMessage.IsVip,
			IsModerator = cmd.ChatMessage.IsModerator,
			BitDonatedThisStream = cmd.ChatMessage.Bits,
			GamePlayed = false,			
		};
		GameSubmissions.Add(submission);
		SaveDatabase();
		RebuildUi();
		TwitchClient.SendBotChatMessage("@"+cmd.ChatMessage.DisplayName + ", your game has been added to the queue");
	}

	private void OnQueueCommand(ChatCommand cmd)
	{
		InvokeQueueFor(cmd.ChatMessage.DisplayName);
	}

	private void InvokeQueueFor(string chatterTwitchName)
	{
		if (CurrentStream == null || !CurrentStream.IsOngoing)
		{
			TwitchClient.SendBotChatMessage("There is no ongoing stream queue open.");
			return;
		}

		if (GameSubmissions == null || !GameSubmissions.Any())
		{
			TwitchClient.SendBotChatMessage("The submission list is empty");
			return;
		}
		List<string> qStr = new List<string>();
		var list = NextInLine(false, true);
		for (int i = 0; i < 5 && i < list.Count; i++)
		{
			GameJamSubmission game = list[i];
			qStr.Add("[" + game.RafflePoints + "] " + game.ChatterTwitchName);
		}

		var message = 
			"We do not use a queue here. We use a raffle system.  " + Environment.NewLine +
			"Each time you !join a raffle you will win Raffle Points.  " + Environment.NewLine +
			"The more points you have, the more chance you will have to be selected.  " + Environment.NewLine + Environment.NewLine +
			"Top 5 in Raffle Points:   " + String.Join(", ", qStr.ToArray()) + ". ";

		var chatterGame = list.FirstOrDefault(s => s.ChatterTwitchName == chatterTwitchName);
		if (chatterGame != null)
		{
			//var indexChatter = list.IndexOf(chatterGame);
			message += "@" + chatterTwitchName + " you have " + chatterGame.RafflePoints + " Raffle Points";
		}
		else
		{
			//Check if game already player
			chatterGame = GameSubmissions.FirstOrDefault(s => s.ChatterTwitchName == chatterTwitchName);
			if (chatterGame != null)
			{
				if (chatterGame.GamePlayed)
					message += "@" + chatterTwitchName + " We already played your game at " + chatterGame.GamePlayedTime;
				else
					message += "@" + chatterTwitchName + " Something is wrong with your game queue, let's ask " + TwitchClient.ChannelName;
			}
		}

		TwitchClient.SendBotChatMessage(message);
	}

	private void OnNextCommand(ChatCommand cmd)
	{
		if (CurrentGame != null)
		{
			TwitchClient.SendBotChatMessage("The previous game was " + CurrentGame.GameLink + " from @" + CurrentGame.ChatterTwitchName + ".");
		}

		var prio = cmd.ArgumentsAsList != null &&
					cmd.ArgumentsAsList.Count > 0 && 
					cmd.ArgumentsAsList[0] == "prio";

		var nextGame = NextInLine(prio, true).FirstOrDefault();

		if (nextGame == null)
		{
			TwitchClient.SendBotChatMessage("The is no more "+(prio ? "priority" : "" )+" game in the queue");
			return;
		}

		nextGame.GamePlayed = true;
		nextGame.GamePlayedTime = DateTime.Now;
		CurrentGame = nextGame;
		SaveDatabase();
		RebuildUi();

		OnGameCommand(cmd);
	}

	private void OnPickCommand(ChatCommand cmd)
	{
		if (CurrentGame != null)
		{
			TwitchClient.SendBotChatMessage("The previous game was " + CurrentGame.GameLink + " from @" + CurrentGame.ChatterTwitchName + ".");
		}

		if (cmd.ArgumentsAsList == null || cmd.ArgumentsAsList.Count != 1)
		{
			TwitchClient.SendBotChatMessage("As a mod, to pick a game, use \"!pick [twitchname]\"");
			return;
		}

		var twitchName = cmd.ArgumentsAsList[0];
		if (twitchName.ToLowerInvariant() == StreamerGame.ChatterTwitchName.ToLowerInvariant())
		{
			OnPayMyGameCommand(cmd);
			return;
		}

		PickGameOf(cmd.ChatMessage.DisplayName, twitchName);

		OnGameCommand(cmd);
	}

	private bool PickGameOf(string requester, string twitchName)
	{
		var found = GameSubmissions.LastOrDefault(s => s.ChatterTwitchName.ToLowerInvariant() == twitchName.ToLowerInvariant());
		if (found == null)
		{
			TwitchClient.SendBotChatMessage("@" + requester + ", no game found from " + twitchName);
			return false;
		}
		PickGame(found);
		return true;
	}

	private void PickGame(GameJamSubmission game)
	{
		game.GamePlayed = true;
		game.GamePlayedTime = DateTime.Now;
		CurrentGame = game;
		SaveDatabase();
		RebuildUi();
	}

	private void OnEndRaffleCommand(ChatCommand cmd)
	{
		if (CurrentGame != null)
		{
			TwitchClient.SendBotChatMessage("The previous game was " + CurrentGame.GameLink + " from @" + CurrentGame.ChatterTwitchName + ".");
		}

		if (CurrentStream == null || !CurrentStream.IsOngoing)
		{
			TwitchClient.SendBotChatMessage("There is no ongoing stream queue open.");
			return;
		}

		CloseRaffle();
	}

	public void CloseRaffle()
	{
		if (RaffleParticipants == null)
		{
			TwitchClient.SendBotChatMessage("There is no raffle open now.");
			return;
		}

		if (!RaffleParticipants.Any())
		{
			RaffleParticipants = null;
			RebuildUi();
			TwitchClient.SendBotChatMessage("No one joined the raffle yet.");
			return;
		}

		int sumOfPoint = RaffleParticipants.Sum(s => s.RafflePoints);
		int randomPoint = UnityEngine.Random.Range(0, sumOfPoint);

		if (CurrentStream.GoldenTicketPrice > 0 &&
			RaffleParticipants.Any(subm => subm.RafflePoints >= CurrentStream.GoldenTicketPrice))
		{
			RaffleParticipants = RaffleParticipants.Where(subm => subm.RafflePoints >= CurrentStream.GoldenTicketPrice).ToList();
		}

		var winner = RaffleParticipants[0];
		foreach (var p in RaffleParticipants.OrderBy(r => UnityEngine.Random.Range(0,1)))
		{
			if (randomPoint < p.RafflePoints)
			{
				winner = p;
				break;
			}

			randomPoint -= p.RafflePoints;
		}
		TwitchClient.SendBotChatMessage("The raffle winner is: @" + winner.ChatterTwitchName);

		RaffleParticipants = null;
		PickGame(winner);
		OnGameCommand(null);
	}

	private void OnRequeueCommand(ChatCommand cmd)
	{
		if (cmd.ArgumentsAsList == null || cmd.ArgumentsAsList.Count != 1)
		{
			TwitchClient.SendBotChatMessage("As a mod, to re-queue a game, use \"!requeue [twitchname]\"");
			return;
		}

		var found = GameSubmissions.LastOrDefault(s => s.GameJamStreamId == CurrentStream.Id &&
														s.ChatterTwitchName.ToLowerInvariant() == cmd.ArgumentsAsList[0].ToLowerInvariant());
		if (found == null)
		{
			TwitchClient.SendBotChatMessage("@" + cmd.ChatMessage.DisplayName + ", no game found from " + cmd.ArgumentsAsList[0]);
			return;
		}

		if (found == CurrentGame)
		{
			CurrentGame = null;
		}

		found.GamePlayed = false;
		GameSubmissions.Remove(found);
		GameSubmissions.Add(found);
		SaveDatabase();
		RebuildUi();
		InvokeQueueFor(cmd.ChatMessage.DisplayName);
	}

	private void OnDeleteCommand(ChatCommand cmd)
	{
		if (cmd.ArgumentsAsList == null || cmd.ArgumentsAsList.Count != 1)
		{
			TwitchClient.SendBotChatMessage("As a mod, to re-queue a game, use \"!requeue [twitchname]\"");
			return;
		}

		var found = GameSubmissions.LastOrDefault(s => s.GameJamStreamId == CurrentStream.Id && 
														s.ChatterTwitchName.ToLowerInvariant() == cmd.ArgumentsAsList[0].ToLowerInvariant());
		if (found == null)
		{
			TwitchClient.SendBotChatMessage("@" + cmd.ChatMessage.DisplayName + ", no game found from " + cmd.ArgumentsAsList[0]);
			return;
		}

		found.Deleted = true;
		SaveDatabase();
		RebuildUi();
		GameSubmissions.Remove(found);

		TwitchClient.SendBotChatMessage("@"+cmd.ArgumentsAsList[0]+ ", your game has been removed from the queue by a mod");
	}

	public List<GameJamSubmission> NextInLine(bool priorityOnly, bool orderByRafflePoints)
	{
		if (CurrentStream == null || GameSubmissions == null)
		{
			return new List<GameJamSubmission>();
		}

		var query = GameSubmissions.Where(s => s.GamePlayed == false && s.GameJamStreamId == CurrentStream.Id);

		if (priorityOnly)
		{
			query = query.Where(s => IsPriority(s));
		}

		if (orderByRafflePoints)
		{
			query = query.OrderByDescending(subm => subm.RafflePoints);
		}

		return query.ToList();
	}


	private bool IsPriority(GameJamSubmission game)
	{
		return game.IsModerator || game.IsVIP || game.IsSub || game.BitDonatedThisStream >= 500;
	}

	public Dictionary<string, float> lastCmdInvocation = new Dictionary<string, float>();
	public void TimedCommand(ChatCommand cmd, float minDelayBetweenInvocation, Action<ChatCommand> action)
	{
		var key = cmd.CommandText;
		if (!lastCmdInvocation.ContainsKey(key) || 
			lastCmdInvocation[key] + minDelayBetweenInvocation < Time.time)
		{
			lastCmdInvocation[key] = Time.time;
			action(cmd);
			return;
		}

		//TwitchClient.SendWhisper(cmd.ChatMessage.DisplayName, "This command can only be called every "+ minDelayBetweenInvocation +" second to avoid spam. It's probably in chat already");
	}

	public void ProtectedCommand(ChatCommand cmd, Action<ChatCommand> action)
	{
		if (cmd.ChatMessage.IsBroadcaster || cmd.ChatMessage.IsModerator)
		{
			action(cmd);
			return;
		}
		TwitchClient.SendBotChatMessage("Only the broadcaster or a mod can use this command!");
	}

	private void OnOpenCommand(ChatCommand cmd)
	{
		if (cmd.ArgumentsAsList == null || cmd.ArgumentsAsList.Count != 1)
		{
			TwitchClient.SendBotChatMessage("To use the open command, type \"!open [stream name]\" in chat");
			return;
		}

		if (CurrentStream != null)
		{
			CloseStream();
		}

		CurrentStream = new GameJamStream
		{
			IsOngoing = true,
			Name = cmd.ArgumentsAsList[0],
			StartDate = DateTime.Now
		}; 
		SaveDatabase();
		RebuildUi();

		TwitchClient.SendBotChatMessage("A new stream queue is starting: " + cmd.ArgumentsAsList[0]);
	}

	private void OnCloseCommand(ChatCommand cmd)
	{
		CloseStream();
	}

	public void CloseStream()
	{
		if (CurrentStream == null)
		{
			return;
		}

		CurrentStream.IsOngoing = false;
		SaveDatabase();
		RebuildUi();

		CurrentStream = null;
		GameSubmissions.Clear();

		TwitchClient.SendBotChatMessage("The stream queue " + name + " is closing");
	}

	public void SaveDatabase()
	{
		using (var db = new LiteDatabase(Application.persistentDataPath + @"\database.db"))
		{
			Debug.Log("Saving database in folder : " + Application.persistentDataPath);
			// Get a collection (or create, if doesn't exist)
			var streamTable = db.GetCollection<GameJamStream>("GameJamStream");
			// Index document using document Name property
			streamTable.EnsureIndex(x => x.Name);

			if (CurrentStream != null)
			{
				if (CurrentStream.Id == 0)
				{
					streamTable.Insert(CurrentStream);
				}
				else
				{
					streamTable.Update(CurrentStream);
				}
			}

			var gamesTable = db.GetCollection<GameJamSubmission>("GameJamSubmission");
			// Index document using document Name property
			gamesTable.EnsureIndex(x => x.ChatterTwitchName);
			if (GameSubmissions != null)
			{
				foreach (var gs in GameSubmissions)
				{
					if (gs.Id == 0)
					{
						gamesTable.Insert(gs);
					}
					else
					{
						gamesTable.Update(gs);
					}
				}
			}
		}
	}

	public void LoadDatabase()
	{
		using (var db = new LiteDatabase(Application.persistentDataPath + @"\database.db"))
		{
			Debug.Log("Loading database from folder : " + Application.persistentDataPath);

			// Get a collection (or create, if doesn't exist)
			var streamTable = db.GetCollection<GameJamStream>("GameJamStream");

			CurrentStream = streamTable.FindOne(s => s.IsOngoing);

			if (CurrentStream != null)
			{
				var gamesTable = db.GetCollection<GameJamSubmission>("GameJamSubmission");
				GameSubmissions = gamesTable.Find(s => s.GameJamStreamId == CurrentStream.Id && !s.Deleted).ToList();
			}
		}
	}
}
