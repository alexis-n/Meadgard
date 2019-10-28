using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ThiefTooltip
{
    public GameObject[] hps, skill1, skill2, skill3;
}

[System.Serializable]
public class MissiveTooltip
{
    public TextMeshProUGUI title, content;
    public Image titleBG;
}

[System.Serializable]
public class AlertTooltip
{
    public TextMeshProUGUI content;
}

[System.Serializable]
public class EmployeeTooltip
{
    public GameObject[] levels;
    public Image expBar;
    public Image[] skill1, skill2, skill3;
    public Image[] skillIcons;
    public TextMeshProUGUI[] skillNames;
}

[System.Serializable]
public class ClientTooltip
{
    public TextMeshProUGUI satisfactionPercentage, orderName;
    public Image satisfactionBar, orderIcon;
}

public class Tooltip : MonoBehaviour
{ 
    public static Tooltip instance;
    [HideInInspector] public CanvasGroup group;
    private RectTransform rect;
    public TextMeshProUGUI nameTagText, leftClickText, holdClickText, rightClickText;
    public GameObject nameTagGO, leftClickGO, holdClickGO, rightClickGO, thiefGO, missiveGO, alertGO, employeeGO, clientGO;
    public Image rightClickBG;

    public ThiefTooltip thiefTooltip;
    public MissiveTooltip missiveTooltip;
    public AlertTooltip alertTooltip;
    public EmployeeTooltip employeeTooltip;
    public ClientTooltip clientTooltip;

    public bool overTooltipableObject;

    private void Awake()
    {
        instance = this;
        rect = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
    }

    private void Update()
    {
        if (!overTooltipableObject)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0 && results[0].gameObject.GetComponent<Tooltipable>() && !UIManager.instance.cursorsBank.draggingElement)
                results[0].gameObject.GetComponent<Tooltipable>().TooltipMe();
            else group.alpha = 0;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void UITooltip(Tooltipable target, string nameTag, string leftClick, string holdClick, string rightClick, Color rightClickColor)
    {
        if (Input.mousePosition.x < (Screen.width / 4)*3)
        {
            rect.pivot = new Vector2(0, rect.pivot.y);
            rect.anchoredPosition = new Vector2(25, rect.anchoredPosition.y);
            GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
        }
        else
        {
            rect.pivot = new Vector2(1, rect.pivot.y);
            rect.anchoredPosition = new Vector2(-25, rect.anchoredPosition.y);
            GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleRight;
        }

        if (target.type == Tooltipable.TooltipType.Thief) thiefGO.SetActive(true);
        else thiefGO.SetActive(false);
        if (target.type == Tooltipable.TooltipType.CurrentMissive ||
            target.type == Tooltipable.TooltipType.PredictedMissive) missiveGO.SetActive(true);
        else missiveGO.SetActive(false);
        if (target.type == Tooltipable.TooltipType.Alert) alertGO.SetActive(true);
        else alertGO.SetActive(false);
        if (target.type == Tooltipable.TooltipType.Employee) employeeGO.SetActive(true);
        else employeeGO.SetActive(false);
        if (target.type == Tooltipable.TooltipType.Client) clientGO.SetActive(true);
        else clientGO.SetActive(false);

        switch (target.type)
        {
            case Tooltipable.TooltipType.Nothing:
                if (nameTag != string.Empty)
                {
                    nameTagGO.SetActive(true);
                    nameTagText.text = nameTag;
                }
                else nameTagGO.SetActive(false);
                break;

            case Tooltipable.TooltipType.Client:
                Client client = target.GetComponent<Client>();
                nameTagGO.SetActive(true);
                nameTagText.text = client.name;
                break;

            case Tooltipable.TooltipType.Furniture:
                Furniture furniture = target.GetComponent<Furniture>();
                nameTagGO.SetActive(true);
                nameTagText.text = furniture.name;
                break;

            case Tooltipable.TooltipType.Employee:
                Employee employee = target.GetComponent<Employee>();
                nameTagGO.SetActive(true);
                nameTagText.text = employee.employeeValues.employeeName;
                EmployeeTooltip(employee);
                break;

            case Tooltipable.TooltipType.Thief:
                Thief thief = target.GetComponent<Thief>();
                nameTagGO.SetActive(true);
                nameTagText.text = thief.name;
                ThiefTooltip(thief);
                break;

            case Tooltipable.TooltipType.Alert:
                Alert alert = target.GetComponent<Alert>();
                nameTagGO.SetActive(true);
                AlertTooltip(alert);
                break;

            case Tooltipable.TooltipType.PredictedMissive:
                if (nameTag != string.Empty)
                {
                    nameTagGO.SetActive(true);
                    nameTagText.text = nameTag;
                }
                else nameTagGO.SetActive(false);
                PredictedMissiveTooltip();
                break;

            case Tooltipable.TooltipType.CurrentMissive:
                if (nameTag != string.Empty)
                {
                    nameTagGO.SetActive(true);
                    nameTagText.text = nameTag;
                }
                else nameTagGO.SetActive(false);
                CurrentMissiveTooltip();
                break;
        }

        if (leftClick != string.Empty)
        {
            leftClickGO.SetActive(true);
            leftClickText.text = leftClick;
        }
        else leftClickGO.SetActive(false);

        if (holdClick != string.Empty)
        {
            holdClickGO.SetActive(true);
            holdClickText.text = holdClick;
        }
        else holdClickGO.SetActive(false);

        if (rightClick != string.Empty)
        {
            rightClickGO.SetActive(true);
            rightClickText.text = rightClick;
            rightClickBG.color = rightClickColor;
        }
        else rightClickGO.SetActive(false);

        group.DOFade(1, 0.2f);
    }

