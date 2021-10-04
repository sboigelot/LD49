using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    public GameObject ClockHandle;

    public float DurationInSecond;

    public float FullDegreeRotation = 360;

    public float StartTime;

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
