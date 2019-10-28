using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SelectionPanel : MonoBehaviour
{
    #region ///  VARIABLES  ///
    [Header("General")]
    private bool inAnimation = false;
    private InteractableObject selectedObject;
    public UnityEvent openingSelectionPanel = new UnityEvent();
    private UnityAction updatePanel;

    [SerializeField] private RectTransform panelRectTransform;
    private Vector3 panelBasePosition;
    public Vector3 panelMovementAmount;
    public float animationDuration;

    [SerializeField] private Image icon;
    [SerializeField] private new TextMeshProUGUI name;
    private List<GameObject> contents = new List<GameObject>();

    [SerializeField] private GameObject level;
    [SerializeField] private TextMeshProUGUI levelText;
    [Space(7)]

    [Header("Dish Content")]
    [SerializeField] private GameObject dishContent;
    [SerializeField] private Image[] dishGradients;
    [SerializeField] private TextMeshProUGUI qualityText, freshnessText;
    [SerializeField] private GameObject[] dishLevels;
    [SerializeField] private Image dishTemperature;
    [Space(7)]

    [Header("Client Content")]
    [SerializeField] private GameObject clientContent;
    [SerializeField] private TextMeshProUGUI clientSatisfactionText;
    [SerializeField] private RectTransform unhappyRect, middleRect, happyRect, cursorRect;
    [Space(7)]

    [Header("Employee Content")]
    [SerializeField] private Image expFiller;
    [SerializeField] private RectTransform statsRectTransform;
    private Vector3 statsBasePosition;
    public Vector3 statsMovementAmount;
    [SerializeField] private GameObject openStatsButton, closeStatsButton;
    [SerializeField] private GameObject diamondObj;
    [SerializeField] private Image diamondColor;

    [SerializeField] private Image rarityGradient;
    [SerializeField] private TextMeshProUGUI rarityText, remainingPoints, skill1Name, skill2Name, skill3Name;
    [SerializeField] private GameObject[] skill1Points, skill2Points, skill3Points, skillGradients,
        skill1Arrows, skill2Arrows, skill3Arrows;
    private List<GameObject> levelUpSkill1Points = new List<GameObject>(),
        levelUpSkill2Points = new List<GameObject>(),
        levelUpSkill3Points = new List<GameObject>();

    private int tempPoints, tempSkill1Points, tempSkill2Points, tempSkill3Points;
    [SerializeField] private GameObject confirmationButton;
    [Space(7)]

    [Header("Waitress Content")]
    [SerializeField] private GameObject waitressContent;
    [SerializeField] private DragPriority orderDrag, serveDrag, cleanDrag;
    [SerializeField] private PrioritySlot[] waitressManualSlots;
    [SerializeField] private PrioritySlot [] waitressAutoSlots;
    [Space(7)]

    [Header("Cook Content")]
    [SerializeField] private GameObject cookContent;
    [SerializeField] private GameObject cookOccupied;
    [SerializeField] private Image cookOccupiedIcon;
    [SerializeField] private DragPriority cookingDrag, mixingDrag;
    [SerializeField] private PrioritySlot[] cookManualSlots;
    [SerializeField] private PrioritySlot[] cookAutoSlots;
    [Space(7)]

    [Header("Bouncer Content")]
    [SerializeField] private GameObject bouncerContent;
    [SerializeField] private TMP_InputField bouncerInput;
    [Space(7)]

    [Header("Bard Content")]
    [SerializeField] private GameObject bardContent;
    [Space(7)]

    [Header("Furniture Content")]
    [SerializeField] private GameObject upgradeContent;
    [SerializeField] private Image upgradeButtonColor;
    [SerializeField] private TextMeshProUGUI upgradePrice, addedValue;
    #endregion

    private void Start()
    {
        panelRectTransform.GetComponent<RectTransform>();
        contents.Add(dishContent);
        contents.Add(waitressContent);
        contents.Add(cookContent);
        contents.Add(bouncerContent);
        contents.Add(bardContent);
        contents.Add(clientContent);
        contents.Add(upgradeContent);
        for (int i = 0; i < contents.Count; i++)
        {
            contents[i].SetActive(false);
        }

        panelBasePosition = panelRectTransform.anchoredPosition;
        statsBasePosition = statsRectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (updatePanel != null && !inAnimation) updatePanel();
    }

    //Fonction qui anime l'ouverture du panel
    private void OpenPanel(InteractableObject interactableObject)
    {
        selectedObject = interactableObject;
        icon.sprite = interactableObject.data.icon;

        if (panelRectTransform.position != panelBasePosition)
        {
            inAnimation = true;
            panelRectTransform.DOAnchorPos(panelBasePosition, animationDuration/2).SetEase(Ease.InQuad)
                .OnComplete(() => OpenAndClose());
            if (selectedObject.GetComponent<Employee>() && statsRectTransform.position != statsBasePosition) CloseStats(animationDuration/2);
        }
        else
        {
            UpdatePanel();
            panelRectTransform.DOAnchorPos(new Vector2((panelBasePosition.x + panelMovementAmount.x), (panelBasePosition.y + panelMovementAmount.y)), animationDuration);
            if (selectedObject.GetComponent<Employee>() && selectedObject.GetComponent<Employee>().employeeValues.employeeLevelup) OpenStats();
        }

        if (!selectedObject.GetComponent<Employee>())
        {
            expFiller.fillAmount = 0;
            openStatsButton.SetActive(false);
            closeStatsButton.SetActive(false);
            diamondObj.SetActive(false);
        }
        else
        {
            diamondObj.SetActive(true);
            openStatsButton.SetActive(true);
            diamondColor.color = selectedObject.GetComponent<Employee>().diamondColor;
        }
    }
    private void OpenAndClose()
    {
        UpdatePanel();
        openingSelectionPanel.Invoke();
        inAnimation = false;
        panelRectTransform.DOAnchorPos(new Vector2((panelBasePosition.x + panelMovementAmount.x), (panelBasePosition.y + panelMovementAmount.y)), animationDuration/2).SetEase(Ease.OutQuad);
        if (selectedObject.GetComponent<Employee>() && selectedObject.GetComponent<Employee>().employeeValues.employeeLevelup) OpenStats(animationDuration / 2);
    }
    //Fonction qui update le panel
    public void UpdatePanel()
    {
        if (selectedObject != null)
        {
            name.text = selectedObject.name;

            if (selectedObject.GetComponent<Employee>())
            {
                level.SetActive(true);
                levelText.text = selectedObject.GetComponent<Employee>().employeeValues.employeeLevel.ToString();
                expFiller.fillAmount = selectedObject.GetComponent<Employee>().employeeValues.employeeExperience / selectedObject.GetComponent<Employee>().employeeData.levelThresholds[selectedObject.GetComponent<Employee>().employeeValues.employeeLevel - 1];
            }
            else if (selectedObject.GetComponent<Furniture>())
            {
                level.SetActive(true);
                levelText.text = selectedObject.GetComponent<Furniture>().level.ToString();
                if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Prep &&
                selectedObject.GetComponent<Furniture>().level < selectedObject.GetComponent<Furniture>().maxLevel) upgradeContent.SetActive(true);
                else upgradeContent.SetActive(false);
            }
            else level.SetActive(false);
        }
    }
    //Fonction qui anime la fermeture du panel
    public void ClosePanel()
    {
        panelRectTransform.DOAnchorPos(panelBasePosition, animationDuration).SetEase(Ease.InQuad);
        if (selectedObject.GetComponent<Employee>() && statsRectTransform.position != statsBasePosition) CloseStats();
        selectedObject = null;
        updatePanel = null;

        for (int i = 0; i < contents.Count; i++)
        {
            contents[i].SetActive(false);
        }
    }

    //fonction qui ouvre les stats
    public void OpenStats(float duration = 0)
    {
        openStatsButton.SetActive(false);
        closeStatsButton.SetActive(true);
        if (duration == 0) duration = animationDuration;
        statsRectTransform.DOAnchorPos(new Vector2((statsBasePosition.x + statsMovementAmount.x), (statsBasePosition.y + statsMovementAmount.y)), duration).SetEase(Ease.OutQuad);

        var employee = selectedObject.GetComponent<Employee>();

        rarityGradient.color = GameManager.instance.data.rarityColors[(int)employee.employeeValues.employeeRarity];
        rarityText.text = employee.employeeValues.employeeRarity.ToString();

        if (employee.employeeValues.employeeLevelup) tempPoints = employee.employeeData.skillPointsPerLevel;

        levelUpSkill1Points.Clear();
        tempSkill1Points = 0;
        levelUpSkill2Points.Clear();
        tempSkill2Points = 0;
        levelUpSkill3Points.Clear();
        tempSkill3Points = 0;

        if (employee.employeeValues.employeeLevelup) remainingPoints.gameObject.SetActive(true);
        else remainingPoints.gameObject.SetActive(false);

        skill1Name.text = employee.employeeData.skillNames[0];
        skill2Name.text = employee.employeeData.skillNames[1];
        skill3Name.text = employee.employeeData.skillNames[2];

        skillGradients[0].GetComponent<Image>().color = employee.employeeData.skillsColor[0];
        skillGradients[1].GetComponent<Image>().color = employee.employeeData.skillsColor[1];
        skillGradients[2].GetComponent<Image>().color = employee.employeeData.skillsColor[2];

        for (int i = 0; i < 5; i++)
        {
            if (System.Array.IndexOf(skill1Points, skill1Points[i]) < employee.employeeValues.employeeSkills[0])
            {
                skill1Points[i].SetActive(true);
                skill1Points[i].GetComponent<Image>().color = Color.green;
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
                skill2Points[i].GetComponent<Image>().color = Color.green;
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
                skill3Points[i].GetComponent<Image>().color = Color.green;
            }
            else
            {
                skill3Points[i].SetActive(false);
                skill3Points[i].GetComponent<Image>().color = Color.white;
                levelUpSkill3Points.Add(skill3Points[i]);
            }
        }

        UpdateStats();
    }
    //fonction qui ouvre les stats
    public void CloseStats(float duration = 0)
    {
        openStatsButton.SetActive(true);
        closeStatsButton.SetActive(false);
        if (duration == 0) duration = animationDuration;
        statsRectTransform.DOAnchorPos(statsBasePosition, duration).SetEase(Ease.InQuad);
    }
    //Fonction qui met à jour le menu de restockage
    public void UpdateStats()
    {
        var employee = selectedObject.GetComponent<Employee>();

        //Affichage des flèches qui retirent des points
        if (tempSkill1Points == 0 || !employee.employeeValues.employeeLevelup) skill1Arrows[0].SetActive(false);
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
            if (System.Array.IndexOf(skill1Points, skill1Points[i]) < employee.employeeValues.employeeSkills[0] + tempSkill1Points) skill1Points[i].SetActive(true);
            else skill1Points[i].SetActive(false);
            if (System.Array.IndexOf(skill2Points, skill2Points[i]) < employee.employeeValues.employeeSkills[1] + tempSkill2Points) skill2Points[i].SetActive(true);
            else skill2Points[i].SetActive(false);
            if (System.Array.IndexOf(skill3Points, skill3Points[i]) < employee.employeeValues.employeeSkills[2] + tempSkill3Points) skill3Points[i].SetActive(true);
            else skill3Points[i].SetActive(false);
        }

        remainingPoints.text = tempPoints.ToString();
        if (tempPoints == 0 && employee.employeeValues.employeeLevelup)
        {
            confirmationButton.SetActive(true);
            remainingPoints.gameObject.SetActive(false);
        }
        else
        {
            confirmationButton.SetActive(false);
            remainingPoints.gameObject.SetActive(true);
        }
    }
    //Fonction qui permet d'alouer un point de compétence
    public void AddTemporaryPoint(int index)
    {
        switch (index)
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

        UpdateStats();
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

        UpdateStats();
    }
    //fonction qui distribue les points
    public void ConfirmDistribution()
    {
        int[] temp = new int[3];
        temp[0] += tempSkill1Points;
        temp[1] += tempSkill2Points;
        temp[2] += tempSkill3Points;
        selectedObject.GetComponent<Employee>().LevelUp(temp);

        OpenStats();
    }

    //Fonction qui affiche les infos d'un Dish
    public void DishContent(Dish dish)
    {
        dishContent.SetActive(true);
        OpenPanel(dish);

        for (int i = 0; i < dishGradients.Length; i++)
        {
            dishGradients[i].color = dish.dishData.order.ressourceColor;
        }

        var temp = 0;
        switch(dish.quality)
        {
            case Dish.Quality.Average:
                qualityText.text = "Moyenne";
                break;
            case Dish.Quality.Bland:
                qualityText.text = "Fade";
                temp = 1;
                break;
            case Dish.Quality.Tasty:
                qualityText.text = "Savoureuse";
                temp = 2;
                break;
            case Dish.Quality.Delicious:
                qualityText.text = "Délicieuse";
                temp = 3;
                break;
            case Dish.Quality.Masterpiece:
                qualityText.text = "Chef-d'oeuvre";
                temp = 4;
                break;
        }
        for (int i = 0; i < dishLevels.Length; i++)
        {
            if (i <= temp) dishLevels[i].SetActive(true);
            else dishLevels[i].SetActive(false);
        }

        updatePanel = DishUpdate;
    }
    public void DishUpdate()
    {
        UpdatePanel();
        var dish = selectedObject.GetComponent<Dish>();
        switch (dish.freshness)
        {
            case Dish.Freshness.Fresh:
                freshnessText.text = "Fraîche";
                if (dish.dishData.order.ressourceType == Data.RessourceType.Drink)
                    dishTemperature.color = Color.blue;
                else dishTemperature.color = Color.red;
                break;
            case Dish.Freshness.Normal:
                freshnessText.text = "Normale";
                if (dish.dishData.order.ressourceType == Data.RessourceType.Drink)
                    dishTemperature.color = Color.Lerp(Color.blue, Color.grey, 0.5f);
                else dishTemperature.color = Color.Lerp(Color.red, Color.grey, 0.5f);
                break;
            case Dish.Freshness.Stale:
                freshnessText.text = "Périmée";
                dishTemperature.color = Color.grey;
                break;
        }
    }

    //Fonction qui affiche les infos d'un Client
    public void ClientContent(Client client)
    {
        clientContent.SetActive(true);
        OpenPanel(client);
        updatePanel = ClientUpdate;
    }
    public void ClientUpdate()
    {
        UpdatePanel();
        var client = selectedObject.GetComponent<Client>();

        unhappyRect.sizeDelta = new Vector2(GameManager.instance.data.unhappyThreshold*2, unhappyRect.sizeDelta.y);
        happyRect.sizeDelta = new Vector2(GameManager.instance.data.happyThreshold * 2, unhappyRect.sizeDelta.y);
        middleRect.sizeDelta = new Vector2((200f - unhappyRect.sizeDelta.x - happyRect.sizeDelta.x), unhappyRect.sizeDelta.y);
        cursorRect.anchoredPosition = new Vector2(client.Satisfaction * 2, cursorRect.anchoredPosition.y);

        clientSatisfactionText.text = (int)client.Satisfaction + "%";
    }

    //Fonction qui enregistre le changement de priorité d'un employé
    public void EmployeePriority(string priority, int value)
    {
        switch (priority)
        {
            default:
                break;

            case "orderPickup":
                selectedObject.GetComponent<Waitress>().orderPickupPriority = value;
                break;

            case "dishServing":
                selectedObject.GetComponent<Waitress>().dishServingPriority = value;
                break;

            case "trashCleaning":
                selectedObject.GetComponent<Waitress>().trashCleaningPriority = value;
                break;

            case "cooking":
                selectedObject.GetComponent<Cook>().cookingPriority = value;
                break;

            case "mixing":
                selectedObject.GetComponent<Cook>().mixingPriority = value;
                break;
        }
    }
    //Fonction qui permet de donner une instruction à un employé
    public void EmployeeInstruction(InteractableObject interactableObject)
    {
        selectedObject.GetComponent<Employee>().InteractWith(interactableObject);
    }
    //Fonction qui affiche les infos d'une Waitress
    public void WaitressContent(Waitress waitress)
    {
        waitressContent.SetActive(true);
        OpenPanel(waitress);

        updatePanel = WaitressUpdate;

        //on redistribue les éléments de priorité en fonction de leurs valeurs
        orderDrag.currentSlot.slottedPriority = null;
        serveDrag.currentSlot.slottedPriority = null;
        cleanDrag.currentSlot.slottedPriority = null;
        WaitressPriority(waitress.orderPickupPriority, orderDrag);
        WaitressPriority(waitress.dishServingPriority, serveDrag);
        WaitressPriority(waitress.trashCleaningPriority, cleanDrag);
    }
    public void WaitressPriority(int priority, DragPriority elementToMove)
    {
        switch (priority)
        {
            case 0:
                for (int i = 0; i < waitressManualSlots.Length; i++)
                {
                    if (waitressManualSlots[i].slottedPriority == null)
                    {
                        elementToMove.currentSlot = waitressManualSlots[i];
                        elementToMove.transform.SetParent(waitressManualSlots[i].transform);
                        elementToMove.transform.SetAsFirstSibling();
                        elementToMove.transform.localPosition = Vector3.zero;

                        waitressManualSlots[i].slottedPriority = elementToMove;
                        break;
                    }
                }
                break;
            case 3:
                elementToMove.currentSlot = waitressAutoSlots[0];
                elementToMove.transform.SetParent(waitressAutoSlots[0].transform);
                elementToMove.transform.SetAsFirstSibling();
                elementToMove.transform.localPosition = Vector3.zero;

                waitressAutoSlots[0].slottedPriority = elementToMove;
                break;
            case 2:
                elementToMove.currentSlot = waitressAutoSlots[1];
                elementToMove.transform.SetParent(waitressAutoSlots[1].transform);
                elementToMove.transform.SetAsFirstSibling();
                elementToMove.transform.localPosition = Vector3.zero;

                waitressAutoSlots[1].slottedPriority = elementToMove;
                break;
            case 1:
                elementToMove.currentSlot = waitressAutoSlots[2];
                elementToMove.transform.SetParent(waitressAutoSlots[2].transform);
                elementToMove.transform.SetAsFirstSibling();
                elementToMove.transform.localPosition = Vector3.zero;

                waitressAutoSlots[2].slottedPriority = elementToMove;
                break;
        }
    }
    public void WaitressUpdate()
    {
        UpdatePanel();

        var waitress = selectedObject.GetComponent<Waitress>();
    }
    //Fonction qui affiche les infos d'un Cook
    public void CookContent(Cook cook)
    {
        cookContent.SetActive(true);
        OpenPanel(cook);

        updatePanel = CookUpdate;

        //on redistribue les éléments de priorité en fonction de leurs valeurs
        cookingDrag.currentSlot.slottedPriority = null;
        mixingDrag.currentSlot.slottedPriority = null;
        CookPriority(cook.cookingPriority, cookingDrag);
        CookPriority(cook.mixingPriority, mixingDrag);
    }
    public void CookPriority(int priority, DragPriority elementToMove)
    {
        switch (priority)
        {
            case 0:
                for (int i = 0; i < cookManualSlots.Length; i++)
                {
                    if (cookManualSlots[i].slottedPriority == null)
                    {
                        elementToMove.currentSlot = cookManualSlots[i];
                        elementToMove.transform.SetParent(cookManualSlots[i].transform);
                        elementToMove.transform.SetAsFirstSibling();
                        elementToMove.transform.localPosition = Vector3.zero;

                        cookManualSlots[i].slottedPriority = elementToMove;
                        break;
                    }
                }
                break;
            case 2:
                elementToMove.currentSlot = cookAutoSlots[0];
                elementToMove.transform.SetParent(cookAutoSlots[0].transform);
                elementToMove.transform.SetAsFirstSibling();
                elementToMove.transform.localPosition = Vector3.zero;

                cookAutoSlots[0].slottedPriority = elementToMove;
                break;
            case 1:
                elementToMove.currentSlot = cookAutoSlots[1];
                elementToMove.transform.SetParent(cookAutoSlots[1].transform);
                elementToMove.transform.SetAsFirstSibling();
                elementToMove.transform.localPosition = Vector3.zero;

                cookAutoSlots[1].slottedPriority = elementToMove;
                break;
        }
    }
    public void CookUpdate()
    {
        UpdatePanel();

        var cook = selectedObject.GetComponent<Cook>();
        if (cook.state != Cook.State.Free && cook.state != Cook.State.Standby || PhaseManager.instance.currentPhase == Data.CurrentPhase.Prep)
        {
            if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Prep)
                cookOccupiedIcon.gameObject.SetActive(false);
            else
            {
                cookOccupiedIcon.gameObject.SetActive(true);
                cookOccupiedIcon.sprite = cook.order.ressourceSprite;
            }

            cookOccupied.SetActive(true);
        }
        else cookOccupied.SetActive(false);
    }
    //Fonction qui affiche les infos d'un Bouncer
    public void BouncerContent(Bouncer bouncer)
    {
        updatePanel = BouncerUpdate;
        bouncerContent.SetActive(true);
        OpenPanel(bouncer);
    }
    public void BouncerUpdate()
    {
        UpdatePanel();
    }
    //Fonction qui affiche les infos d'un Bard
    public void BardContent(Bard bard)
    {
        updatePanel = BardUpdate;
        bardContent.SetActive(true);
        OpenPanel(bard);
    }
    public void BardUpdate()
    {
        UpdatePanel();
    }
    public void BardInstruction(bool yesno)
    {
        Bard.generatePopularity = yesno;
    }

    //Fonction qui affiche les infos d'une Table
    public void TableContent(Table table)
    {
        OpenPanel(table);

        upgradePrice.text = (int)(selectedObject.GetComponent<Furniture>().furnitureData.upgradePrice * Missive.currentMissive.furnitureUpgradingPrice) + "g";
        addedValue.text = "+2 chaises";

        updatePanel = TableUpdate;
    }
    public void TableUpdate()
    {
        UpdatePanel();
    }
    //Fonction qui affiche les infos d'un Counter
    public void CounterContent(Counter counter)
    {
        OpenPanel(counter);

        upgradePrice.text = (int)(selectedObject.GetComponent<Furniture>().furnitureData.upgradePrice * Missive.currentMissive.furnitureUpgradingPrice) + "g";
        addedValue.text = "+3 places";

        updatePanel = CounterUpdate;
    }
    public void CounterUpdate()
    {
        UpdatePanel();
    }
    //Fonction qui affiche les infos d'un Shelf
    public void ShelfContent(Shelf shelf)
    {
        OpenPanel(shelf);

        upgradePrice.text = (int)(selectedObject.GetComponent<Furniture>().furnitureData.upgradePrice * Missive.currentMissive.furnitureUpgradingPrice) + "g";
        addedValue.text = "+10 en capacité";

        updatePanel = UpdateShelf;
    }
    public void UpdateShelf()
    {
        UpdatePanel();
    }
    //Fonction qui affiche les infos d'un Furnace
    public void FurnaceContent(Furnace furnace)
    {
        OpenPanel(furnace);

        upgradePrice.text = (int)(selectedObject.GetComponent<Furniture>().furnitureData.upgradePrice * Missive.currentMissive.furnitureUpgradingPrice) + "g";
        addedValue.text = "- perte de fraîcheur";

        updatePanel = UpdateFurnace;
    }
    public void UpdateFurnace()
    {
        UpdatePanel();
    }

    //Fonction qui affiche les infos des déchêts
    public void TrashContent(Trash trash)
    {
        OpenPanel(trash);
    }

    //Fonction qui permet de focus la caméra sur l'objet sélectionné
    public void FocusSelected()
    {
        if (selectedObject != null) CameraManager.instance.ViewTarget(selectedObject.gameObject);
    }

    //fonction qui permet d'upgrade un meuble
    public void FurnitureUpgrade()
    {
        if (PlayerManager.instance.PlayerMoney >= selectedObject.GetComponent<Furniture>().furnitureData.upgradePrice * Missive.currentMissive.furnitureUpgradingPrice)
        {
            PlayerManager.instance.PlayerMoney -= (int)(selectedObject.GetComponent<Furniture>().furnitureData.upgradePrice * Missive.currentMissive.furnitureUpgradingPrice);
            PlayerManager.instance.furnitureMoney -= (int)(selectedObject.GetComponent<Furniture>().furnitureData.upgradePrice * Missive.currentMissive.furnitureUpgradingPrice);
            UIManager.instance.blackPanel.recapPanel.furnitureValue -= (int)(selectedObject.GetComponent<Furniture>().furnitureData.upgradePrice * Missive.currentMissive.furnitureUpgradingPrice);
            selectedObject.GetComponent<Furniture>().Upgrade();
        }
        else UIManager.instance.playerMoney.StatFlickerRed();
    }
    //fonction qui supprimer un plat
    public void DeleteDish()
    {
        UIManager.instance.clientInfos.UpdateDishInfos(selectedObject.GetComponent<Dish>().dishData.order, true);
        selectedObject.GetComponent<Dish>().Autodestruct();
        SelectionManager.instance.UnselectObject();
    }
}
