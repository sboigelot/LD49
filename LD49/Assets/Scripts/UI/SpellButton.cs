using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    public Spell Spell;

	public Text DisplayNameText;

	public Image Image;

	public Image SelectionHighlight;

	public Color SelectedColor;

	public Color NotSelectedColor;

	public Text KeyCodeText;

	public GameObject SelectionKeyCap;

	public GameObject WasdKeyCaps;

	public GameObject MouseIcon;

	public void ReBuild()
	{
		if (Spell == null)
		{
			return;
		}

		DisplayNameText.text = Spell.DisplayName;
		Image.sprite = Spell.Icon;
		KeyCodeText.text = Spell.ActivationCode.ToString().Replace("Alpha", "");
	}

	public void Update()
	{
		bool isCurrentSpell = MageController.Instance.CurrentSpell == Spell;

		if (SelectionKeyCap != null)
		{
			SelectionKeyCap.SetActive(!isCurrentSpell);
		}

		if (WasdKeyCaps != null)
		{
			WasdKeyCaps.SetActive(isCurrentSpell && Spell.UseWasd);
		}

		if (MouseIcon != null)
		{
			MouseIcon.SetActive(isCurrentSpell);
		}

		if (SelectionHighlight != null)
		{
			SelectionHighlight.color =  isCurrentSpell ? SelectedColor : NotSelectedColor;
		}

	}

	public void SelectSpell()
	{
		MageController.Instance.SelectSpell(Spell);
	}
}
