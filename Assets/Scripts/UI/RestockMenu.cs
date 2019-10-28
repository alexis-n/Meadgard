using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RestockMenu : Menu, ITutorialable
{
    #region /// VARIABLES ///
    [SerializeField] private List<TutorialStep> steps = new List<TutorialStep>();

    [SerializeField] private TextMeshProUGUI foodPriceText, drinkPriceText, finalPriceText, finalTimeText;
    [SerializeField] private TMP_InputField foodAmountText, drinkAmountText;
    [SerializeField] private Slider foodSlider, drinkSlider;
    [SerializeField] private TMP_Dropdown deliveryDropdown;
    [SerializeField] private GameObject unavailable, confirmationButton;
    public Button menuButton;
    float timeBeforeDelivery;

    private int finalPrice, finalTime;
    [SerializeField] private RectTransform deliveryTimer;
    [HideInInspector] public bool toggleTimer = false;
    float deliveryTimeLeft = 0;
    [SerializeField] private Vector2 deliveryTimerMovementAmount;
    [SerializeField] private TextMeshProUGUI deliveryTimerText;
    #endregion

    private void Awake()
    {
        if (content.activeSelf) content.SetActive(false);
    }

    private new void Update()
    {
        base.Update();

        if (toggleTimer)
        {
            deliveryTimeLeft -= Time.deltaTime;

            string minutes = Mathf.Floor(deliveryTimeLeft / 60).ToString("00");
            string seconds = (deliveryTimeLeft % 60).ToString("00");
            deliveryTimerText.text = minutes + ":" + seconds;
        }
    }

    public void StartTutorial()
    {
        if (!foldToggle) FoldAndUnfold();
        Tutorial.instance.StartTutorial(transform, steps);
    }

    //Fonction qui ouvre ou ferme le menu de restockage
    public override void OnOpening()
    {
        base.OnOpening();

        foodSlider.value = 0;
        drinkSlider.value = 0;
        deliveryDropdown.value = 0;
    }

    //Fonction qui met à jour le menu de restockage
    public override void UpdateContent()
    {
        base.UpdateContent();

        unavailable.SetActive(toggleTimer);
        if (foodSlider.value != 0 || drinkSlider.value != 0) confirmationButton.SetActive(!toggleTimer);
        else confirmationButton.SetActive(false);

        foodPriceText.text = (PlayerManager.instance.foodPrice * Missive.currentMissive.foodRestockPrice).ToString();
        drinkPriceText.text = (PlayerManager.instance.drinkPrice * Missive.currentMissive.drinkRestockPrice).ToString();

        foodSlider.maxValue = (PlayerManager.instance.PlayerFoodCapacity - PlayerManager.instance.PlayerFood);
        drinkSlider.maxValue = (PlayerManager.instance.PlayerDrinksCapacity - PlayerManager.instance.PlayerDrinks);

        finalPrice = (int)((int)foodSlider.value * PlayerManager.instance.foodPrice * Missive.currentMissive.foodRestockPrice) + (int)((int)drinkSlider.value * PlayerManager.instance.drinkPrice * Missive.currentMissive.drinkRestockPrice);

        if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Service)
        {
            finalTime = deliveryDropdown.value;
            deliveryDropdown.interactable = true;

            switch (finalTime)
            {
                case 0:
                    finalTimeText.text = "1 min.";
                    timeBeforeDelivery = 60;
                    break;
                case 1:
                    finalPrice = (int)(finalPrice * (2 * Missive.currentMissive.deliveryPrice));
                    finalTimeText.text = "30 s.";
                    timeBeforeDelivery = 30;
                    break;
                case 2:
                    finalPrice = (int)(finalPrice * (3 * Missive.currentMissive.deliveryPrice));
                    finalTimeText.text = "0 s.";
                    timeBeforeDelivery = 0;
                    break;
            }
        }
        else
        {
            finalTime = 2;
            deliveryDropdown.interactable = false;
            finalTimeText.text = "0 s.";
        }

        finalPriceText.text = finalPrice.ToString();
    }
    //Fonction qui gère le restockage par les sliders
    public void UpdateContentWithSlider()
    {
        foodAmountText.text = foodSlider.value.ToString();
        drinkAmountText.text = drinkSlider.value.ToString();

        UpdateContent();
    }
    //Fonction qui gère le restockage par les inputfields
    public void UpdateContentWithInputField()
    {
        foodSlider.value = int.Parse(foodAmountText.text);
        drinkSlider.value = int.Parse(drinkAmountText.text);

        UpdateContent();
    }

    //fonction qui confirme la commande et vérifie si le joueur a assez d'argent pour l'effectuer
    public void RequestRestocking()
    {
        if (PlayerManager.instance.PlayerMoney >= finalPrice)
        {
            PlayerManager.instance.PlayerMoney -= finalPrice;
            PlayerManager.instance.restockMoney -= finalPrice;
            UIManager.instance.blackPanel.recapPanel.restockValue -= finalPrice;

            if ((int)foodSlider.value > 0) UIManager.instance.newsboardMenu.hasBoughtFood = true;
            if ((int)drinkSlider.value > 0) UIManager.instance.newsboardMenu.hasBoughtDrinks = true;

            if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Prep)
                PlayerManager.instance.Restock((int)foodSlider.value, (int)drinkSlider.value);

            else if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Service)
                LaunchDeliveryTimer((int)foodSlider.value, (int)drinkSlider.value, timeBeforeDelivery);

            if (content.activeSelf) content.SetActive(false);
            UpdateContent();
        }
        else UIManager.instance.playerMoney.StatFlickerRed();
    }

    public void LaunchDeliveryTimer(int foodAmount, int drinkAmount, float timeBeforeDelivery)
    {
        StartCoroutine(DeliveryTimer(foodAmount, drinkAmount, timeBeforeDelivery));
    }
    //fonction qui initialise le timer de livraison
    public IEnumerator DeliveryTimer(int foodAmount, int drinkAmount, float timeBeforeDelivery)
    {
        //on anime l'ouverture du timer
        deliveryTimer.DOAnchorPos(new Vector2(deliveryTimer.anchoredPosition.x + deliveryTimerMovementAmount.x, deliveryTimer.anchoredPosition.y + deliveryTimerMovementAmount.y), 0.2f);

        deliveryTimeLeft = timeBeforeDelivery;
        toggleTimer = true;
        yield return new WaitForSeconds(deliveryTimeLeft);

        toggleTimer = false;
        PlayerManager.instance.Restock(foodAmount, drinkAmount);
        deliveryTimer.DOAnchorPos(new Vector2(deliveryTimer.anchoredPosition.x - deliveryTimerMovementAmount.x, deliveryTimer.anchoredPosition.y - deliveryTimerMovementAmount.y), 0.2f);
    }

    #region /// VIEUX CODE ///
    /*
    public void ToggleRestockMenu()
    {
        if (restockMenuTrigger) CloseRestockMenu();
        else OpenRestockMenu();
    }
    public void OpenRestockMenu()
    {
        CloseEmployeeMenu();

        restockMenu.SetActive(true);
        restockMenuTrigger = true;

        foodPriceText.text = ServiceManager.instance.FoodPrice.ToString();
        drinkPriceText.text = ServiceManager.instance.DrinkPrice.ToString();
        foodSlider.value = 0;
        drinkSlider.value = 0;
        deliveryDropdown.value = 0;
    }
    public void CloseRestockMenu()
    {
        restockMenu.SetActive(false);
        restockMenuTrigger = false;
        Tooltip();
    }
    public void UpdateRestockMenu()
    {
        foodSlider.maxValue = (ServiceManager.instance.PlayerFoodCapacity - ServiceManager.instance.PlayerFood);
        drinkSlider.maxValue = (ServiceManager.instance.PlayerDrinksCapacity - ServiceManager.instance.PlayerDrinks);

        foodAmountText.text = foodSlider.value.ToString();
        drinkAmountText.text = drinkSlider.value.ToString();
        finalPrice = ((int)foodSlider.value * ServiceManager.instance.FoodPrice) + ((int)drinkSlider.value * ServiceManager.instance.DrinkPrice);

        if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Service)
        {
            finalTime = deliveryDropdown.value;
            deliveryDropdown.interactable = true;

            switch (finalTime)
            {
                case 0:
                    finalTimeText.text = "1:00";
                    break;
                case 1:
                    finalPrice *= 2;
                    finalTimeText.text = "0:30";
                    break;
                case 2:
                    finalPrice *= 3;
                    finalTimeText.text = "Immediat";
                    break;
            }
        }
        else
        {
            finalTime = 2;
            deliveryDropdown.interactable = false;
            finalTimeText.text = "Immediat";
        }

        finalPriceText.text = finalPrice.ToString();
    }
    public void RequestRestocking()
    {
        if (ServiceManager.instance.PlayerMoney >= finalPrice)
        {
            ServiceManager.instance.Restock((int)foodSlider.value, (int)drinkSlider.value, finalTime);
            CloseRestockMenu();
        }
        else playerMoney.StatFlickerRed();
    }
    public void DisplayDeliveryTimer(bool trigger, int timeToWait = 0)
    {
        if (trigger)
        {
            deliveryTimerPanel.DOAnchorPos3DX(deliveryTimerPanel.localPosition.x + deliveryTimerOffset, 0.2f);
            restockButton.interactable = false;
            deliveryTime = timeToWait;
            deliveryTimerTrigger = true;
        }
        else
        {
            deliveryTimerPanel.DOAnchorPos3DX(deliveryTimerPanel.localPosition.x - deliveryTimerOffset, 0.2f);
            restockButton.interactable = true;
            deliveryTimerTrigger = false;
        }
    }
    */
    #endregion
}
