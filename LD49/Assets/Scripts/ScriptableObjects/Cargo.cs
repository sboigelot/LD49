using UnityEngine;


[CreateAssetMenu(fileName = "Cargo", menuName = "TrainMage/Cargo")]
public class Cargo : ScriptableObject
{
	public float SpawnTimeInSecond;

	public GameObject CargoPrefab;

	public Vector2 Displacement;
}
