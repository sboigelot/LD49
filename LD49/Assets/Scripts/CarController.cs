using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CarController : MonoBehaviour
{
    public Car Car;

    public List<GameObject> StoredCrate = new List<GameObject>();

    public void OnTriggerEnter2D(Collider2D collider)
	{
		if (StoredCrate.Contains(collider.gameObject))
		{
			return;
		}

		var cargoController = collider.gameObject.GetComponent<CargoController>();
		if (cargoController == null)
		{
			return;
		}

		if (cargoController.CargoType != Car.DesiredCargoType)
		{
			return;
		}

		StoredCrate.Add(collider.gameObject);
		GameManager.Instance.Score += Car.ScorePerCargo;
		GameManager.Instance.SpawnFloatingText(collider.gameObject, Color.green, "+" + Car.ScorePerCargo);
	}

	public void OnTriggerExit2D(Collider2D collider)
    {
        if (StoredCrate.Contains(collider.gameObject))
        {
            StoredCrate.Remove(collider.gameObject);
			GameManager.Instance.Score -= Car.ScorePerCargo;
			GameManager.Instance.SpawnFloatingText(collider.gameObject, Color.red, "-" + Car.ScorePerCargo);
		}
    }
}
