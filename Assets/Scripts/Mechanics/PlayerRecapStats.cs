using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRecapStats : MonoBehaviour {

    public Text playertavern, playerCount, moneyCount, clientCount;
    public Image fill;

    public void UpdateStats(string tavern, int playerNumber, int money, int clients, Color tavernColor)
    {
        playertavern.text = tavern;
        playerCount.text = ("Player " + playerNumber);
        moneyCount.text = money.ToString();
        clientCount.text = clients.ToString();
        fill.color = tavernColor;
    }
}
