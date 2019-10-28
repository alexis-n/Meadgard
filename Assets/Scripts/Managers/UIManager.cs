using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    /*=========================================================================================================================*/
    /*=========================================================================================================================*/
    /*=========================================================================================================================*/

    #region Variables
    public static UIManager instance;
    public Canvas canvas;

    public Sprite foodSprite, drinkSprite;
    public Color foodColor, drinkColor, silverColor, popularityColor;

    public GameObject debugInfo;

    public PlayerStatsUI playerMoney, playerFood, playerDrinks, playerCleanliness, playerPopularity;
    public SelectionPanel selectionPanel;
    public ClientInfos clientInfos;
    public AlertPanel alertPanel;
    public ReadyButton readyButton;

    public RectTransform buttonRack;
    public Vector2 buttonRackMovementAmount;
    public float animationDuration = 1f;

    public Menu currentMenu;
    public RestockMenu restockMenu;
    public HireMenu hireMenu;
    public EmployeesMenu employeesMenu;
    public ThiefMenu thiefMenu;
    public NewsboardMenu newsboardMenu;
    public Clock clock;

    public CursorsBank cursorsBank;

    public BlackPanel blackPanel;

    public RectTransform mouseFollower;

    public RectTransform godIcon;

    [SerializeField] private TextMeshProUGUI clientsMoney, furnitureMoney, employeeMoney, restockMoney, thievesMoney;

    [SerializeField] private GameObject ressourcePopupPrefab;
    #endregion

    /*=========================================================================================================================*/
    /*=========================================================================================================================*/
    /*=========================================================================================================================*/


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        restockMenu.defaultPosition = restockMenu.GetComponent<RectTransform>().anchoredPosition;
        hireMenu.defaultPosition = hireMenu.GetComponent<RectTransform>().anchoredPosition;
        employeesMenu.defaultPosition = employeesMenu.GetComponent<RectTransform>().anchoredPosition;
        thiefMenu.defaultPosition = thiefMenu.GetComponent<RectTransform>().anchoredPosition;
}

    #region PhaseGUI Methods
    public void PreparationPhaseGUI ()
    {
        clock.ResetClock();

        newsboardMenu.ResetBoard();

        if (!blackPanel.gameObject.activeSelf) blackPanel.gameObject.SetActive(true);
        blackPanel.FadeToMissive();

        CameraManager.instance.controlsEnabled = true;

        if(PhaseManager.instance.turn > 1) buttonRack.DOAnchorPos(new Vector2(buttonRack.anchoredPosition.x - buttonRackMovementAmount.x, buttonRack.anchoredPosition.y - buttonRackMovementAmount.y), animationDuration);

        clientInfos.Close();
    }

    public void ServicePhaseGUI ()
    {
        readyButton.Close();

        clock.StartRotatingClock();

        restockMenu.UpdateContent();

        buttonRack.DOAnchorPos(new Vector2(buttonRack.anchoredPosition.x + buttonRackMovementAmount.x, buttonRack.anchoredPosition.y + buttonRackMovementAmount.y),animationDuration);
    }

    public void RecapitulationPhaseGUI()
    {
        CameraManager.instance.controlsEnabled = false;
        blackPanel.gameObject.SetActive(true);
        blackPanel.FadeToPanel();
    }
    #endregion

    public void UpdateMoneyTooltip()
    {
        clientsMoney.text = PlayerManager.instance.clientsMoney.ToString();
        furnitureMoney.text = PlayerManager.instance.furnitureMoney.ToString();
        employeeMoney.text = PlayerManager.instance.employeeMoney.ToString();
        restockMoney.text = PlayerManager.instance.restockMoney.ToString();
        thievesMoney.text = PlayerManager.instance.thiefMoney.ToString();
    }

    public void DebugInfo(bool yesno)
    {
        debugInfo.SetActive(yesno);
    }

    public void CloseCurrentMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.content.SetActive(false);
            Tooltip.instance.group.alpha = 0;
            cursorsBank.Basic();
            currentMenu = null;
        }
    }
    public void OpenMenu(Menu menuToOpen)
    {
        CloseCurrentMenu();
        menuToOpen.content.SetActive(true);
        currentMenu = menuToOpen;
        currentMenu.OnOpening();
    }

    public void RessourcePopup(Vector3 worldPosition, int value, Color backgroundColor, Sprite icon = null)
    {
        var temp = Instantiate(ressourcePopupPrefab, canvas.transform);
        temp.GetComponent<RessourcePopup>().Popup(worldPosition, value, backgroundColor, icon);
    }
}