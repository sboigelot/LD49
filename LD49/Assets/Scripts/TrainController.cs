using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Range(-20, 20)]
    public float Speed; // Unit per second; negative for opposite direction

    public float DestroyXLocation;

    public void Update()
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x + Speed * Time.deltaTime,
            transform.localPosition.y,
            transform.localPosition.z);

        if (transform.localPosition.x >= DestroyXLocation)
        {
            Destroy(gameObject);
        }
    }
}
