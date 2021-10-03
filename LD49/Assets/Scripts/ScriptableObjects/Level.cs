using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "TrainMage/Level")]
public class Level : ScriptableObject
{
	public string DisplayName;

	public Sprite Image;

	public int Order;

	public List<Train> Trains;

	public List<Crate> Crates;
}
