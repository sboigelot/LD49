using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "TrainMage/WorldEvent")]
public class WorldEvent : ScriptableObject
{
	public float SpawnTimeInSecond;

	public Vector3 Intensity;

	public WorldEventType WorldEventType;
}


