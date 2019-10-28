using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : Furniture
{
    #region ///  VARIABLES  ///
    [SerializeField] private GameObject[] drinkVisuals;
    [SerializeField] private ParticleSystem[] foodEffects;
    [SerializeField] private Transform[] foodCoordinates, drinkCoordinates;
    public Transform cookSpawner;
    public float[] freshnessMultipliers;
    [HideInInspector] public bool taken = false;
    #endregion

    //Fonction qui override la fonction Build de base en y ajoutant des actions spécifiques à Table
    public override void Build()
    {
        ToggleProp(false);
        interactable = true;
        base.Build();
        if (level == 0) level = 1;
        FurnaceSpawner();
    }

    //Fonction qui override la fonction Upgrade de base en y ajoutant des actions spécifiques à Table
    public override void Upgrade()
    {
        base.Upgrade();
        FurnaceSpawner();
    }

    //Fonction qui fait spawner le four approprié
    public void FurnaceSpawner()
    {
        name = furnitureData.namePerLevel[level];
        //on n'active que le four concerné et désactivant toutes les autres
        for (int i = 0; i < visuals.Length; i++)
        {
            if (i == level - 1) visuals[i].SetActive(true);
            else visuals[i].SetActive(false);
        }

        if (SelectionManager.instance.selectedObject == this) UIManager.instance.selectionPanel.UpdateFurnace();
    }

        //Fonction qui cache le spawner de l'objet quand on passe en phase de service et que l'object est de niveau 0
    public override void HideSpawner()
    {
        if (visuals.Length != 0)
        {
            foreach(var item in ghosts)
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
        UIManager.instance.selectionPanel.FurnaceContent(this);
    }

    //Fonction qui toggle le prop approprié
    public void ToggleProp(bool toggle, Cook cook = null, Dish dish = null)
    {
        interactable = !toggle;
        if (SelectionManager.instance.selectedObject == this) Unselect();

        for (int i = 0; i < maxLevel; i++)
        {
            foodEffects[i].Stop();
            drinkVisuals[i].SetActive(false);
        }

        if(toggle)
        {
            switch (dish.dishData.order.ressourceType)
            {
                case Data.RessourceType.Food:
                    foodEffects[level - 1].Play();
                    dish.gameObject.transform.position = foodCoordinates[level - 1].position;
                    dish.transform.parent = foodCoordinates[level - 1];
                    cook.gameObject.transform.LookAt(foodCoordinates[level - 1].position);
                    break;
                case Data.RessourceType.Drink:
                    drinkVisuals[level - 1].SetActive(true);
                    dish.gameObject.transform.position = drinkCoordinates[level - 1].position;
                    dish.transform.parent = drinkCoordinates[level - 1];
                    cook.gameObject.transform.LookAt(drinkCoordinates[level - 1].position);
                    break;
            }
        }
    }
}
