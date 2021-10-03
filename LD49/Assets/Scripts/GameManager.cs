using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public GameObject DebugCratePrefab;
	public Transform CrateSpawnPoint;

	public Transform TrainSpawnPoint;
	public Vector2 CarDisplacement;

	public List<Level> Levels;

	private Level currentLevel;

	private List<Crate> pendingCrates;

	private List<Train> pendingTrains;

	public void SelectLevel(Level level)
	{
		currentLevel = level;

		pendingCrates = new List<Crate>(level.Crates).OrderBy(t => t.SpawnTimeInSecond).ToList();
		pendingTrains = new List<Train>(level.Trains).OrderBy(t => t.SpawnTimeInSecond).ToList();
	}

	public void Update()
	{
		HandleDebugInput();

		if (currentLevel == null)
		{
			if (Levels.Any())
			{
				SelectLevel(Levels.First());
			}
			return;
		}

		if (pendingCrates.Any())
		{
			if (pendingCrates[0].SpawnTimeInSecond <= Time.time)
			{
				SpawnCrate(pendingCrates[0]);
				pendingCrates.RemoveAt(0);
			}
		}

		if (pendingTrains.Any())
		{
			if (pendingTrains[0].SpawnTimeInSecond <= Time.time)
			{
				SpawnTrain(pendingTrains[0]);
				pendingTrains.RemoveAt(0);
			}
		}
	}

	private void HandleDebugInput()
	{
		if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			SpawnCrate(new Crate { CratePrefab = DebugCratePrefab });
		}
	}

	public void SpawnCrate(Crate crate)
	{
		var newCrate = Instantiate(crate.CratePrefab, CrateSpawnPoint);
		newCrate.transform.position = new Vector3(
			CrateSpawnPoint.transform.position.x + crate.Displacement.x,
			CrateSpawnPoint.transform.position.y + crate.Displacement.y,
			CrateSpawnPoint.transform.position.z);
		FindObjectOfType<MageController>().SelectedCrate = newCrate.GetComponent<Rigidbody2D>();
	}

	public void SpawnTrain(Train train)
	{
		var newTrain = new GameObject("Train", typeof(TrainController));

		newTrain.transform.SetParent(TrainSpawnPoint);
		newTrain.transform.position = new Vector3(
			TrainSpawnPoint.transform.position.x,
			TrainSpawnPoint.transform.position.y,
			TrainSpawnPoint.transform.position.z);

		SpawnLocomotive(train, newTrain);

		for (int i = 0; i < train.Cars.Count; i++)
		{
			Car car = train.Cars[i];

			SpawnCar(car, i, newTrain);
		}

		newTrain.GetComponent<TrainController>().Speed = train.Speed;
	}

	private void SpawnLocomotive(Train train, GameObject newTrain)
	{
		var newCrate = Instantiate(train.LocoPrefab, newTrain.transform);
		newCrate.transform.position = new Vector3(
			newTrain.transform.transform.position.x,
			newTrain.transform.transform.position.y,
			newTrain.transform.transform.position.z);
	}

	private void SpawnCar(Car car, int i, GameObject newTrain)
	{
		var newCrate = Instantiate(car.CarPrefab, newTrain.transform);

		newCrate.transform.position = new Vector3(
			newTrain.transform.transform.position.x - (i+1) * CarDisplacement.x,
			newTrain.transform.transform.position.y - CarDisplacement.y,
			newTrain.transform.transform.position.z);
	}
}