using System;
using UnityEngine;

public class WorldImpactSpell : Spell
{
	public Vector3 Intensity;

	public override void OnHandleInputs()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			ChangeGravity(Intensity.y);
		}

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			ChangeGravity(-Intensity.y);
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
		{
			TiltTrainStation(-Intensity.z);
		}

		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
		{
			TiltTrainStation(Intensity.z);
		}
	}

	private void TiltTrainStation(float intensity)
	{
		PayAnd(() => GameManager.Instance.TiltWorld(intensity));
	}

	private void ChangeGravity(float intensity)
	{
		PayAnd(()=>GameManager.Instance.ImpactGravity(intensity));
	}

	public void PayAnd(Action action)
	{
		if (Mage.CurrentMana < ManaCost)
		{
			Mage.StartCoroutine(Mage.BlinkManaBar(Color.red));
			return;
		}

		action();

		Mage.CurrentMana -= ManaCost;
	}

	public override bool PayWhileActive()
	{
		if (Mage.CurrentMana < ManaCost)
		{
			return false;
		}
		return true;
	}
}