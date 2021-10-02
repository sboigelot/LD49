using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CarController : MonoBehaviour
{
    public List<GameObject> StoredCrate = new List<GameObject>();

    public void OnTriggerEnter2D(Collider2D crate)
    {
        if (!StoredCrate.Contains(crate.gameObject))
        {
            StoredCrate.Add(crate.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D crate)
    {
        if (StoredCrate.Contains(crate.gameObject))
        {
            StoredCrate.Remove(crate.gameObject);
        }
    }
}
