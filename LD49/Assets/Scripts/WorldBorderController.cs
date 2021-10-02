using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldBorderController : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D crate)
    {
        Destroy(crate.gameObject);
    }
}
