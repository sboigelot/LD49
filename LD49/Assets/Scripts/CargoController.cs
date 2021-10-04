using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    public bool IsSelected;

    public float HitPoint;

    private float initialHp;

    public SpriteRenderer MainSpriteRenderer;

    public GameObject SelectionSprite;

    public Sprite BrokenSprite;

    public CargoType CargoType;

    public bool IsDestroyed;

    public bool IsBroken;

	public void Start()
	{
        initialHp = HitPoint;
	}

	void Update()
    {
        SelectionSprite.SetActive(IsSelected);
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        var force = collision2D.relativeVelocity;
        HitPoint -= force.magnitude;

        if (force.magnitude > 2f)
        {
            PlayCollideSound();
        }

        if (!IsBroken && HitPoint <= initialHp / 2)
        {
            PlayBreakSound();
            IsBroken = true;
            MainSpriteRenderer.sprite = BrokenSprite;
            GameManager.Instance.SpawnFloatingText(transform.position, Color.yellow, "Be carefull!");
        }

        if (HitPoint <= 0)
        {
            PlayBreakSound();
            GameManager.Instance.SpawnPoofAnim(transform.position);
            Destroy(gameObject);
        }
    }

	private void PlayCollideSound()
    {
        switch (CargoType)
        {
            case CargoType.Crate:
                SoundFxLibrary.PlayRandom(SoundFxLibrary.Instance.CollideCrate, true);
                break;

            case CargoType.Bag:
                SoundFxLibrary.PlayRandom(SoundFxLibrary.Instance.CollideBag, true);
                break;

            case CargoType.Barrel:
                SoundFxLibrary.PlayRandom(SoundFxLibrary.Instance.CollideBarrel, true);
                break;
        }
    }

	private void PlayBreakSound()
	{
        switch (CargoType)
        {
            case CargoType.Crate:
                SoundFxLibrary.PlayRandom(SoundFxLibrary.Instance.BrakeCargo, true);
                break;

            case CargoType.Bag:
                SoundFxLibrary.PlayRandom(SoundFxLibrary.Instance.BreakBag, true);
                break;

            case CargoType.Barrel:
                SoundFxLibrary.PlayRandom(SoundFxLibrary.Instance.BrakeBarrel, true);
                break;

        }
    }
}
