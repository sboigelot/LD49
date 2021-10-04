using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MageController : MonoBehaviourSingleton<MageController>
{
	public Spell CurrentSpell;

	[Range(0, 300)]
	public float MaxMana;

	[Range(0, 300)]
	public float CurrentMana;

	#region UI
	public Slider ManaBar;
	private Image manaBarFillImage;
	private Color manaBarFillImageOriginalColor;
	public Slider SpellCostSlider;
	#endregion

	public void Start()
	{
		if (ManaBar != null)
		{
			manaBarFillImage = ManaBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
			manaBarFillImageOriginalColor = manaBarFillImage.color;
		}
	}

	public void SelectSpell(Spell spell)
	{
		if (CurrentSpell == spell)
		{
			return;
		}

		if (CurrentSpell != null)
		{
			CurrentSpell.OnDeactivated();
		}

		CurrentSpell = spell;
		CurrentSpell.OnActivated(this);
	}

	public void Update()
	{
		if (CurrentSpell != null)
		{
			if (!CurrentSpell.PayWhileActive())
			{
				SelectSpell(GameManager.Instance.Spells[0]);
			}
			else
			{
				CurrentSpell.OnUpdate();
				CurrentSpell.OnHandleInputs();
			}
		}
		else
		{
			if (GameManager.Instance != null &&
				GameManager.Instance.Spells != null &&
				GameManager.Instance.Spells.Any())
			{
				SelectSpell(GameManager.Instance.Spells[0]);
			}
		}

		HandleChangeSelectedSpell();
		UpdateUi();
	}

	private void UpdateUi()
	{
		if (ManaBar != null)
		{
			ManaBar.maxValue = MaxMana;
			ManaBar.value = CurrentMana;

			if (SpellCostSlider != null)
			{
				if (CurrentSpell == null)
				{
					SpellCostSlider.gameObject.SetActive(false);
					return;
				}

				SpellCostSlider.gameObject.SetActive(true);
				SpellCostSlider.maxValue = CurrentMana;
				SpellCostSlider.value = CurrentSpell.ManaCost;

				var image = SpellCostSlider.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
				image.color = new Color(
					image.color.r,
					image.color.g,
					image.color.b,
					CurrentSpell.ManaCost < CurrentMana ? 0.65f : 1f);
			}
		}
	}

	private void HandleChangeSelectedSpell()
	{
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1))
		{
			SelectSpell(GameManager.Instance.Spells[0]);
		}

		foreach (var spell in GameManager.Instance.Spells)
		{
			if (Input.GetKeyDown(spell.ActivationCode))
			{
				SelectSpell(spell);
			}
		}
	}

	public IEnumerator BlinkManaBar(Color color)
	{
		manaBarFillImage.color = color;

		yield return new WaitForSeconds(1f);

		manaBarFillImage.color = manaBarFillImageOriginalColor;
	}
}
