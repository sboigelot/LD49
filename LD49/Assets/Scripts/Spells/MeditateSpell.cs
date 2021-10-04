using UnityEngine;

public class MeditateSpell : Spell
{
	public override void OnUpdate()
	{
		RegenerateMana();
	}

	private void RegenerateMana()
	{
		if (Mage.CurrentMana < Mage.MaxMana)
		{
			Mage.CurrentMana -= (ManaCost * Time.deltaTime);
		}
	}
}

