using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay", menuName = "Gameplay/General Data")]
public class GameplayData : ScriptableObject
{
    public God[] gods;

    public float straightToStageBaseChance = 0f;
    [Tooltip("1st value= human, 2nd value= dwarf, 3rd value= elf, 4th value= jottun")]
    public float[] raceChances, rarityChances;
    public Color[] rarityColors;
    public int unhappyThreshold = 25, happyThreshold = 25, unhappyClientsBeforeFight = 5, removedPopularityPerFighter = 10, moneyToPayForKicking = 20;
    public Vector3 minMinmaxMaxClientSpawnTime;

    public int[] tablesLevel = new int[5],
        countersLevel = new int[3],
        shelvesLevel = new int[3],
        furnacesLevel = new int[3];

    public List<EmployeeValues> startingWaitresses = new List<EmployeeValues>(),
        startingCooks = new List<EmployeeValues>(),
        startingBouncers = new List<EmployeeValues>(),
        startingBards = new List<EmployeeValues>();

    public ThiefValues[] startingThieves = new ThiefValues[3];

    public int startAmountMoney = 100;
    public int startAmountFood = 5;
    public int startAmountDrinks = 5;
    public int startAmountPopularity = 25;
}
