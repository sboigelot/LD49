using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
	public GameObject DebugCratePrefab;
	public Transform CrateSpawnPoint;

	public Transform TrainSpawnPoint;
	public Vector2 CarDisplacement;

	public Level DebugLevel;

	private Level currentLevel;

	private List<Crate> pendingCrates;

	private List<Train> pendingTrains;

	public Text TimerText;

	public GameObject MainMenuScreenPrefab;
	public Transform ScreenPlaceholder;

	public void Start()
	{
		Time.timeScale = 1f;
		SelectLevel(GameInfo.CurrentLevel);
	}

	public void SelectLevel(Level level)
	{
		if (currentLevel != null)
		{
			Debug.LogError("Changing level is not implemented without reloading the scene");
			return;
		}

		currentLevel = level;

		if (currentLevel == null)
		{
			if (DebugLevel == null)
			{
				Debug.LogError("No Level selected in GameInfo.Level and no GameManager.Debuglevel set");
				return;
			}

			currentLevel = DebugLevel;
		}		

		pendingCrates = new List<Crate>(currentLevel.Crates).OrderBy(t => t.SpawnTimeInSecond).ToList();
		pendingTrains = new List<Train>(currentLevel.Trains).OrderBy(t => t.SpawnTimeInSecond).ToList();
	}

	public void Update()
	{
		if (TimerText != null)
		{
			TimerText.text = ""+ (int)Math.Floor(Time.timeSinceLevelLoad);
		}

		HandleDebugInput();
		HandleInput();

		if (pendingCrates.Any())
		{
			if (pendingCrates[0].SpawnTimeInSecond <= Time.timeSinceLevelLoad)
			{
				SpawnCrate(pendingCrates[0]);
				pendingCrates.RemoveAt(0);
			}
		}

		if (pendingTrains.Any())
		{
			if (pendingTrains[0].SpawnTimeInSecond <= Time.timeSinceLevelLoad)
			{
				SpawnTrain(pendingTrains[0]);
				pendingTrains.RemoveAt(0);
			}
		}
	}

	private void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			var mainMenu = Instantiate(MainMenuScreenPrefab, ScreenPlaceholder);
			mainMenu.GetComponent<MainMenuScreen>().IsOverlay = true;
			Time.timeScale = 0;
		}
	}

	private void HandleDebugInput()
	{
		if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			SpawnCrate(null);
		}
	}

	public void SpawnCrate(Crate crate)
	{
		var prefab = crate == null ? DebugCratePrefab : crate.CratePrefab;
		var displacement = crate == null ? Vector2.zero : crate.Displacement;

		var newCrate = Instantiate(prefab, CrateSpawnPoint);
		newCrate.transform.position = new Vector3(
			CrateSpawnPoint.transform.position.x + displacement.x,
			CrateSpawnPoint.transform.position.y + displacement.y,
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

		var trainController = newTrain.GetComponent<TrainController>();
		trainController.Speed = train.Speed;
		var halfSizeOfScreenInUnityUnit = 35;
		trainController.DestroyXLocation = (train.Cars.Count + 1) * CarDisplacement.x + halfSizeOfScreenInUnityUnit;
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
