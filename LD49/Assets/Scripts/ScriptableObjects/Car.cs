using UnityEngine;

[CreateAssetMenu(fileName = "Car", menuName = "TrainMage/Car")]
public class Car : ScriptableObject
{
	public CargoType DesiredCargoType;

	public int ScorePerCargo;

	public GameObject CarPrefab;
}



