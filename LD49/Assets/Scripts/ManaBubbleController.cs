using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBubbleController : FloatingText
{
	public SpriteRenderer SpriteRenderer;

	public int ManaBonus;

	public void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

			if (SpriteRenderer.bounds.Contains(mousePos2D))
			{
				OnClick();
			}
		}
	}

	public void OnClick()
	{
		SoundFxLibrary.PlayRandom(SoundFxLibrary.Instance.BubblePop, true);
		int scaledBonus = (int)Math.Round(ManaBonus * transform.localScale.x);
		GameManager.Instance.SpawnFloatingText(transform.localPosition, Color.magenta, "+" + scaledBonus);
		
		var mage = MageController.Instance;
		mage.CurrentMana += scaledBonus;
		if (mage.CurrentMana > mage.MaxMana)
		{
			mage.CurrentMana = mage.MaxMana;
		}
		Destroy(gameObject);
	}
}
