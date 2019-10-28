using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//cette classe regroupe les valeurs qui seront sauvegardées
[System.Serializable]
public class PlayerValues
{
    public int playerMoney, playerFood, playerDrinks;
    public float playerPopularity;
}

public class PlayerManager : MonoBehaviour, ISaveable
{
    #region /// VARIABLES ///

    public static PlayerManager instance;
    private PlayerValues playerValues = new PlayerValues(); 

    [SerializeField] private int playerTurnMoney, playerFoodCapacity, playerDrinksCapacity, playerCleanliness;
    public int PlayerMoney
    {
        get
        {
            return playerValues.playerMoney;
        }

        set
        {
            playerValues.playerMoney = value;
            UIManager.instance.playerMoney.statText.text = playerValues.playerMoney.ToString();
        }
    }
    public int PlayerCleanliness
    {
        get
        {
            return playerCleanliness;
        }

        set
        {
            playerCleanliness = value;
            playerCleanliness = Mathf.Clamp(playerCleanliness, 0, 100);

            UIManager.instance.playerCleanliness.statText.text = playerCleanliness.ToString();
            UIManager.instance.playerCleanliness.GhostFiller((float)playerCleanliness / 100);
        }
    }
    public float PlayerPopularity
    {
        get
        {
            return playerValues.playerPopularity;
        }

        set
        {
            playerValues.playerPopularity = value;
            playerValues.playerPopularity = Mathf.Clamp(playerValues.playerPopularity, 0, 100);

            UIManager.instance.playerPopularity.statText.text = ((int)playerValues.playerPopularity).ToString();
            UIManager.instance.playerPopularity.GhostFiller(playerValues.playerPopularity / 100);
        }
    }
    [HideInInspector] public int PlayerTurnMoney
    {
        get
        {
            return playerTurnMoney;
        }

        set
        {
            playerTurnMoney = value;
        }
    }
    public int PlayerFood
    {
        get
        {
            return playerValues.playerFood;
        }

        set
        {
            playerValues.playerFood = value;
            playerValues.playerFood = Mathf.Clamp(playerValues.playerFood, 0, PlayerFoodCapacity);

            if (playerValues.playerFood == 0) hasBoughtEnoughFood = false;
            else hasBoughtEnoughFood = true;
            UIManager.instance.playerFood.statText.text = playerValues.playerFood.ToString();
            UIManager.instance.playerFood.GhostFiller((float)playerValues.playerFood / (float)playerFoodCapacity);
        }
    }
    public int PlayerFoodCapacity
    {
        get
        {
            return playerFoodCapacity;
        }

        set
        {
            playerFoodCapacity = value;
            UIManager.instance.playerFood.capacityText.text = playerFoodCapacity.ToString();
            UIManager.instance.playerFood.GhostFiller((float)playerValues.playerFood / (float)playerFoodCapacity);
        }

    }
    public int PlayerDrinks
    {
        get
        {
            return playerValues.playerDrinks;
        }

        set
        {
            playerValues.playerDrinks = value;
            playerValues.playerDrinks = Mathf.Clamp(playerValues.playerDrinks, 0, PlayerDrinksCapacity);

            if (playerValues.playerDrinks == 0) hasBoughtEnoughDrinks = false;
            else hasBoughtEnoughDrinks = true;
            UIManager.instance.playerDrinks.statText.text = playerValues.playerDrinks.ToString();
            UIManager.instance.playerDrinks.GhostFiller((float)playerValues.playerDrinks / (float)playerDrinksCapacity);
        }
    }
    public int PlayerDrinksCapacity
    {
        get
        {
            return playerDrinksCapacity;
        }

        set
        {
            playerDrinksCapacity = value;
            UIManager.instance.playerDrinks.capacityText.text = playerDrinksCapacity.ToString();
            UIManager.instance.playerDrinks.GhostFiller((float)playerValues.playerDrinks / (float)playerDrinksCapacity);
        }

    }

    [HideInInspector] public int clientsMoney, furnitureMoney, employeeMoney, restockMoney, thiefMoney, unhappyClients = 0;

    #region Offline Room Properties

    public int foodPrice = 2, drinkPrice = 2;

    #endregion

    [HideInInspector] public bool hasBoughtEnoughDrinks = false, hasBoughtEnoughFood = false;

    public Tavern tavern;
    public Transform spawnPoint;

    public float cameraZoomLimit = 17.51015f;

    public NavMeshSurface surface;
    [SerializeField] private GameObject[] navmeshObstacles;
    #endregion

    private void Awake()
    {
        instance = this;
        AddListeners();
    }

    void Start()
    {
        var temp = Instantiate(GameManager.god.tavern, spawnPoint.transform.position, spawnPoint.transform.rotation);

        tavern = temp.GetComponent<Tavern>();

        //On update la navmesh
        surface.BuildNavMesh();
        tavern.BakeTavernSurfaces();
        foreach (var item in navmeshObstacles)
        {
            item.SetActive(false);
        }

        // On place la caméra de sorte qu'elle focus la taverne
        CameraManager.instance.InitCameraPosition(spawnPoint);
    }

    public void AddListeners()
    {
        GameSaver.instance.savingGame.AddListener(SaveValues);
        GameSaver.instance.loadingPlayer.AddListener(LoadValues);
    }
    public void SaveValues()
    {
        GameSaver.instance.gameSave.playerValues = playerValues;
    }
    public void LoadValues()
    {
        playerValues = GameSaver.instance.gameSave.playerValues;

        PlayerMoney = playerValues.playerMoney;
        PlayerFood = playerValues.playerFood;
        PlayerDrinks = playerValues.playerDrinks;
        PlayerPopularity = playerValues.playerPopularity;
        PlayerCleanliness = 100;

        tavern.UpdateShelvesProps();
    }

    //fonction qui permet au joueur de se restocker en nourriture et/ou boissons
    public void Restock(int foodAmount, int drinkAmount)
    {
        AlertPanel.instance.GenerateAlert(Alert.AlertType.Delivery, foodAmount.ToString(), drinkAmount.ToString());

        PlayerFood += foodAmount;

        PlayerDrinks += drinkAmount;

        if (UIManager.instance.currentMenu == UIManager.instance.restockMenu) UIManager.instance.restockMenu.UpdateContent();
        tavern.UpdateShelvesProps();
    }

    //fonction qui permet au joueur de gagner de l'argent selon le plat que le client à consommé
    public int GainMoney(Dish dish)
    {
        int moneyGained = dish.FinalPriceCalculator();
        UIManager.instance.blackPanel.recapPanel.clientsTotalValue += moneyGained;
        PlayerMoney += moneyGained;
        clientsMoney += moneyGained;
        return moneyGained;
    }

    //fonction qui permet de vérifier si le joueur peut se permettre de dépenser de l'argent
    public bool CanSpendMoney(int amount)
    {
        if (amount <= PlayerMoney) return true;
        else
        {
            UIManager.instance.playerMoney.StatFlickerRed();
            return false;
        }
    }
}
