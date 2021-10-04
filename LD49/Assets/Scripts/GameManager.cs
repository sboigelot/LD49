using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
	public GameObject DebugCratePrefab;
	public Transform CrateSpawnPoint;

	public Transform TrainSpawnPoint;
	public Vector2 CarDisplacement;

	public Transform VortexCenter;

	public Level DebugLevel;

	public Level CurrentLevel;

	public List<Cargo> PendingCargos;
	public List<Train> PendingTrains;
	public List<WorldEvent> PendingWorldEvents;

	public Text TimerText;

	public GameObject MainMenuScreenPrefab;
	public Transform ScreenPlaceholder;

	public GameObject SpellButtonPrefab;
	public Transform SpellGrid;
	public List<Spell> Spells;

	public Sprite SpellIconForcePursh;
	public Sprite SpellIconVortex;
	public Sprite SpellIconMeditate;
	public Sprite SpellIconWorldImpact;

	public int Score;
	public Text ScoreText;

	public Transform TrainStation;

	public GameObject FloatingTextPrefab;
	public Transform FloatingTextPlaceholder;

	public GameObject FloatingSpritePrefab;

	public Sprite Smoke;

	public GameObject EndLevelScreenPrefab;

	public ClockController Clock;
	public TiltGadgetController TiltGadget;

	public GameObject ForcePushAnim;

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
				ManaCost = 20,
				ForcePushAnim = ForcePushAnim
			},
			new VortexSpell {
				ActivationCode = KeyCode.Alpha3,
				DisplayName = "Vortex",
				Icon = SpellIconVortex,
				ManaCostPerSecond = 5,
			},
			new WorldImpactSpell{
				ActivationCode = KeyCode.Alpha4,
				DisplayName = "World Impact",
				Icon = SpellIconWorldImpact,
				ManaCost = 25,
				Intensity = new Vector3(0,3,5)
			},
		};

		BuildSpellListUi();

		Time.timeScale = 1f;
		SelectLevel(GameInfo.CurrentLevel);
	}

	public FloatingText SpawnFloatingText(Vector3 worldPosition, Color color, string text)
	{
		var viewportPosition = Camera.main.WorldToScreenPoint(worldPosition);
		return SpawnUiFloatintText(viewportPosition, color, text);
	}

	public FloatingText SpawnUiFloatintText(Vector2 screenPosition, Color color, string text)
	{
		var newFloatingText = Instantiate(FloatingTextPrefab, FloatingTextPlaceholder);

		//newFloatingText.transform.localPosition = viewportPosition;
		var rectTransform = newFloatingText.GetComponent<RectTransform>();
		rectTransform.position = screenPosition;

		var flotingText = newFloatingText.GetComponent<FloatingText>();
		flotingText.Text.color = color;
		flotingText.Text.text = text;

		return flotingText;
	}

	public FloatingText SpawnFloatingSprite(Vector3 worldPosition, float opacity, Sprite sprite)
	{
		var viewportPosition = Camera.main.WorldToScreenPoint(worldPosition);
		return SpawnUiFloatintSprite(viewportPosition, opacity, sprite);
	}

	public FloatingText SpawnUiFloatintSprite(Vector2 screenPosition, float opacity, Sprite sprite)
	{
		var newFloatingText = Instantiate(FloatingSpritePrefab, FloatingTextPlaceholder);

		//newFloatingText.transform.localPosition = viewportPosition;
		var rectTransform = newFloatingText.GetComponent<RectTransform>();
		rectTransform.position = screenPosition;

		var flotingText = newFloatingText.GetComponent<FloatingText>();
		flotingText.Image.color = new Color(1f, 1f, 1f, opacity);
		flotingText.Image.sprite = sprite;

		return flotingText;
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
		if (CurrentLevel != null)
		{
			Debug.LogError("Changing level is not implemented without reloading the scene");
			return;
		}

		CurrentLevel = level;

		if (CurrentLevel == null)
		{
			if (DebugLevel == null)
			{
				Debug.LogError("No Level selected in GameInfo.Level and no GameManager.Debuglevel set");
				return;
			}

			CurrentLevel = DebugLevel;
		}		

		PendingCargos = new List<Cargo>(CurrentLevel.Cargos).OrderBy(t => t.SpawnTimeInSecond).ToList();
		PendingTrains = new List<Train>(CurrentLevel.Trains).OrderBy(t => t.SpawnTimeInSecond).ToList();
		PendingWorldEvents = new List<WorldEvent>(CurrentLevel.WorldEvents).OrderBy(t => t.SpawnTimeInSecond).ToList();

		Clock.DurationInSecond = PendingWorldEvents.Last().SpawnTimeInSecond;
		Clock.RebuildClockExtrusions();
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

		if (PendingCargos.Any())
		{
			if (PendingCargos[0].SpawnTimeInSecond <= Time.timeSinceLevelLoad)
			{
				SpawnCrate(PendingCargos[0]);
				PendingCargos.RemoveAt(0);
			}
		}

		if (PendingTrains.Any())
		{
			if (PendingTrains[0].SpawnTimeInSecond <= Time.timeSinceLevelLoad)
			{
				SpawnTrain(PendingTrains[0]);
				PendingTrains.RemoveAt(0);
			}
		}

		if (PendingWorldEvents.Any())
		{
			if (PendingWorldEvents[0].SpawnTimeInSecond <= Time.timeSinceLevelLoad)
			{
				TriggerWorldEvent(PendingWorldEvents[0]);
				PendingWorldEvents.RemoveAt(0);
			}
		}
	}

	private void TriggerWorldEvent(WorldEvent worldEvent)
	{
		switch (worldEvent.WorldEventType)
		{
			case WorldEventType.Tilt:
				var intensity = worldEvent.Intensity.z;
				TiltWorld(intensity);
				break;

			case WorldEventType.Gravity:
				ChangeGravity(worldEvent.Intensity);
				break;

			case WorldEventType.EndGame:
				GameOver();
				break;
		}
	}

	private void GameOver()
	{
		OpenEndScreen();
	}

	public void ChangeGravity(Vector2 gravity)
	{
		Physics2D.gravity = gravity;
	}

	public void ImpactGravity(float updown)
	{
		Physics2D.gravity = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y + updown);
	}

	public void TiltWorld(float intensity)
	{
		TrainStation.Rotate(new Vector3(0, 0, intensity));
		TiltGadget.UpdateTilt(TrainStation.transform.rotation.z);
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

	public void OpenEndScreen()
	{
		var endScreen = Instantiate(EndLevelScreenPrefab, ScreenPlaceholder);
		endScreen.GetComponent<EndLevelScreen>().Open(CurrentLevel, Score);
		Time.timeScale = 0;
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
		var newTrain = new GameObject("Train", typeof(TrainController), typeof(Rigidbody2D));

		newTrain.transform.SetParent(TrainSpawnPoint);
		newTrain.transform.position = new Vector3(
			TrainSpawnPoint.transform.position.x,
			TrainSpawnPoint.transform.position.y,
			TrainSpawnPoint.transform.position.z);
		
		SpawnLocomotive(train, newTrain);

		var trainController = newTrain.GetComponent<TrainController>();
		trainController.Cars = new List<CarController>();
		trainController.Speed = train.Speed;
		var halfSizeOfScreenInUnityUnit = 35;
		trainController.DestroyXLocation = (train.Cars.Count + 1) * CarDisplacement.x + halfSizeOfScreenInUnityUnit;

		var rigidBody = newTrain.GetComponent<Rigidbody2D>();
		rigidBody.bodyType = RigidbodyType2D.Kinematic;
		rigidBody.gravityScale = 0f;

		for (int i = 0; i < train.Cars.Count; i++)
		{
			Car car = train.Cars[i];

			var carController = SpawnCar(car, i, newTrain);
			trainController.Cars.Add(carController);
		}
	}

	private void SpawnLocomotive(Train train, GameObject newTrain)
	{
		var newCrate = Instantiate(train.LocoPrefab, newTrain.transform);
		newCrate.transform.position = new Vector3(
			newTrain.transform.transform.position.x,
			newTrain.transform.transform.position.y,
			newTrain.transform.transform.position.z);
	}

	private CarController SpawnCar(Car car, int i, GameObject newTrain)
	{
		var newCar = Instantiate(car.CarPrefab, newTrain.transform);

		newCar.transform.position = new Vector3(
			newTrain.transform.transform.position.x - (i+1) * CarDisplacement.x,
			newTrain.transform.transform.position.y - CarDisplacement.y,
			newTrain.transform.transform.position.z);

		var controller = newCar.GetComponent<CarController>();
		controller.Car = car;

		return controller;
	}
}
