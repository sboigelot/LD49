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

	public void ReBuild()
	{
		if (Spell == null)
		{
			return;
		}

		DisplayNameText.text = Spell.DisplayName;
		Image.sprite = Spell.Icon;
	}

	public void Update()
	{
		if (SelectionHighlight == null)
		{
			return;
		}

		SelectionHighlight.color = MageController.Instance.CurrentSpell == Spell ?
			SelectedColor : 
			NotSelectedColor;
	}

	public void SelectSpell()
	{
		MageController.Instance.SelectSpell(Spell);
	}
}