    public void ThiefTooltip(Thief thief)
    {
        for (int i = 0; i < thiefTooltip.hps.Length; i++)
        {
            if (i <= thief.thiefValues.thiefHealth - 1) thiefTooltip.hps[i].SetActive(true);
            else thiefTooltip.hps[i].SetActive(false);
        }

        for (int i = 0; i < thiefTooltip.skill1.Length; i++)
        {
            if (i <= thief.thiefValues.thiefSkills[0] - 1) thiefTooltip.skill1[i].SetActive(true);
            else thiefTooltip.skill1[i].SetActive(false);
            if (i <= thief.thiefValues.thiefSkills[1] - 1) thiefTooltip.skill2[i].SetActive(true);
            else thiefTooltip.skill2[i].SetActive(false);
            if (i <= thief.thiefValues.thiefSkills[2] - 1) thiefTooltip.skill3[i].SetActive(true);
            else thiefTooltip.skill3[i].SetActive(false);
        }
    }

    public void PredictedMissiveTooltip()
    {
        missiveTooltip.title.text = Missive.predictedMissive.missiveName;
        missiveTooltip.content.text = Missive.predictedMissive.missiveDescription;
        missiveTooltip.titleBG.color = Missive.predictedMissive.missiveColor;
    }
    public void CurrentMissiveTooltip()
    {
        missiveTooltip.title.text = Missive.currentMissive.missiveName;
        missiveTooltip.content.text = Missive.currentMissive.missiveDescription;
        missiveTooltip.titleBG.color = Missive.currentMissive.missiveColor;
    }

    public void AlertTooltip(Alert alert)
    {
        nameTagText.text = alert.alertTimeList[alert.index];
        alertTooltip.content.text = alert.alertContentList[alert.index];
    }

    public void EmployeeTooltip(Employee employee)
    {
        for (int i = 0; i < employeeTooltip.levels.Length; i++)
        {
            if (i <= employee.employeeValues.employeeLevel - 1) employeeTooltip.levels[i].SetActive(true);
            else employeeTooltip.levels[i].SetActive(false);
        }

        employeeTooltip.expBar.fillAmount = employee.employeeValues.employeeExperience / employee.employeeData.levelThresholds[employee.employeeValues.employeeLevel - 1];
        if (employeeTooltip.expBar.fillAmount == 1) employeeTooltip.expBar.color = GameManager.instance.data.rarityColors[4];
        else employeeTooltip.expBar.color = GameManager.instance.data.rarityColors[1];

        for (int i = 0; i < employee.employeeData.skillNames.Length; i++)
        {
            employeeTooltip.skillNames[i].text = employee.employeeData.skillNames[i] + ":";
            employeeTooltip.skillIcons[i].sprite = employee.employeeData.tokens[i];
        }

        for (int i = 0; i < employeeTooltip.skill1.Length; i++)
        {
            if (i <= employee.employeeValues.employeeSkills[0] - 1) employeeTooltip.skill1[i].gameObject.SetActive(true);
            else employeeTooltip.skill1[i].gameObject.SetActive(false);
            employeeTooltip.skill1[i].color = employee.employeeData.skillsColor[0];

            if (i <= employee.employeeValues.employeeSkills[1] - 1) employeeTooltip.skill2[i].gameObject.SetActive(true);
            else employeeTooltip.skill2[i].gameObject.SetActive(false);
            employeeTooltip.skill2[i].color = employee.employeeData.skillsColor[1];

            if (i <= employee.employeeValues.employeeSkills[2] - 1) employeeTooltip.skill3[i].gameObject.SetActive(true);
            else employeeTooltip.skill3[i].gameObject.SetActive(false);
            employeeTooltip.skill3[i].color = employee.employeeData.skillsColor[2];
        }
    }
}
