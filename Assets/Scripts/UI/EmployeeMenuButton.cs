using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EmployeeMenuButton : MonoBehaviour {

    public Employee employee;
    public GameObject content, fireButton;
    public TextMeshProUGUI npcName, employeeWage;
   [SerializeField] private Image[] levelStars;
    public Image rarityStar, rarityPaint, experienceFiller;

    [SerializeField] private Tooltipable[] skillsTooltips;
    [SerializeField] private TextMeshProUGUI[] skillsTexts;
    [SerializeField] private Image[] skillsIcons;

    [SerializeField] private Tooltipable rarityTooltip;

    [SerializeField] private Image diamondColor;

    public void Initilialization(Employee givenEmployee)
    {
        employee = givenEmployee;
        npcName.text = givenEmployee.name;
        employeeWage.text = employee.employeeValues.employeeWage.ToString();

        Color temp = GameManager.instance.data.rarityColors[(int)employee.employeeValues.employeeRarity];
        rarityStar.color = temp;
        rarityPaint.color = new Color(temp.r, temp.g, temp.b, rarityPaint.color.a);
        rarityTooltip.nameTag = employee.employeeValues.employeeRarity.ToString();

        diamondColor.color = employee.diamondColor;

        UpdateContent();
    }

    public void UpdateContent()
    {
        if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Prep) fireButton.SetActive(true);
        else fireButton.SetActive(false);

        for (int i = 0; i < employee.employeeValues.employeeSkills.Length; i++)
        {
            skillsTexts[i].text = employee.employeeValues.employeeSkills[i].ToString();
            skillsIcons[i].sprite = employee.employeeData.tokens[i];
            skillsTooltips[i].nameTag = employee.employeeData.skillDesc[i];
        }

        for (int i = 0; i < levelStars.Length; i++)
        {
            if (i < employee.employeeValues.employeeLevel) levelStars[i].gameObject.SetActive(true);
            else levelStars[i].gameObject.SetActive(false);
        }
        experienceFiller.fillAmount = employee.employeeValues.employeeExperience / employee.employeeData.levelThresholds[employee.employeeValues.employeeLevel - 1];
        if (experienceFiller.fillAmount == 1) experienceFiller.color = GameManager.instance.data.rarityColors[4];
        else experienceFiller.color = GameManager.instance.data.rarityColors[1];
    }
    
    public void ShowNPC ()
    {
        CameraManager.instance.ViewTarget(employee.gameObject);

        if (UIManager.instance.currentMenu == UIManager.instance.employeesMenu)
            UIManager.instance.CloseCurrentMenu();

        SelectionManager.instance.SelectNewObject(employee);
    }

    public void FireNPC()
    {
        NPCManager.instance.FireEmployee(employee);
    }

    public void NextColor()
    {
        var temp = System.Array.IndexOf(employee.employeeData.diamondColors, diamondColor.color);
        if (temp == employee.employeeData.diamondColors.Length -1)
        {
            employee.diamondColor = employee.employeeData.diamondColors[0];
            employee.employeeValues.diamondColorIndex = 0;
            employee.npcBubble.bubble.diamondColor.color = employee.diamondColor;
        }
        else
        {
            employee.diamondColor = employee.employeeData.diamondColors[temp + 1];
            employee.employeeValues.diamondColorIndex = temp + 1;
            employee.npcBubble.bubble.diamondColor.color = employee.diamondColor;
        }

        diamondColor.color = employee.diamondColor;
    }
    public void PreviousColor()
    {
        var temp = System.Array.IndexOf(employee.employeeData.diamondColors, diamondColor.color);
        if (temp == 0)
        {
            employee.diamondColor = employee.employeeData.diamondColors[employee.employeeData.diamondColors.Length-1];
            employee.employeeValues.diamondColorIndex = employee.employeeData.diamondColors.Length - 1;
            employee.npcBubble.bubble.diamondColor.color = employee.diamondColor;
        }
        else
        {
            employee.diamondColor = employee.employeeData.diamondColors[temp - 1];
            employee.employeeValues.diamondColorIndex = temp - 1;
            employee.npcBubble.bubble.diamondColor.color = employee.diamondColor;
        }

        diamondColor.color = employee.diamondColor;
    }
}
