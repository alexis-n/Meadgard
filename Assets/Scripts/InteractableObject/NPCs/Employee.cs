using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class EmployeeValues
{
    public string employeeName;
    public bool employeeLevelup;
    public float employeeExperience;
    public Employee.Rarity employeeRarity;
    public Employee.EmployeeType employeeType;
    public int employeeLevel = 1, employeeSkillPoints, employeeWage = 5, employeeRecruitmentFee = 20;
    public int[] employeeSkills = new int[3] { 1, 1, 1 };
    public int diamondColorIndex = -1;
}

[RequireComponent(typeof(AudioSource))]
public class Employee : NPC
{
    #region ///  VARIABLES  ///
    public EmployeeData employeeData;
    public EmployeeValues employeeValues;
    protected AudioSource audioSource;

    public enum Rarity { Amateur, Apprenti, Professionnel, Expert, Célèbre };
    public enum EmployeeType { Waitress, Cook, Bouncer, Bard };

    [HideInInspector] public Transform idlePosition;
    public Color diamondColor;

    [SerializeField] private GameObject levelupParticles;
    #endregion

    public new void Awake()
    {
        base.data = employeeData;
        base.Awake();

        audioSource = GetComponent<AudioSource>();
        if (employeeValues.employeeLevelup) levelupParticles.SetActive(true);
    }

    //On override la fonction CheckIfInteractable afin de préciser des conditions propres à Employee
    public override bool CheckIfSelectable()
    {
        //Les employés peuvent toujours être séléctionnés
        selectable = true;

        return base.CheckIfSelectable();
    }

    //Fonction qui gère le gain d'expérience
    public void GainExperience(float amount)
    {
        if (employeeValues.employeeLevelup || employeeValues.employeeLevel == employeeData.maxLevel) return;
        else
        {
            employeeValues.employeeExperience += amount;
            if (employeeValues.employeeExperience >= employeeData.levelThresholds[employeeValues.employeeLevel - 1])
            {
                employeeValues.employeeLevelup = true;
                employeeValues.employeeExperience = employeeData.levelThresholds[employeeValues.employeeLevel - 1];
                employeeValues.employeeSkillPoints += employeeData.skillPointsPerLevel;
                AlertPanel.instance.GenerateAlert(Alert.AlertType.LevelUp, name, (employeeValues.employeeLevel + 1).ToString());
                levelupParticles.SetActive(true);
            }
        }
    }

    public override void Select()
    {
        base.Select();
        audioSource.clip = employeeData.selectionSounds[Random.Range(0, employeeData.selectionSounds.Length)];
        audioSource.Play();
    }

    //Fonction qui gère l'allocation des points de talents
    public void LevelUp(int[] addedSkillPoints)
    {
        employeeValues.employeeSkills[0] += addedSkillPoints[0];
        employeeValues.employeeSkills[1] += addedSkillPoints[1];
        employeeValues.employeeSkills[2] += addedSkillPoints[2];

        employeeValues.employeeLevel++;
        employeeValues.employeeExperience = 0;
        employeeValues.employeeLevelup = false;

        UIManager.instance.newsboardMenu.hasLeveledUpStaff = true;

        levelupParticles.SetActive(false);
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //si on a pas déjà selectionné l'objet et qu'on peut le selectionner
            if (SelectionManager.instance.selectedObject != this &&
                CheckIfSelectable())
            {
                Tooltip.instance.overTooltipableObject = true;
                Tooltip.instance.UITooltip(tooltipable, employeeValues.employeeName, tooltipable.leftClick, tooltipable.holdClick, tooltipable.rightClick, tooltipable.rightClickColor);
            }
        }
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        Tooltip.instance.overTooltipableObject = false;
    }
}
