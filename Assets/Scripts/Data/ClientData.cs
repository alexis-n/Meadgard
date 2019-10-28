using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClientData", menuName = "Data/Client")]
public class ClientData : InteractableObjectData
{
    [Header("Variables")]
    public Data.Race race;
    public Order[] allOrders, banditOrders;
    public Material banditMaterial;
    public Sprite question;
    public AudioClip[] talksGood;
    public AudioClip[] talksBad;
    public AudioClip[] eat;
    public AudioClip[] drink;
    public AudioClip[] thiefEscape;
    public AudioClip[] thiefKick;

}
