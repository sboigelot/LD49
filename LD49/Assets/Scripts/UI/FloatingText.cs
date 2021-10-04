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

    public AnimationCurve XAnimationCurveOverLifetime;

	public void Start()
	{
        BirthTime = Time.time;
	}

	public void FixedUpdate()
    {
        var speedx = Speed.x;
        if (XAnimationCurveOverLifetime != null)
        {
            var percent = (Time.time - BirthTime) / LifeTime;
            speedx = speedx * XAnimationCurveOverLifetime.Evaluate(percent);
        }

        transform.localPosition = new Vector3(
            transform.localPosition.x + speedx * Time.deltaTime,
            transform.localPosition.y + Speed.y * Time.deltaTime,
            transform.localPosition.z);

        if (BirthTime + LifeTime < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
