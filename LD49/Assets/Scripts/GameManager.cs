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

	public Transform VortexCenter;

	public Level DebugLevel;

	private Level currentLevel;

	private List<Cargo> pendingCargos;
	private List<Train> pendingTrains;
	private List<WorldEvent> pendingWorldEvents;

	public Text TimerText;

	public GameObject MainMenuScreenPrefab;
	public Transform ScreenPlaceholder;

	public GameObject SpellButtonPrefab;
	public Transform SpellGrid;
	public List<Spell> Spells;

	public Sprite SpellIconForcePursh;
	public Sprite SpellIconVortex;
	public Sprite SpellIconMeditate;

	public int Score;
	public Text ScoreText;

	public Transform TrainStation;

	public void Start()
	{
		Spells = new List<Spell>
		{
			new MeditateSpell {
				ActivationCode = KeyCode.Alpha1,
				DisplayName = "Meditate",
				Icon = SpellIconMeditate,
				ManaRegenPerSecond = 50,
			},
			new ForcePushSpell {
				ActivationCode = KeyCode.Alpha2,
				DisplayName = "Force Push",
				Icon = SpellIconForcePursh,
				ForcePower = 8,
				ManaCost = 20 },
			new VortexSpell {
				ActivationCode = KeyCode.Alpha3,
				DisplayName = "Vortex",
				Icon = SpellIconVortex,
				ManaCostPerSecond = 5,
			},
		};

		BuildSpellListUi();

		Time.timeScale = 1f;
		SelectLevel(GameInfo.CurrentLevel);
	}

	public void SpawnFloatingText(GameObject gameObject, Color green, string v)
	{
		//TODO implement this!
	}

	private void BuildSpellListUi()
	{
		SpellGrid.ClearChildren();
		foreach (var spell in Spells)
		{
			var spellButton = Instantiate(SpellButtonPrefab, SpellGrid);
			var spellButtonController = spellButton.GetComponent<SpellButton>();
			spellButtonController.Spell = spell;
			spellButtonController.ReBuild();
		}
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

		pendingCargos = new List<Cargo>(currentLevel.Cargos).OrderBy(t => t.SpawnTimeInSecond).ToList();
		pendingTrains = new List<Train>(currentLevel.Trains).OrderBy(t => t.SpawnTimeInSecond).ToList();
		pendingWorldEvents = new List<WorldEvent>(currentLevel.WorldEvents).OrderBy(t => t.SpawnTimeInSecond).ToList();
	}

	public void Update()
	{
		if (TimerText != null)
		{
			TimerText.text = "TIMER: "+ (int)Math.Floor(Time.timeSinceLevelLoad);
		}

		if (ScoreText != null)
		{
			ScoreText.text = "SCORE: " + Score;
		}

		HandleDebugInput();
		HandleInput();

		if (pendingCargos.Any())
		{
			if (pendingCargos[0].SpawnTimeInSecond <= Time.timeSinceLevelLoad)
			{
				SpawnCrate(pendingCargos[0]);
				pendingCargos.RemoveAt(0);
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

		if (pendingWorldEvents.Any())
		{
			if (pendingWorldEvents[0].SpawnTimeInSecond <= Time.timeSinceLevelLoad)
			{
				TriggerWorldEvent(pendingWorldEvents[0]);
				pendingWorldEvents.RemoveAt(0);
			}
		}
	}

	private void TriggerWorldEvent(WorldEvent worldEvent)
	{
		switch (worldEvent.WorldEventType)
		{
			case WorldEventType.Tilt:
				TrainStation.Rotate(new Vector3(0, 0, worldEvent.Intensity.z));
				break;

			case WorldEventType.Gravity:
				Physics2D.gravity = worldEvent.Intensity;
				break;
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

	public void SpawnCrate(Cargo cargo)
	{
		var prefab = cargo == null ? DebugCratePrefab : cargo.CargoPrefab;
		var displacement = cargo == null ? Vector2.zero : cargo.Displacement;

		var newCargo = Instantiate(prefab, CrateSpawnPoint.parent);
		newCargo.transform.position = new Vector3(
			CrateSpawnPoint.transform.position.x + displacement.x,
			CrateSpawnPoint.transform.position.y + displacement.y,
			CrateSpawnPoint.transform.position.z);
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
		var newCar = Instantiate(car.CarPrefab, newTrain.transform);

		newCar.transform.position = new Vector3(
			newTrain.transform.transform.position.x - (i+1) * CarDisplacement.x,
			newTrain.transform.transform.position.y - CarDisplacement.y,
			newTrain.transform.transform.position.z);

		newCar.GetComponent<CarController>().Car = car;
	}
}
