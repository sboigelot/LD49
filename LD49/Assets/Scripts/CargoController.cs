using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    public bool IsSelected;

    public float HitPoint;

    public GameObject SelectionSprite;

    public CargoType CargoType;

    public bool IsDestroyed;

	void Update()
    {
        SelectionSprite.SetActive(IsSelected);
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        var force = collision2D.relativeVelocity;

        HitPoint -= force.magnitude;
        if (HitPoint <= 0)
        {
            //TODO Spawn Explosion Effect
            Destroy(gameObject);
        }
    }
}
