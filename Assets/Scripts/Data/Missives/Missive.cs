using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Missive", menuName = "Gameplay/New Missive", order = 4)]
public class Missive : ScriptableObject
{
    [HideInInspector] public static Missive currentMissive, predictedMissive;
    [HideInInspector] public static Missive[] odinMissives = new Missive[3];

    [Header("General Info")]
    public Sprite missiveIcon;
    public Color missiveColor;
    public string missiveName;
    [TextArea] public string missiveDescription;
    [Space(7)]

    [Header("Stock")]
    public float foodRestockPrice = 1;
    public float drinkRestockPrice = 1, deliveryPrice = 1,
        foodRotting = 1, drinkRotting = 1;
    [Space(7)]

    [Header("Clients")]
    public float overallSpawnRate = 1;
    public float humanSpawnRate = 1;
    public float dwarfSpawnRate = 1;
    public float elfSpawnRate = 1;
    public float jottunSpawnRate = 1;
    [Space(7)]

    [Header("Employees")]
    public float waitressFee = 1;
    public float cookFee = 1, bouncerFee = 1, bardFee = 1,
        waitressSalary = 1, cookSalary = 1, bouncerSalary = 1, bardSalary = 1;
    [Space(7)]

    [Header("Furniture")]
    public float furnitureUpgradingPrice = 1;
}
