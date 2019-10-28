using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bard : Employee
{
    public static bool generatePopularity = false;
    public GameObject[] instruments;

    // Use this for initialization 
    new void Awake()
    {
        base.Awake();
    }

    new void Update()
    {
        base.Update();
    }

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Waitress
    public override void Select()
    {
        base.Select();
        //On indique au panneau de sélection de s'ouvrir avec les valeurs de ce plat
        UIManager.instance.selectionPanel.BardContent(this);

        UpdateSelection();
    }
}
