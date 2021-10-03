using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldBorderController : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Contains("Crate"))
        {
            var cargoController = collider.gameObject.GetComponent<CargoController>();
            cargoController.IsDestroyed = true;
            Destroy(collider.gameObject);
        }
    }
}
