using UnityEngine;
using UnityEngine.UI;

public class QueueGirdItemController : MonoBehaviour
{
	public GameObject GoldenTicketImage;

	public GameObject GoldenBorder;

	public Text PositionText;

	public Image StarImage;

	public Image SilverStarImage;

	public Text NameText;

	public void Setup(bool nowPlaying, int position, string name, bool vipsub, bool follow, bool hasGoldenTicket)
	{
		PositionText.text = ""+position;
		PositionText.gameObject.SetActive(!nowPlaying && !hasGoldenTicket);

		var gold = !nowPlaying && hasGoldenTicket;
		GoldenTicketImage.gameObject.SetActive(gold);
		GoldenBorder.gameObject.SetActive(gold);

		NameText.text = name;
		NameText.alignment = nowPlaying ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;

		StarImage.gameObject.SetActive(vipsub);
		SilverStarImage.gameObject.SetActive(!vipsub && follow);
	}
}
