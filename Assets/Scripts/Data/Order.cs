using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Order", menuName = "Gameplay/New Order", order = 1)]
public class Order : ScriptableObject
{
    public new string name;
    public Data.RessourceType ressourceType;
    public Data.RessourceDetail ressourceDetail;

    public int ressourceAmount, ressourcePrice;
    public GameObject prefab;
    public Sprite ressourceSprite;
    public Color ressourceColor;
}