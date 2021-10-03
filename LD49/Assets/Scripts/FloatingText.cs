using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Vector2 Speed;

    public float LifeTime;

    private float BirthTime;

    public Text Text;

    public Image Image;

	public void Start()
	{
        BirthTime = Time.time;
	}

	public void Update()
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x + Speed.x * Time.deltaTime,
            transform.localPosition.y + Speed.y * Time.deltaTime,
            transform.localPosition.z);

        if (BirthTime + LifeTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
