using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VortexSpell : Spell
{
	private Transform vortexCenter;

	public override void OnActivated(MageController mage)
	{
		if (IsActive == true)
		{
			return;
		}

		base.OnActivated(mage);

		vortexCenter = GameManager.Instance.VortexCenter;
		vortexCenter.gameObject.SetActive(true);
		vortexCenter.GetComponent<Animator>().Play("Appear");
		MoveVotexCenterToMouseCursor();
	}

	public override void OnDeactivated()
	{
		base.OnDeactivated();
		Mage.StartCoroutine(CloseVortex());		
	}

	private IEnumerator CloseVortex()
	{
		if (vortexCenter == null)
		{
			yield break;
		}

		var animator = vortexCenter.GetComponent<Animator>();
		animator.Play("Disappear");

		// Length is always 0
		//var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
		//yield return new WaitForSeconds(clipInfo.Length);

		int numberOfAsepriteFrame = 6;
		float asepriteFrameTime = 60f / 1000f;
		yield return new WaitForSeconds((float)numberOfAsepriteFrame * asepriteFrameTime);

		vortexCenter.gameObject.SetActive(false);
	}

	public override bool PayWhileActive()
	{
		var realVortexCost = ManaCost * Time.deltaTime;
		if (Mage.CurrentMana < realVortexCost)
		{
			return false;
		}

		Mage.CurrentMana -= realVortexCost;
		return true;
	}

	public override void OnHandleInputs()
	{
		base.OnHandleInputs();
		if (Input.GetMouseButtonDown(0))
		{
			MageController.Instance.SelectSpell(GameManager.Instance.Spells[0]);
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		MoveVotexCenterToMouseCursor();
	}

	private void MoveVotexCenterToMouseCursor()
	{
		var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		vortexCenter.position = new Vector3(
			worldPosition.x,
			worldPosition.y,
			vortexCenter.position.z);
	}


}

