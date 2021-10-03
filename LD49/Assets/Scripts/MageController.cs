using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MageController : MonoBehaviour
{
	public enum Spells
	{
		Possession,
		Vortex
	}

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

	public Spells SelectedSpell = Spells.Possession;

	public Transform VortexCenter;

	[Range(1, 20)]
	public int VortexSpeed;

	public float VertexSpellCostPerSecond;

	public void Start()
	{
		SelectedSpell = Spells.Possession;
		if (VortexCenter != null)
		{
			VortexCenter.gameObject.SetActive(false);
		}

		if (ManaBar != null)
		{
			manaBarFillImage = ManaBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
			manaBarFillImageOriginalColor = manaBarFillImage.color;
		}
	}

	public void Update()
	{
		RegenerateMana();
		PayForActiveSpell();
		HandleInputs();
		UpdateUi();
	}

	private void PayForActiveSpell()
	{
		if (SelectedSpell == Spells.Vortex)
		{
			var realVertexCost = VertexSpellCostPerSecond * Time.deltaTime;
			if (CurrentMana < realVertexCost)
			{
				SelectedSpell = Spells.Possession;
				if (VortexCenter != null)
				{
					VortexCenter.gameObject.SetActive(false);
				}

				StartCoroutine(BlinkManaBar(Color.red));
				return;
			}

			CurrentMana -= realVertexCost;
		}
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
		HandleChangeSelectedSpell();
		if (SelectedSpell == Spells.Possession)
		{
			HandlePossessionSpell();
			return;
		}

		if (SelectedSpell == Spells.Vortex)
		{
			HandleVortexSpell();
			return;
		}
	}

	private void HandleChangeSelectedSpell()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (SelectedSpell == Spells.Possession)
				SelectedSpell = Spells.Vortex;
			else
				SelectedSpell = Spells.Possession;


			if (VortexCenter != null)
			{
				VortexCenter.gameObject.SetActive(SelectedSpell == Spells.Vortex); //Coroutine set false after end of anim

				if (SelectedSpell == Spells.Vortex)
				{
					VortexCenter.GetComponent<Animator>().Play("Appear");
				}
				else
				{
					VortexCenter.GetComponent<Animator>().Play("Disappear");
				}
			}
		}
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

	private void HandleVortexSpell()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			CastVortexSpell(Vector2.up);
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			CastVortexSpell(Vector2.down);
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			CastVortexSpell(Vector2.left);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			CastVortexSpell(Vector2.right);
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

	private void CastVortexSpell(Vector2 vector2)
	{
		if (VortexCenter == null)
		{
			return;
		}

		vector2 = vector2 * Time.deltaTime * VortexSpeed;
		VortexCenter.position = new Vector3(
			VortexCenter.position.x + vector2.x,
			VortexCenter.position.y + vector2.y,
			VortexCenter.position.z);
	}

	private IEnumerator BlinkManaBar(Color color)
	{
		manaBarFillImage.color = color;

		yield return new WaitForSeconds(1f);

		manaBarFillImage.color = manaBarFillImageOriginalColor;
	}
}
