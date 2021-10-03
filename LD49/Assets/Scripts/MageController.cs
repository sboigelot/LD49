using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MageController : Singleton<MageController>
{
	public Spell CurrentSpell;

	[Range(0, 100)]
	public float MaxMana;

	[Range(0, 100)]
	public float CurrentMana;

	//Meditate Spell
	public float ManaRegenerationPerSecond;

	#region UI
	public Slider ManaBar;
	private Image manaBarFillImage;
	private Color manaBarFillImageOriginalColor;
	#endregion

	public void Start()
	{
		SelectSpell(GameManager.Instance.Spells[0]); ;

		if (ManaBar != null)
		{
			manaBarFillImage = ManaBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
			manaBarFillImageOriginalColor = manaBarFillImage.color;
		}
	}

	public void SelectSpell(Spell spell)
	{
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
				SelectSpell(null);
			}
			else
			{
				CurrentSpell.OnUpdate();
				CurrentSpell.OnHandleInputs();
			}
		}

		//Move to meditate ?
		RegenerateMana();

		HandleChangeSelectedSpell();
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

	private void HandleChangeSelectedSpell()
	{
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
