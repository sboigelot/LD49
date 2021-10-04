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

        if (!IsBroken && HitPoint <= initialHp / 2)
        {
            IsBroken = true;
            MainSpriteRenderer.sprite = BrokenSprite;
            GameManager.Instance.SpawnFloatingText(transform.position, Color.yellow, "Be carefull!");
        }

        if (HitPoint <= 0)
        {
            //TODO Spawn Explosion Effect
            GameManager.Instance.SpawnFloatingText(transform.position, Color.yellow, "Big bada boum");
            Destroy(gameObject);
        }
    }
}
