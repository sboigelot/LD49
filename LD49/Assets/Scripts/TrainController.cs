using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Range(-20, 20)]
    public float Speed; // Unit per second; negative for opposite direction
            
    public void Update()
    {
        transform.position = new Vector3(
            transform.position.x + Speed * Time.deltaTime,
            transform.position.y,
            transform.position.z);
    }
}
