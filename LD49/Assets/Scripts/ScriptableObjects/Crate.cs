using UnityEngine;


[CreateAssetMenu(fileName = "Crate", menuName = "TrainMage/Crate")]
public class Crate : ScriptableObject
{
	public float SpawnTimeInSecond;

	public GameObject CratePrefab;

	public Vector2 Displacement;
}