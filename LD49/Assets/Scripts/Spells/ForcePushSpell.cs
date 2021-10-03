using UnityEngine;

public class ForcePushSpell : Spell
{
	[Range(5, 20)]
	public int ForcePower;

	[Range(5, 20)]
	public int ManaCost;

	public CargoController NearestCargo;

	public override void OnUpdate()
	{		
		var cratesParent = GameManager.Instance.CrateSpawnPoint.parent;
		if (cratesParent.childCount == 0)
		{
			NearestCargo = null;
			return;
		}

		var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float maximumDistanceInUnityUnit = 3f;
		var closestDistance = maximumDistanceInUnityUnit;
		GameObject foundCargo = null;
		for (var i = 0; i < cratesParent.childCount; i++)
		{
			var child = cratesParent.GetChild(i);

			if (child.GetComponent<CargoController>() == null)
			{
				continue;
			}

			var childDistance = Vector2.Distance(worldPosition, child.transform.position);
			if (childDistance < closestDistance)
			{
				closestDistance = childDistance;
				foundCargo = child.gameObject;
			}
		}

		if (foundCargo == null)
		{
			if (NearestCargo != null)
			{
				NearestCargo.IsSelected = false;
				NearestCargo = null;
			}
			return;
		}

		//Debug.Log(closestDistance);
		if (NearestCargo != null)
		{
			NearestCargo.IsSelected = false;
		}
		NearestCargo = foundCargo.GetComponent<CargoController>();
		NearestCargo.IsSelected = true;
	}

	public override void OnHandleInputs()
	{
		if (NearestCargo == null)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			ForcePush(Vector2.up);
		}

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			ForcePush(Vector2.down);
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
		{
			ForcePush(Vector2.left);
		}

		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
		{
			ForcePush(Vector2.right);
		}
	}

	private void ForcePush(Vector2 direction)
	{
		if (NearestCargo == null)
		{
			return;
		}

		if (Mage.CurrentMana < ManaCost)
		{
			Mage.StartCoroutine(Mage.BlinkManaBar(Color.red));
			return;
		}

		NearestCargo.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * ForcePower, ForceMode2D.Impulse);
		Mage.CurrentMana -= ManaCost;
	}
}
