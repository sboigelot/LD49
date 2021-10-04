using System.Collections;
using UnityEngine;

public class ForcePushSpell : Spell
{
	public int ForcePower;

	public CargoController NearestCargo;

	public GameObject ForcePushAnim;

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

	public override bool PayWhileActive()
	{
		if (Mage.CurrentMana < ManaCost)
		{
			return false;
		}
		return true;
	}

	public override void OnHandleInputs()
	{
		if (NearestCargo == null)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			ForcePush(Vector2.up, -90f);
		}

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			ForcePush(Vector2.down, 90f);
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
		{
			ForcePush(Vector2.left, 0f);
		}

		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
		{
			ForcePush(Vector2.right, 180f);
		}
	}

	private void ForcePush(Vector2 direction, float animRotation)
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

		GameManager.Instance.StartCoroutine(ShowForcePush(NearestCargo.gameObject, animRotation));
		Mage.CurrentMana -= ManaCost;
	}

	private IEnumerator ShowForcePush(GameObject target, float animRotation)
	{
		if (ForcePushAnim == null)
		{
			yield break;
		}

		ForcePushAnim.gameObject.SetActive(true);
		ForcePushAnim.transform.localPosition = new Vector3(
			target.transform.localPosition.x,
			target.transform.localPosition.y,
			target.transform.localPosition.z);

		ForcePushAnim.transform.rotation = Quaternion.identity;
		ForcePushAnim.transform.Rotate(new Vector3(0f, 0f, animRotation));

		int numberOfAsepriteFrame = 6;
		float asepriteFrameTime = 60f / 1000f;
		yield return new WaitForSeconds((float)numberOfAsepriteFrame * asepriteFrameTime);

		ForcePushAnim.gameObject.SetActive(false);
	}
}
