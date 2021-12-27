using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;
using UnityEngine;
using UnityEngine.UI;

public class CounterManager : MonoBehaviour
{
    public TwitchClient TwitchClient;
    public TwitchApi TwitchApi;

	public InputField TemplateText;

	public InputField CountInputField;

	string FilePath;

	public InputField FilePathInputField;

	public InputField FileTemplateText;

	public InputField ModuloChatSpamInputField;

	public int GetCount()
	{
		if (CountInputField == null)
		{
			return 0;
		}
		int c = 0;
		int.TryParse(CountInputField.text, out c);

		return c;
	}

	public void ChatAndUpdateFile()
	{
		SetCount(GetCount());
		DisplayCountInChat();
	}

	public void SetCount(int c)
	{
		if (CountInputField == null)
		{
			return;
		}

		CountInputField.text = ""+c;
		System.IO.File.WriteAllText(FilePath, GetCountText(true));
	}

	public void CountAdd()
	{
		var count = GetCount() + 1;
		SetCount(count);

		var modSpam = 1;
		if (ModuloChatSpamInputField != null && ModuloChatSpamInputField.text != null)
		{
			int.TryParse(ModuloChatSpamInputField.text, out modSpam);
			if (modSpam <= 0)
			{
				modSpam = 1;
			}
		}

		if (count % modSpam == 0)
		{
			DisplayCountInChat();
		}
	}

	public void CountRemove()
	{
		SetCount(GetCount() - 1);
		DisplayCountInChat();
	}

	public void CountReset()
	{
		SetCount(0);
		DisplayCountInChat();
	}

	public void Start()
	{
		FilePath = Application.persistentDataPath + "\\counter.txt";
		if (FilePathInputField != null)
		{
			FilePathInputField.text = FilePath;
		}
		
		//viewer commands
		TwitchClient.RegisterCommandHandler("count", cmd => TimedCommand(cmd, 15, DisplayCountInChat));
		TwitchClient.RegisterCommandHandler("deaths", cmd => TimedCommand(cmd, 15, DisplayCountInChat));

		//mod commands
		TwitchClient.RegisterCommandHandler("adddeath", cmd => ProtectedCommand(cmd, (c) =>
		{
			CountAdd();
		}));

		TwitchClient.RegisterCommandHandler("countadd", cmd => ProtectedCommand(cmd, (c) =>
		{
			CountAdd();
		}));

		TwitchClient.RegisterCommandHandler("countset", cmd => ProtectedCommand(cmd, (cmd) =>
		{
			if (cmd.ArgumentsAsList == null || !cmd.ArgumentsAsList.Any())
			{
				return;
			}
			int c = 0;
			int.TryParse(cmd.ArgumentsAsList[0], out c);
			SetCount(c);
			DisplayCountInChat(cmd);
		}));
	}

	public string GetCountText(bool file = false)
	{
		var count = GetCount();

		var text = "The count is {0}";
		if (TemplateText != null && TemplateText.text != null)
		{
			text = TemplateText.text;
		}

		if (file && FileTemplateText != null && FileTemplateText.text != null)
		{
			text = FileTemplateText.text;
		}

		return string.Format(text, count);
	}

	public void DisplayCountInChat()
	{
		TwitchClient.SendBotChatMessage(GetCountText());
	}

	private void DisplayCountInChat(ChatCommand obj)
	{
		DisplayCountInChat();
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
}
