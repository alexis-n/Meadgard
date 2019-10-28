using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : ScriptableObject
{
    [HideInInspector] public enum BanditMission { StealFood, StealDrinks, StealMoney };
    public BanditMission mission;
    [HideInInspector] public enum BanditState { Wanted, Caught, Escaped };
    public BanditState state;
    public int bounty = 0, amountStolen = 0;
    private int minBountyAmount = 20, maxBountyAmount = 100;
    public float willSpawnAt = 0f;

    private void OnEnable()
    {
        mission = (BanditMission)Random.Range(0, 2);
        state = BanditState.Wanted;
        bounty = Random.Range(minBountyAmount, maxBountyAmount);
        willSpawnAt = Random.Range(0, PhaseManager.instance.serviceDuration);
    }

    public void Mission(Vector3 worldPosition)
    {
        switch (mission)
        {
            case BanditMission.StealFood:
                amountStolen = PlayerManager.instance.PlayerFood;
                PlayerManager.instance.PlayerFood -= amountStolen;
                UIManager.instance.RessourcePopup(worldPosition, -amountStolen, UIManager.instance.foodColor, UIManager.instance.foodSprite);
                AlertPanel.instance.GenerateAlert(Alert.AlertType.EnnemyThief, (amountStolen + " de nourriture"));
                break;
            case BanditMission.StealDrinks:
                amountStolen = PlayerManager.instance.PlayerDrinks;
                PlayerManager.instance.PlayerDrinks -= amountStolen;
                UIManager.instance.RessourcePopup(worldPosition, -amountStolen, UIManager.instance.drinkColor, UIManager.instance.drinkSprite);
                AlertPanel.instance.GenerateAlert(Alert.AlertType.EnnemyThief, (amountStolen + " d'hydromel"));
                break;
            case BanditMission.StealMoney:
                amountStolen = Random.Range(0, PlayerManager.instance.PlayerMoney);
                PlayerManager.instance.PlayerMoney -= amountStolen;
                UIManager.instance.RessourcePopup(worldPosition, -amountStolen, UIManager.instance.silverColor);
                AlertPanel.instance.GenerateAlert(Alert.AlertType.EnnemyThief, (amountStolen + " d'argent"));
                break;
        }
    }
}
