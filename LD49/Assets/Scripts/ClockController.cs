using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    public GameObject ClockHandle;

    public float DurationInSecond;

    public float FullDegreeRotation = 360;

    public float StartTime;

    public Sprite SpriteClockExtrusionT;
    public Sprite SpriteClockExtrusionC;
    public Sprite SpriteClockExtrusionE;

    public Transform ClockExtrusionPlaceholder;
    public GameObject ClockExtrusionPrefab;

    public void RebuildClockExtrusions()
    {
        ClockExtrusionPlaceholder.ClearChildren();

        foreach (var train in GameManager.Instance.PendingTrains)
        {
            var ext = Instantiate(ClockExtrusionPrefab, ClockExtrusionPlaceholder);
            ext.GetComponent<SpriteRenderer>().sprite = SpriteClockExtrusionT;

            var spawnTime = train.SpawnTimeInSecond;
            var percentElapsed = spawnTime / DurationInSecond;
            var effectiveRotation = FullDegreeRotation * percentElapsed;
            ext.transform.Rotate(new Vector3(0f, 0f, -effectiveRotation));
        }

        foreach (var cargo in GameManager.Instance.PendingCargos)
        {
            var ext = Instantiate(ClockExtrusionPrefab, ClockExtrusionPlaceholder);
            ext.GetComponent<SpriteRenderer>().sprite = SpriteClockExtrusionC;

            var spawnTime = cargo.SpawnTimeInSecond;
            var percentElapsed = spawnTime / DurationInSecond;
            var effectiveRotation = FullDegreeRotation * percentElapsed;
            ext.transform.Rotate(new Vector3(0f, 0f, -effectiveRotation));
        }

        foreach (var worldEvent in GameManager.Instance.PendingWorldEvents.Take(GameManager.Instance.PendingWorldEvents.Count - 1))
        {
            var ext = Instantiate(ClockExtrusionPrefab, ClockExtrusionPlaceholder);
            ext.GetComponent<SpriteRenderer>().sprite = SpriteClockExtrusionE;

            var spawnTime = worldEvent.SpawnTimeInSecond;
            var percentElapsed = spawnTime / DurationInSecond;
            var effectiveRotation = FullDegreeRotation * percentElapsed;
            ext.transform.Rotate(new Vector3(0f, 0f, -effectiveRotation));
        }
    }

    public void Start()
	{
        StartTime = Time.time;
	}

	public void Update()
    {
        if (ClockHandle == null)
        {
            return;
        }

        ClockHandle.transform.rotation = Quaternion.identity;

        var elapsedTime = Time.time - StartTime;
        var percentElapsed = elapsedTime / DurationInSecond;
        var effectiveRotation = FullDegreeRotation * percentElapsed;
        ClockHandle.transform.Rotate(new Vector3(0f, 0f, -effectiveRotation));
    }
}
