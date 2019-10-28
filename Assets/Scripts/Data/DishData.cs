using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DishData", menuName = "Data/Dish")]
public class DishData : InteractableObjectData
{
    [Header("Variables")]
    public Order order;
    public float timeBeingFresh, timeBetweenStates = 30f;
}
