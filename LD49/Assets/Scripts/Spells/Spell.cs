using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Spell
{
	public string DisplayName;

	public Sprite Icon;

	public KeyCode ActivationCode;

	public bool IsActive;

	protected MageController Mage;

	public virtual void OnActivated(MageController mage)
	{
		Mage = mage;
		IsActive = true;
	}

	public virtual void OnDeactivated()
	{
	}

	public virtual void OnHandleInputs()
	{
	}

	public virtual void OnUpdate()
	{
	}

	public virtual bool PayWhileActive()
	{
		return true;
	}
}

