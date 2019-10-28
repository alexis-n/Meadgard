using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HireMenuButton : MonoBehaviour
{
    private EmployeeValues employeeValues;
    [SerializeField] private EmployeeData data;
    public GameObject content;
    [SerializeField] private TextMeshProUGUI employeeName, employeeWage, employeeFee;

    [SerializeField] private TextMeshProUGUI[] skillsTexts;
    [SerializeField] private Image[] skillsIcons;
    [SerializeField] private Tooltipable[] skillsTooltips;

    [SerializeField] private Image rarityStar, rarityPaint;
    [SerializeField] private Tooltipable rarityTooltip;

    [SerializeField] private TMP_Dropdown diamondDropdown;

    public void Initilialization(EmployeeValues givenValues)
    {
        employeeValues = givenValues;

        employeeName.text = givenValues.employeeName.Replace("\n", " "); 
        employeeWage.text = givenValues.employeeWage.ToString();
        employeeFee.text = givenValues.employeeRecruitmentFee.ToString();

        Color temp = GameManager.instance.data.rarityColors[(int)givenValues.employeeRarity];
        rarityStar.color = temp;
        rarityPaint.color = new Color(temp.r, temp.g, temp.b, rarityPaint.color.a);
        rarityTooltip.nameTag = employeeValues.employeeRarity.ToString();

        for (int i = 0; i < employeeValues.employeeSkills.Length; i++)
        {
            skillsTexts[i].text = employeeValues.employeeSkills[i].ToString();
            skillsIcons[i].sprite = data.tokens[i];
            skillsTooltips[i].nameTag = data.skillDesc[i];
        }
    }

    public void HireEmployee()
    {
        if (PlayerManager.instance.CanSpendMoney(employeeValues.employeeRecruitmentFee))
        {
            content.SetActive(false);
            UIManager.instance.newsboardMenu.hasHiredNewStaff = true;
            NPCManager.instance.HireEmployee(employeeValues);
        }
        else UIManager.instance.playerMoney.StatFlickerRed();
    }
}
