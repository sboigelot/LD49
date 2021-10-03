using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSpriteEmitter : MonoBehaviour
{
    public Sprite Sprite;

    public float Opacity;

    public float Frequency;

    private float LastEmission;

    public bool RandomRotation;

    public bool RandomScale;
        
    public Vector2 RandomScaleRange;

    public void Start()
    {
        LastEmission = Time.time;        
    }

    public void Update()
    {
        if (Time.time >= LastEmission + Frequency)
        {
            EmitSprite();
            LastEmission = Time.time;
        }
    }

	private void EmitSprite()
    {
       var floatingSprite = GameManager.Instance.SpawnFloatingSprite(gameObject.transform.position, Opacity, Sprite);

        if (RandomRotation)
        {
            floatingSprite.gameObject.transform.Rotate(new Vector3(0, 0, UnityEngine.Random.Range(-180, 180)));
        }

        if (RandomScale)
        {
            var scale = UnityEngine.Random.Range(RandomScaleRange.x, RandomScaleRange.y);
            floatingSprite.gameObject.transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}
