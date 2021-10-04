using UnityEngine;

public class MeditateSpell : Spell
{
	public float Frequency;

	private float LastEmission;

	public RangeInt EmissionCount;

	public bool RandomRotation;
	public bool RandomScale;
	public Vector2 RandomScaleRange;

	public Bounds WorldPossitionBoundaries;

	public override void OnActivated(MageController mage)
	{
		base.OnActivated(mage);
		LastEmission = Time.time;
	}

	public override void OnUpdate()
	{
		RegeneratePassiveMana();

		if (Time.time >= LastEmission + Frequency)
		{
			int newBubbleCount = Random.Range(EmissionCount.start, EmissionCount.end + 1);
			for (int i = 0; i < newBubbleCount; i++)
			{
				EmitBubble();
			}
			LastEmission = Time.time;
		}
	}

	private void EmitBubble()
	{
		var bubble = GameManager.Instance.SpawnBubble(new Vector3(
			Random.Range(WorldPossitionBoundaries.min.x, WorldPossitionBoundaries.max.x),
			Random.Range(WorldPossitionBoundaries.min.y, WorldPossitionBoundaries.max.y),
			0f));

		if (RandomRotation)
		{
			bubble.transform.Rotate(new Vector3(0, 0, Random.Range(-180, 180)));
		}

		if (RandomScale)
		{
			var scale = Random.Range(RandomScaleRange.x, RandomScaleRange.y);
			bubble.transform.localScale = new Vector3(scale, scale, 1);
		}
	}

	private void RegeneratePassiveMana()
	{
		if (Mage.CurrentMana < Mage.MaxMana)
		{
			Mage.CurrentMana -= (ManaCost * Time.deltaTime);
		}
	}
}

