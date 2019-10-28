using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmployeesMenu : Menu, ITutorialable
{
    #region /// VARIABLES ///
    [SerializeField] private List<TutorialStep> steps = new List<TutorialStep>();

    [SerializeField] private EmployeeMenuButton[] waitressCards, cookCards, bouncerCards, bardCards;
    #endregion

    private void Awake()
    {
        if (content.activeSelf) content.SetActive(false);
    }

    //Fonction qui ouvre ou ferme le menu de restockage
    public override void OnOpening()
    {
        base.OnOpening();

        for (int i = 0; i < waitressCards.Length; i++)
        {
            if (NPCManager.instance.hiredWaitresses.Count > i)
            {
                waitressCards[i].content.SetActive(true);
                waitressCards[i].Initilialization(NPCManager.instance.hiredWaitresses[i]);
            }
            else waitressCards[i].content.SetActive(false);
        }

        for (int i = 0; i < cookCards.Length; i++)
        {
            if (NPCManager.instance.hiredCooks.Count > i)
            {
                cookCards[i].content.SetActive(true);
                cookCards[i].Initilialization(NPCManager.instance.hiredCooks[i]);
            }
            else cookCards[i].content.SetActive(false);
        }

        for (int i = 0; i < bouncerCards.Length; i++)
        {
            if (NPCManager.instance.hiredBouncers.Count > i)
            {
                bouncerCards[i].content.SetActive(true);
                bouncerCards[i].Initilialization(NPCManager.instance.hiredBouncers[i]);
            }
            else bouncerCards[i].content.SetActive(false);
        }

        for (int i = 0; i < bardCards.Length; i++)
        {
            if (NPCManager.instance.hiredBards.Count > i)
            {
                bardCards[i].content.SetActive(true);
                bardCards[i].Initilialization(NPCManager.instance.hiredBards[i]);
            }
            else bardCards[i].content.SetActive(false);
        }
    }

    public void StartTutorial()
    {
        if (!foldToggle) FoldAndUnfold();
        Tutorial.instance.StartTutorial(transform, steps);
    }

    //Fonction qui met à jour le menu de restockage
    public override void UpdateContent()
    {
        for (int i = 0; i < waitressCards.Length; i++)
        {
            if (NPCManager.instance.hiredWaitresses.Count > i)
            {
                waitressCards[i].content.SetActive(true);
                waitressCards[i].UpdateContent();
            }
            else waitressCards[i].content.SetActive(false);
        }

        for (int i = 0; i < cookCards.Length; i++)
        {
            if (NPCManager.instance.hiredCooks.Count > i)
            {
                cookCards[i].content.SetActive(true);
                cookCards[i].UpdateContent();
            }
            else cookCards[i].content.SetActive(false);
        }

        for (int i = 0; i < bouncerCards.Length; i++)
        {
            if (NPCManager.instance.hiredBouncers.Count > i)
            {
                bouncerCards[i].content.SetActive(true);
                bouncerCards[i].UpdateContent();
            }
            else bouncerCards[i].content.SetActive(false);
        }

        for (int i = 0; i < bardCards.Length; i++)
        {
            if (NPCManager.instance.hiredBards.Count > i)
            {
                bardCards[i].content.SetActive(true);
                bardCards[i].UpdateContent();
            }
            else bardCards[i].content.SetActive(false);
        }
    }

    #region /// VIEUX CODE ///
    /*
     public void ToggleEmployeeMenu()
    {
        if (employeeMenuTrigger) CloseEmployeeMenu();
        else OpenEmployeeMenu();
    }
    public void OpenEmployeeMenu()
    {
        employeeMenuToggle.onClick.Invoke();
        employeeMenuScrollbar.size = 0;
        hireMenuScrollbar.size = 0;
        employeeMenuTrigger = true;
        employeeMenu.SetActive(true);
        UpdateEmployeeMenus();
    }
    public void CloseEmployeeMenu()
    {
        employeeMenuTrigger = false;
        employeeMenu.SetActive(false);
        Tooltip();
    }
    public void UpdateEmployeeMenus()
    {
        UpdateEmployeeMenu();
        UpdateHireMenu();
    }
    public void UpdateEmployeeMenu()
    {
        for (int i = 0; i < employeeButtons.Count; i++)
        {
            Destroy(employeeButtons[i]);
        }

        for (int i = 0; i < ServiceManager.instance.waitresses.Count; i++)
        {
            var temp = Instantiate(employeeButtonPrefab, waitressListParent.transform);
            //temp.GetComponent<EmployeeMenuButton>().Initilialization(ServiceManager.instance.waitresses[i]);
            employeeButtons.Add(temp);
        }

        for (int i = 0; i < ServiceManager.instance.cooks.Count; i++)
        {
            var temp = Instantiate(employeeButtonPrefab, cookListParent.transform);
            //temp.GetComponent<EmployeeMenuButton>().Initilialization(ServiceManager.instance.cooks[i]);
            employeeButtons.Add(temp);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(employeeContentPanel);
    }
    public void UpdateHireMenu()
    {
        for (int i = 0; i < hireButtons.Count; i++)
        {
            Destroy(hireButtons[i]);
        }

        for (int i = 0; i < ServiceManager.instance.hiredWaitresses.Count; i++)
        {
            var temp = Instantiate(hireButtonPrefab, hireWaitressListParent.transform);
            temp.GetComponent<HireMenuButton>().Initilialization(ServiceManager.instance.hiredWaitresses[i]);
            hireButtons.Add(temp);
        }

        for (int i = 0; i < ServiceManager.instance.hiredCooks.Count; i++)
        {
            var temp = Instantiate(hireButtonPrefab, hireCookListParent.transform);
            temp.GetComponent<HireMenuButton>().Initilialization(ServiceManager.instance.hiredCooks[i]);
            hireButtons.Add(temp);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(hireContentPanel);
    }
     */
    #endregion
}
