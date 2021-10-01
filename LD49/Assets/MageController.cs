using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MageController : MonoBehaviour
{
	public Rigidbody2D SelectedCrate;

	[Range(5, 20)]
	public int PossessionSpellPower;

	[Range(5, 20)]
	public int PossessionSpellCost;

	public float ManaRegenerationPerSecond;

	[Range(0, 100)]
	public float MaxMana;

	[Range(0, 100)]
	public float CurrentMana;

	public Slider ManaBar;
	private Image manaBarFillImage;
	private Color manaBarFillImageOriginalColor;

	public void Start()
	{
		if (ManaBar != null)
		{
			manaBarFillImage = ManaBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
			manaBarFillImageOriginalColor = manaBarFillImage.color;
		}
	}

	public void Update()
	{
		RegenerateMana();
		HandleInputs();
		UpdateUi();
	}

	private void RegenerateMana()
	{
		if (CurrentMana < MaxMana)
		{
			CurrentMana += (ManaRegenerationPerSecond * Time.deltaTime);
		}
	}

	private void UpdateUi()
	{
		if (ManaBar != null)
		{
			ManaBar.maxValue = MaxMana;
			ManaBar.value = CurrentMana;
		}
	}

	private void HandleInputs()
	{
		HandlePossessionSpell();
	}

	private void HandlePossessionSpell()
	{
		if (SelectedCrate == null)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			CastPossessionSpell(Vector2.up);
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			CastPossessionSpell(Vector2.down);
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			CastPossessionSpell(Vector2.left);
		}

		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			CastPossessionSpell(Vector2.right);
		}
	}

	private void CastPossessionSpell(Vector2 direction)
	{
		if (CurrentMana < PossessionSpellCost)
		{
			StartCoroutine(BlinkManaBar(Color.red));
			return;
		}

		SelectedCrate.AddForce(direction * PossessionSpellPower, ForceMode2D.Impulse);
		CurrentMana -= PossessionSpellCost;
	}

	private IEnumerator BlinkManaBar(Color color)
	{
		manaBarFillImage.color = color;

		yield return new WaitForSeconds(1f);

		manaBarFillImage.color = manaBarFillImageOriginalColor;
	}
}
