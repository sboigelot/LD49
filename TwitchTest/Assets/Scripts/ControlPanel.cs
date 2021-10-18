using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    public QueueManager QueueManager;

    public InputField StartTimeInput;

    public InputField IntroDurationInMinutesInput;

    public InputField RemoveInput;

    public InputField OutputText;

    public InputField SkipInput;
    public InputField TakeInput;

    public void OnGenerateYoutubeChapters()
    {
        if (OutputText == null)
        {
            return;
        }

        if (QueueManager == null)
        {
            OutputText.text = "QueueManager null";
            return;
        }

        if (StartTimeInput == null)
        {
            OutputText.text = "StartTimeInput null";
            return;
        }

        DateTime dt;
        if (!DateTime.TryParse(StartTimeInput.text, out dt))
        {
            OutputText.text = "'"+ StartTimeInput.text + "' is not a valid DateTime";
            var first = Played().FirstOrDefault();
            if (first != null)
            {
                OutputText.text += Environment.NewLine + "Start at " + first.GamePlayedTime;
            }
            return;
        }

        int introDuration = 0;
        if (IntroDurationInMinutesInput != null)
        {
            int.TryParse(IntroDurationInMinutesInput.text, out introDuration);
        }

        OutputText.text = "Chapters: " +
            Environment.NewLine + 
            Environment.NewLine;


        OutputText.text += "00:00:00 Intro" + Environment.NewLine;

        var played = Played().Where(sub => sub.GamePlayedTime >= dt).ToList();
        foreach (var sub in played)
        {
            var startSub = (sub.GamePlayedTime - dt).Add(TimeSpan.FromMinutes(introDuration));
            var nameSub = RemoveInput != null && RemoveInput.text != "" ? sub.GameLink.Replace(RemoveInput.text, "") : sub.GameLink;
            OutputText.text += 
                string.Format("{0:00}",startSub.Hours) + ":" +
                string.Format("{0:00}", startSub.Minutes) + ":" +
                string.Format("{0:00}", startSub.Seconds) + " " +
                nameSub + Environment.NewLine;
        }

        OutputText.text += Environment.NewLine + Environment.NewLine + "Link to Ludum Dare Game Pages: " + Environment.NewLine;

        foreach (var sub in played)
        {
            var nameSub = RemoveInput != null && RemoveInput.text != "" ? sub.GameLink.Replace(RemoveInput.text, "") : sub.GameLink;
            OutputText.text +=  nameSub + ": " + sub.GameLink + Environment.NewLine;
        }
    }

    public IEnumerable<GameJamSubmission> Played()
    {
        if (QueueManager == null || QueueManager.GameSubmissions == null)
        {
            return new List<GameJamSubmission>();
        }

        int skip = 0;
        if (SkipInput != null)
        {
            int.TryParse(SkipInput.text, out skip);
        }

        int take = 100;
        if (TakeInput != null)
        {
            int.TryParse(TakeInput.text, out take);
        }

        return QueueManager
            .GameSubmissions
            .Where(sub => sub.GamePlayed)
            .OrderBy(sub => sub.GamePlayedTime)
            .Skip(skip)
            .Take(take);
    }
}
