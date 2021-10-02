using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    public GameObject CratePrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            SpawnCrate();
        }
        
    }

	private void SpawnCrate()
	{
        var newCrate = Instantiate(CratePrefab, transform, false);

	}
}
