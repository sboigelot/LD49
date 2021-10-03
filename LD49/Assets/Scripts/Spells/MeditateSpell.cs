using UnityEngine;

public class MeditateSpell : Spell
{
	public int ManaRegenPerSecond;

	public override void OnUpdate()
	{
		RegenerateMana();
	}

	private void RegenerateMana()
	{
		if (Mage.CurrentMana < Mage.MaxMana)
		{
			Mage.CurrentMana += (ManaRegenPerSecond * Time.deltaTime);
		}
	}
}

