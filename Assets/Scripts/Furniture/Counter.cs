using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Furniture
{

    #region ///  VARIABLES  ///
    public List<CounterSpace> freeSpaces;
    public List<CounterSpace> takenSpaces;
    #endregion

    //Fonction qui override la fonction Build de base en y ajoutant des actions spécifiques à Counter
    public override void Build()
    {
        base.Build();
        CounterSpawner();
    }

    //Fonction qui override la fonction Upgrade de base en y ajoutant des actions spécifiques à Table
    public override void Upgrade()
    {
        base.Upgrade();
        CounterSpawner();
    }

    //Fonction qui fait spawner les comptoirs appropriés et gère leurs espaces associés
    public void CounterSpawner()
    {
        name = furnitureData.namePerLevel[level];
        //on n'active que la table concerné et désactivant toutes les autres
        for (int i = 0; i < visuals.Length; i++)
        {
            if (i == level - 1)
            {
                visuals[i].SetActive(true);
                CounterSpace[] temp = visuals[i].GetComponentsInChildren<CounterSpace>();
                for (int o = 0; o < temp.Length; o++)
                {
                    freeSpaces.Add(temp[o]);
                    temp[o].counter = this;
                }
            }
            else
            {
                CounterSpace[] temp = visuals[i].GetComponentsInChildren<CounterSpace>();
                for (int o = 0; o < temp.Length; o++)
                {
                    freeSpaces.Remove(temp[o]);
                }
                visuals[i].SetActive(false);
            }
        }

        if (SelectionManager.instance.selectedObject == this) UIManager.instance.selectionPanel.CounterUpdate();
    }

    //Donne une place sur le comptoir
    public CounterSpace GiveSpace()
    {
        CounterSpace temp = freeSpaces[0];
        SwitchServingPlace(temp);
        return temp;
    }

    //Fonction qui libère ou réserve des places sur le comptoir
    public void SwitchServingPlace(CounterSpace servingPlaceToSwitch)
    {
        if (freeSpaces.Contains(servingPlaceToSwitch))
        {
            freeSpaces.Remove(servingPlaceToSwitch);
            takenSpaces.Add(servingPlaceToSwitch);
            if (freeSpaces.Count <= 0) SelectionManager.instance.ShowFreeCounters.RemoveListener(SelectableVisual);
        }

        else if (takenSpaces.Contains(servingPlaceToSwitch))
        {
            servingPlaceToSwitch.dish = null;
            takenSpaces.Remove(servingPlaceToSwitch);
            freeSpaces.Add(servingPlaceToSwitch);
            if (freeSpaces.Count > 0) SelectionManager.instance.ShowFreeCounters.AddListener(SelectableVisual);
        }
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
            if (level != maxLevel)
            {
                if (level == 0) ghosts[0].SetActive(true);
                else ghosts[1].SetActive(true);
            }
        }
    }

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Counter
    public override void Select()
    {
        base.Select();

        //On indique au panneau de sélection de s'ouvrir avec les valeurs de ce comptoir
        UIManager.instance.selectionPanel.CounterContent(this);
    }

    #region ///  VIEUX CODE  ///
    /*
    #endregion
    public class Counter : Furniture {

    public List<GameObject> freeSpaces = new List<GameObject>(), takenSpaces = new List<GameObject>();
    public GameObject[] level1Spaces, level2Spaces, level3Spaces;

    // Use this for initialization
    private void Start()
    {
        upgrade.AddListener(UpgradeCounter);
        base.Initialization();
        type = Data.FurnitureType.Counter;

        name = "Espace pour table";

        if (mustSpawnAtStart)
        {
            UpgradeCounter();
        }
        else
        {
            spawner.SetActive(true);
        }
    }

    public void GiveSpaceToCook (Cook cook)
    {
        GameObject temp = freeSpaces[Random.Range(0, freeSpaces.Count)];
        freeSpaces.Remove(temp);
        takenSpaces.Add(temp);

        cook.counter = this.gameObject;
        cook.reservedServingPlace = temp;

        if (freeSpaces.Count == 0) full = true;
    }

    public void FreeSpace(GameObject spaceToFree)
    {
        takenSpaces.Remove(spaceToFree);
        freeSpaces.Add(spaceToFree);
        full = false;
    }

    public void UpgradeCounter()
    {
        if (ServiceManager.instance.PlayerMoney >= upgradePrice)
        {
            switch (level)
            {
                case 0:
                    name = "Petit comptoir";
                    selectableObject.canBeSelectedDuringService = true;
                    level1.SetActive(true);
                    spawner.SetActive(false);
                    if (!mustSpawnAtStart)
                    {
                        ServiceManager.instance.PlayerMoney -= upgradePrice;
                        UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    }
                    for (int i = 0; i < level1Spaces.Length; i++)
                    {
                        freeSpaces.Add(level1Spaces[i]);
                    }
                    ServiceManager.instance.tavern.counters.Add(this);
                    upgradePrice = level2Price;
                    break;
                case 1:
                    name = "Comptoir";
                    level2.SetActive(true);
                    ServiceManager.instance.PlayerMoney -= upgradePrice;
                    UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    for (int i = 0; i < level2Spaces.Length; i++)
                    {
                        freeSpaces.Add(level2Spaces[i]);
                    }
                    upgradePrice = level3Price;
                    break;
                case 2:
                    name = "Grand comptoir";
                    level3.SetActive(true);
                    ServiceManager.instance.PlayerMoney -= upgradePrice;
                    UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    for (int i = 0; i < level3Spaces.Length; i++)
                    {
                        freeSpaces.Add(level3Spaces[i]);
                    }
                    break;
            }
            level++;
            UIManager.instance.UpdateFurniturePanel(this);
        }
        else UIManager.instance.FlickerRed(UIManager.instance.money);
    }
    */
    #endregion
}
