using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureData", menuName = "Data/Furniture")]
public class FurnitureData : InteractableObjectData
{
    [Header("Variables")]
    public int upgradePrice = 20;
    [Space(7)]

    [Header("UI")]
    public string[] namePerLevel;
}
