using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : Furniture {

    #region ///  VARIABLES  ///
    [SerializeField] Chair[] level1Chairs, level2Chairs, level3Chairs;
    private List<Chair[]> chairs = new List<Chair[]>();

    public List<Client> clientsOrdering;

    public bool takenCareOf = false;
    #endregion

    //Fonction 

    //Fonction qui override la fonction Build de base en y ajoutant des actions spécifiques à Table
    public override void Build()
    {
        interactable = true;

        chairs.Add(level1Chairs);
        chairs.Add(level2Chairs);
        chairs.Add(level3Chairs);

        base.Build();
        if (level != 0)
            SelectionManager.instance.ShowOrderingTables.AddListener(SelectableVisual);
        TableSpawner();
    }

    //Fonction qui override la fonction Upgrade de base en y ajoutant des actions spécifiques à Table
    public override void Upgrade()
    {
        base.Upgrade();
        TableSpawner();
    }

    //Fonction qui fait spawner la table appropriée et gère les chaises associées
    public void TableSpawner()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        gameObject.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        name = furnitureData.namePerLevel[level];

         //on n'active que la table concerné et désactivant toutes les autres
        for (int i = 0; i < visuals.Length; i++)
        {
            if (i == level - 1)
            {
                visuals[i].SetActive(true);
                foreach (Chair chair in chairs[i])
                {
                    PlayerManager.instance.tavern.availableRestaurantChairs.Add(chair);
                    chair.table = this;
                }
            }
            else
            {
                visuals[i].SetActive(false);
                foreach (Chair chair in chairs[i])
                {
                    PlayerManager.instance.tavern.availableRestaurantChairs.Remove(chair);
                }
            }
        }

        if (SelectionManager.instance.selectedObject == this) UIManager.instance.selectionPanel.TableUpdate();
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

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Table
    public override void Select()
    {
        base.Select();

        //On indique au panneau de sélection de s'ouvrir avec les valeurs de cette table
        UIManager.instance.selectionPanel.TableContent(this);
    }

    //fonction qui active les visuels d'intéragibilité
    public override void SelectableVisual(Color newColor, bool toggle, string tooltipRightClick)
    {
        if (CheckIfInteractable() && toggle && clientsOrdering.Count > 0 && !takenCareOf)
        {
            outline.enabled = true;

            if (outline.OutlineColor.r != newColor.r &&
               outline.OutlineColor.g != newColor.g &&
               outline.OutlineColor.b != newColor.b)
                outline.OutlineColor = new Color(newColor.r, newColor.g, newColor.b, 0.25f);
            outline.OutlineWidth = 5f;

            keepOutlineOn = true;

            tooltipable.rightClickColor = newColor;
            tooltipable.rightClick = tooltipRightClick;
        }
        else if (!toggle)
        {
            outline.OutlineColor = data.color;
            outline.enabled = false;
            keepOutlineOn = false;

            tooltipable.rightClick = string.Empty;
        }
    }

    #region ///  VIEUX CODE  ///
    /*
     public class TableManager : Furniture {

    Table table4, table6, table8;

     public GameObject[] table1DirtyProps, table2DirtyProps, table3DirtyProps;
    [Range(0, 100)]
    public int dirtyThreshold = 75;
    [Range(0,100)]
    private int cleanliness = 100;
    public int Cleanliness
    {
        get
        {
            return cleanliness;
        }
        set
        {
            cleanliness = value;

            if(cleanliness < 100) SelectionManager.instance.ShowDirtyTables.AddListener(SelectableVisual);
            else SelectionManager.instance.ShowDirtyTables.RemoveListener(SelectableVisual);

            if (cleanliness <= dirtyThreshold) PlayerManager.instance.tavern.dirtyTables.Add(this);

            switch(level)
            {
                case 1:
                    for (int i = 0; i < table1DirtyProps.Length; i++)
                    {
                        if (cleanliness <= (100 - ((i + 1) * 10))) table1DirtyProps[i].SetActive(true);
                        else table1DirtyProps[i].SetActive(false);
                    }
                    break;
                case 2:
                    for (int i = 0; i < table2DirtyProps.Length; i++)
                    {
                        if (cleanliness <= (100 - ((i + 1) * 10))) table2DirtyProps[i].SetActive(true);
                        else table2DirtyProps[i].SetActive(false);
                    }
                    break;
                case 3:
                    for (int i = 0; i < table3DirtyProps.Length; i++)
                    {
                        if (cleanliness <= (100 - ((i + 1) * 10))) table3DirtyProps[i].SetActive(true);
                        else table3DirtyProps[i].SetActive(false);
                    }
                    break;
            }
        }
    }

	// Use this for initialization
	void Start () {
        upgrade.AddListener(UpgradeTable);
        base.Initialization();
        table4 = level1.GetComponent<Table>();
        table6 = level2.GetComponent<Table>();
        table8 = level3.GetComponent<Table>();

        name = "Espace pour table";
        sprite = UIManager.instance.bank.tableSprite;

        if (mustSpawnAtStart)
        {
            UpgradeTable();
        }
        else
        {
            spawner.SetActive(true);
        }


        gameObject.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
	}
	
	public void UpgradeTable()
    {
        if (ServiceManager.instance.PlayerMoney >= upgradePrice)
        {
            switch (level)
            {
                case 0:
                    name = "Table de 4";
                    selectableObject.canBeSelectedDuringService = true;
                    level1.SetActive(true);
                    ServiceManager.instance.tavern.tables.Add(table4);
                    if (!mustSpawnAtStart)
                    {
                        ServiceManager.instance.PlayerMoney -= upgradePrice;
                        UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    }
                    spawner.SetActive(false);
                    upgradePrice = level2Price;
                    break;
                case 1:
                    name = "Table de 6";
                    level1.SetActive(false);
                    ServiceManager.instance.tavern.tables.Remove(table4);
                    ServiceManager.instance.PlayerMoney -= upgradePrice;
                    UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    level2.SetActive(true);
                    ServiceManager.instance.tavern.tables.Add(table6);
                    upgradePrice = level3Price;
                    break;
                case 2:
                    name = "Table de 8";
                    level2.SetActive(false);
                    ServiceManager.instance.tavern.tables.Remove(table6);
                    ServiceManager.instance.PlayerMoney -= upgradePrice;
                    UIManager.instance.OpenMoneyVariation(-upgradePrice);
                    level3.SetActive(true);
                    ServiceManager.instance.tavern.tables.Add(table8);
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
