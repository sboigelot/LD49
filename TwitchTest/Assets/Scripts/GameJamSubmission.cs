using System;

public class GameJamSubmission
{
	public int Id { get; set; }

	#region Key
	public int ChatterTwitchId { get; set; }
	public int GameJamStreamId { get; set; } //Change to [BsonRef("customers")] later?
	public string GameLink { get; set; }
	#endregion

	public string ChatterTwitchName { get; set; }

	public bool IsVIP { get; set; }

	public bool IsSub { get; set; }

	public bool IsModerator { get; set; }
	
	public bool IsFollower { get; set; }

	public int BitDonatedThisStream { get; set; }

	public bool GamePlayed { get; set; }

	public DateTime GamePlayedTime { get; set; }

	public int PreviousStreamSubmissionCount { get; set; }

	public bool Deleted { get; set; }

	public int RafflePoints { get; set; }

	public int SubscribedMonthCount { get; set; }
}