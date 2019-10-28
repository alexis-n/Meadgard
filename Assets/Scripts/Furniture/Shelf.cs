using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : Furniture
{
    #region ///  VARIABLES  ///
    public int addedCapacityPerLevel;
    [SerializeField] private GameObject[] level1Props, level2Props, level3Props;
    public int numberOfPropsShown;
    #endregion

    //Fonction qui override la fonction Build de base en y ajoutant des actions spécifiques à Counter
    public override void Build()
    {
        base.Build();
        ShelfSpawner();
        selectable = true;
    }

    //Fonction qui override la fonction Upgrade de base en y ajoutant des actions spécifiques à Table
    public override void Upgrade()
    {
        base.Upgrade();
        ShelfSpawner();
    }

    //Fonction qui fait spawner les étagères appropriées et gère leurs espaces associés
    public void ShelfSpawner()
    {
        name = furnitureData.namePerLevel[level];
        if (level != 0)
        {
            PlayerManager.instance.PlayerFoodCapacity += addedCapacityPerLevel;
            PlayerManager.instance.PlayerDrinksCapacity += addedCapacityPerLevel;
        }
        switch (level)
        {
            case 1:
                PlayerManager.instance.tavern.shelfProps.AddRange(level1Props);
                break;
            case 2:
                PlayerManager.instance.tavern.shelfProps.AddRange(level2Props);
                break;
            case 3:
                PlayerManager.instance.tavern.shelfProps.AddRange(level3Props);
                break;
        }

        //on commence par désactiver tout les visuels
        for (int i = 0; i < visuals.Length; i++)
        {
            visuals[i].SetActive(false);

            //si on est au niveau zero, on n'active que le spawner
            if (i <= level - 1)
            {
                visuals[i].SetActive(true);
            }
        }

        if (SelectionManager.instance.selectedObject == this) UIManager.instance.selectionPanel.UpdateShelf();
    }

    //Fonction qui cache le spawner de l'objet quand on passe en phase de service et que l'object est de niveau 0
    public override void HideSpawner()
    {
        if (visuals.Length != 0)
        {
            foreach (var item in ghosts)
            {
                item.SetActive(false);
            }
        }
    }

    //Fonction qui affiche le spawner de l'objet quand on passe en phase de préparation et que l'object est de niveau 0
    public override void ShowSpawner()
    {
        if (visuals.Length != 0)
        {
            for (int i = 0; i < maxLevel; i++)
            {
                if (i > level - 1) ghosts[i].SetActive(true);
            }
        }
    }

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Shelf
    public override void Select()
    {
        base.Select();

        //On indique au panneau de sélection de s'ouvrir avec les valeurs de ce comptoir
        UIManager.instance.selectionPanel.ShelfContent(this);
    }
}

    #region ///  VIEUX CODE  ///
    /*
    public class Shelf : Furniture
    {

    [SerializeField]
    int level1Capacity, level2Capacity, level3Capacity;

    // Use this for initialization
    void Start()
    {
        upgrade.AddListener(UpgradeShelf);
        base.Initialization();
        name = "Espace pour etagere";
        sprite = UIManager.instance.bank.prepSprite;

        upgradePrice = level1Price;
        if (mustSpawnAtStart)
        {
            UpgradeShelf();
        }
        else
        {
            spawner.SetActive(true);
        }
    }

    public void UpgradeShelf()
    {
        if (ServiceManager.instance.PlayerMoney >= upgradePrice)
        {
            switch (level)
            {
                case 0:
                    name = "Petite etagere";
                    selectableObject.canBeSelectedDuringService = true;
                    level1.SetActive(true);
                    spawner.SetActive(false);
                    if (!mustSpawnAtStart)
                    {
                        ServiceManager.instance.PlayerMoney -= upgradePrice;
                        UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    }
                    ServiceManager.instance.PlayerFoodCapacity += level1Capacity;
                    ServiceManager.instance.PlayerDrinksCapacity += level1Capacity;
                    upgradePrice = level2Price;
                    break;
                case 1:
                    name = "Etagere";
                    level2.SetActive(true);
                    ServiceManager.instance.PlayerMoney -= upgradePrice;
                    UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    ServiceManager.instance.PlayerFoodCapacity += level2Capacity;
                    ServiceManager.instance.PlayerDrinksCapacity += level2Capacity;
                    upgradePrice = level3Price;
                    break;
                case 2:
                    name = "Grande etagere";
                    level3.SetActive(true);
                    ServiceManager.instance.PlayerMoney -= upgradePrice;
                    UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    ServiceManager.instance.PlayerFoodCapacity += level3Capacity;
                    ServiceManager.instance.PlayerDrinksCapacity += level3Capacity;
                    break;
            }
            level++;
            UIManager.instance.UpdateFurniturePanel(this);
        }
        else UIManager.instance.FlickerRed(UIManager.instance.money);
    }
    }
    */
    #endregion
