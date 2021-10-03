using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    public bool IsSelected;

    public GameObject SelectionSprite;

    void Update()
    {
        SelectionSprite.SetActive(IsSelected);
    }
}
