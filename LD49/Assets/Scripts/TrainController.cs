using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Range(-20, 20)]
    public float Speed; // Unit per second; negative for opposite direction

    public float DestroyXLocation;

    private Rigidbody2D rb2D;

    public List<CarController> Cars;

	public void Start()
	{
		rb2D = GetComponent<Rigidbody2D>();
	}

	public void FixedUpdate()
    {
        //var velocity = new Vector2(Speed, 0);
        //rb2D.MovePosition(rb2D.position + velocity * Time.fixedDeltaTime);

        var displacement = Speed * Time.deltaTime;
        transform.localPosition = new Vector3(
            transform.localPosition.x + displacement,
            transform.localPosition.y,
            transform.localPosition.z);

        foreach (var car in Cars)
        {
            foreach (var cargo in car.StoredCargos)
            {
                cargo.transform.localPosition = new Vector3(
                    cargo.transform.localPosition.x + displacement,
                    cargo.transform.localPosition.y,
                    cargo.transform.localPosition.z);
            }
        }

        if (transform.localPosition.x >= DestroyXLocation)
        {
            Destroy(gameObject);
        }
    }
}
