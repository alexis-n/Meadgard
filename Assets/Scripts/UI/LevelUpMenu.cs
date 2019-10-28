using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelUpMenu : Menu
{
    [HideInInspector] public Employee employee;

    [SerializeField] private TextMeshProUGUI skill1Name, skill2Name, skill3Name, remainingPointsText;
    [SerializeField] private Image employeeIcon, skill1Icon, skill2Icon, skill3Icon;
    [SerializeField] private GameObject[] skill1Points, skill2Points, skill3Points,
        skill1Arrows, skill2Arrows, skill3Arrows;
    private List<GameObject> levelUpSkill1Points = new List<GameObject>(),
        levelUpSkill2Points = new List<GameObject>(),
        levelUpSkill3Points = new List<GameObject>();

    private int tempPoints, tempSkill1Points, tempSkill2Points, tempSkill3Points;
    [SerializeField] private GameObject remainingPointsPanel, confirmationButton;
    [SerializeField] private TMP_InputField nameInput;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    private void OnEnable()
    {
        OnOpening();
    }

    //Fonction qui ouvre ou ferme le menu de restockage
    public  void OnOpening()
    {
        GetComponent<RectTransform>().anchoredPosition = defaultPosition;

        if (employee.employeeValues.employeeLevelup) tempPoints = employee.employeeData.skillPointsPerLevel;

        levelUpSkill1Points.Clear();
        tempSkill1Points = 0;
        levelUpSkill2Points.Clear();
        tempSkill2Points = 0;
        levelUpSkill3Points.Clear();
        tempSkill3Points = 0;

        if (!foldToggle) FoldAndUnfold();
        
        nameInput.text = employee.name.Replace("\n", " ");

        if (employee.employeeValues.employeeLevelup) remainingPointsPanel.SetActive(true);
        else remainingPointsPanel.SetActive(false);

        employeeIcon.sprite = employee.employeeData.icon;

        skill1Icon.sprite = employee.employeeData.tokens[0];
        skill1Name.text = employee.employeeData.skillNames[0];

        skill2Icon.sprite = employee.employeeData.tokens[1];
        skill2Name.text = employee.employeeData.skillNames[1];

        skill3Icon.sprite = employee.employeeData.tokens[2];
        skill3Name.text = employee.employeeData.skillNames[2];

        for (int i = 0; i < 5; i++)
        {
            if (System.Array.IndexOf(skill1Points, skill1Points[i]) < employee.employeeValues.employeeSkills[0])
            {
                skill1Points[i].SetActive(true);
                skill1Points[i].GetComponent<Image>().color = employee.employeeData.skillsColor[0];
            }
            else
            {
                skill1Points[i].SetActive(false);
                skill1Points[i].GetComponent<Image>().color = Color.white;
                levelUpSkill1Points.Add(skill1Points[i]);
            }
            if (System.Array.IndexOf(skill2Points, skill2Points[i]) < employee.employeeValues.employeeSkills[1])
            {
                skill2Points[i].SetActive(true);
                skill2Points[i].GetComponent<Image>().color = employee.employeeData.skillsColor[1];
            }
            else
            {
                skill2Points[i].SetActive(false);
                skill2Points[i].GetComponent<Image>().color = Color.white;
                levelUpSkill2Points.Add(skill2Points[i]);
            }
            if (System.Array.IndexOf(skill3Points, skill3Points[i]) < employee.employeeValues.employeeSkills[2])
            {
                skill3Points[i].SetActive(true);
                skill3Points[i].GetComponent<Image>().color = employee.employeeData.skillsColor[2];
            }
            else
            {
                skill3Points[i].SetActive(false);
                skill3Points[i].GetComponent<Image>().color = Color.white;
                levelUpSkill3Points.Add(skill3Points[i]);
            }
        }

        UpdateContent();
    }

    //Fonction qui met à jour le menu de restockage
    public override void UpdateContent()
    {
        //Affichage des flèches qui retirent des points
        if(tempSkill1Points == 0 || !employee.employeeValues.employeeLevelup) skill1Arrows[0].SetActive(false);
        else skill1Arrows[0].SetActive(true);
        if (tempSkill2Points == 0 || !employee.employeeValues.employeeLevelup) skill2Arrows[0].SetActive(false);
        else skill2Arrows[0].SetActive(true);
        if (tempSkill3Points == 0 || !employee.employeeValues.employeeLevelup) skill3Arrows[0].SetActive(false);
        else skill3Arrows[0].SetActive(true);

        //Affichage des flèches qui ajoutent des points
        if ((tempSkill1Points + employee.employeeValues.employeeSkills[0]) == 5 || tempPoints == 0 || !employee.employeeValues.employeeLevelup) skill1Arrows[1].SetActive(false);
        else skill1Arrows[1].SetActive(true);
        if ((tempSkill2Points + employee.employeeValues.employeeSkills[1]) == 5 || tempPoints == 0 || !employee.employeeValues.employeeLevelup) skill2Arrows[1].SetActive(false);
        else skill2Arrows[1].SetActive(true);
        if ((tempSkill3Points + employee.employeeValues.employeeSkills[2]) == 5 || tempPoints == 0 || !employee.employeeValues.employeeLevelup) skill3Arrows[1].SetActive(false);
        else skill3Arrows[1].SetActive(true);


        //Affichage des points temporaires
        for (int i = 0; i < skill1Points.Length; i++)
        {
            if (System.Array.IndexOf(skill1Points, skill1Points[i]) < employee.employeeValues.employeeSkills[0] + tempSkill1Points)
            {
                skill1Points[i].SetActive(true);
            }
            else
            {
                skill1Points[i].SetActive(false);
            }
            if (System.Array.IndexOf(skill2Points, skill2Points[i]) < employee.employeeValues.employeeSkills[1] + tempSkill2Points)
            {
                skill2Points[i].SetActive(true);
            }
            else
            {
                skill2Points[i].SetActive(false);
            }
            if (System.Array.IndexOf(skill3Points, skill3Points[i]) < employee.employeeValues.employeeSkills[2] + tempSkill3Points)
            {
                skill3Points[i].SetActive(true);
            }
            else
            {
                skill3Points[i].SetActive(false);
            }
        }

        remainingPointsText.text = tempPoints.ToString();
        if (tempPoints == 0 && employee.employeeValues.employeeLevelup) confirmationButton.SetActive(true);
        else confirmationButton.SetActive(false);
    }

    //Fonction qui permet d'alouer un point de compétence
    public void AddTemporaryPoint(int index)
    {
        switch(index)
        {
            case 0:
                tempSkill1Points++;
                tempPoints--;
                break;
            case 1:
                tempSkill2Points++;
                tempPoints--;
                break;
            case 2:
                tempSkill3Points++;
                tempPoints--;
                break;
        }

        UpdateContent();
    }
    //Fonction qui permet d'enlever un point de compétence temporaire
    public void RemoveTemporaryPoint(int index)
    {
        switch (index)
        {
            case 0:
                tempSkill1Points--;
                tempPoints++;
                break;
            case 1:
                tempSkill2Points--;
                tempPoints++;
                break;
            case 2:
                tempSkill3Points--;
                tempPoints++;
                break;
        }

        UpdateContent();
    }

    //fonction qui distribue les points
    public void ConfirmDistribution()
    {
        int[] temp= new int[3];
        temp[0] += tempSkill1Points;
        temp[1] += tempSkill2Points;
        temp[2] += tempSkill3Points;
        employee.LevelUp(temp);

        OnOpening();
    }

    // fonction qui confirme le nouveau nom
    public void ConfirmNewName()
    {
        employee.name = nameInput.text;
        UIManager.instance.selectionPanel.UpdatePanel();
    }

    // fonction qui refuse le nouveau nom
    public void DenyNewName()
    {
        nameInput.text = employee.name;
    }


    #region /// VIEUX CODE ///
    /*public void ToggleCharacterMenu()
    {
        if (characterMenuTrigger) CloseCharacterMenu();
        else OpenCharacterMenu();
    }
    public void OpenCharacterMenu()
    {
        characterMenu.SetActive(true);
        waitressCharacterContent.SetActive(false);
        cookCharacterContent.SetActive(false);
        characterMenuName.text = currentNPC.firstName + " " + currentNPC.lastName;

        switch (currentNPC.npcType)
        {
            case Data.NPCType.Waitress:
                waitressCharacterContent.SetActive(true);
                waitressCharacterContent.GetComponent<EmployeeStats>().Initialization(currentNPC.GetComponent<Employee>());
                characterMenuIcon.sprite = ServiceManager.instance.npcBank.waitressSprite;
                break;

            case Data.NPCType.Cook:
                cookCharacterContent.SetActive(true);
                cookCharacterContent.GetComponent<EmployeeStats>().Initialization(currentNPC.GetComponent<Employee>());
                characterMenuIcon.sprite = ServiceManager.instance.npcBank.cookSprite;
                break;

            default:
                break;
        }
    }
    public void ConfirmPointsDistribution()
    {
        switch (currentNPC.npcType)
        {
            case Data.NPCType.Waitress:
                waitressCharacterContent.GetComponent<EmployeeStats>().ConfirmPointsDistribution();
                break;

            case Data.NPCType.Cook:
                cookCharacterContent.GetComponent<EmployeeStats>().ConfirmPointsDistribution();
                break;

            default:
                break;
        }
    }
    public void CloseCharacterMenu()
    {
        characterMenu.SetActive(false);
        Tooltip();
    }*/
    #endregion
}
