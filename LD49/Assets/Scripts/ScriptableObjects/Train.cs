using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Train", menuName = "TrainMage/Train")]
public class Train : ScriptableObject
{
	public float SpawnTimeInSecond;

	public float Speed; //negative speed = flipped

	public List<Car> Cars;

	public GameObject LocoPrefab;
}


