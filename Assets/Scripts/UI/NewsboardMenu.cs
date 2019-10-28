using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewsboardMenu : Menu
{
    #region /// VARIABLES ///
    public bool hasBoughtFood = false, hasBoughtDrinks = false, hasHiredNewStaff = false, hasLeveledUpStaff = true, hasUpgradedFurniture = false, hasSentThief = false;
    [SerializeField] private GameObject[] papers, tornPapers;
    private Furniture tableToUpgrade;


    [SerializeField] private GameObject[] bounties;

    [SerializeField] private TextMeshProUGUI[] bountiesTexts;
    [SerializeField] [TextArea] private string wantedString, caughtString, escapedString;

    [SerializeField] private Image[] bountiesImages;
    [SerializeField] private Sprite wantedSprite, caughtSprite, escapedSprite;
    #endregion

    //Fonction qui ouvre ou ferme le menu de restockage
    public override void OnOpening()
    {
        base.OnOpening();
    }

    //Fonction qui met à jour le menu de restockage
    public override void UpdateContent()
    {
        base.UpdateContent();

        papers[0].SetActive(!hasBoughtFood);
        tornPapers[0].SetActive(hasBoughtFood);

        papers[1].SetActive(!hasBoughtDrinks);
        tornPapers[1].SetActive(hasBoughtDrinks);

        papers[2].SetActive(!hasHiredNewStaff);
        tornPapers[2].SetActive(hasHiredNewStaff);

        papers[3].SetActive(!hasLeveledUpStaff);
        tornPapers[3].SetActive(hasLeveledUpStaff);

        papers[4].SetActive(!hasUpgradedFurniture);
        tornPapers[4].SetActive(hasUpgradedFurniture);

        papers[5].SetActive(!hasSentThief);
        tornPapers[5].SetActive(hasSentThief);

        for (int i = 0; i < NPCManager.instance.bandits.Length; i++)
        {
            if (NPCManager.instance.bandits[i] != null)
            {
                bounties[i].SetActive(true);
                switch(NPCManager.instance.bandits[i].state)
                {
                    case Bandit.BanditState.Wanted:
                        bountiesTexts[i].text = wantedString;
                        bountiesTexts[i].text = bountiesTexts[i].text.Replace("x", NPCManager.instance.bandits[i].bounty.ToString());
                        bountiesImages[i].sprite = wantedSprite;
                        break;
                    case Bandit.BanditState.Escaped:
                        bountiesTexts[i].text = escapedString;
                        bountiesTexts[i].text = bountiesTexts[i].text.Replace("x", NPCManager.instance.bandits[i].amountStolen.ToString());
                        bountiesImages[i].sprite = escapedSprite;
                        break;
                    case Bandit.BanditState.Caught:
                        bountiesTexts[i].text = caughtString;
                        bountiesImages[i].sprite = caughtSprite;
                        break;
                }
            }
            else bounties[i].SetActive(false);
        }
    }

    public void ResetBoard()
    {
        if (PlayerManager.instance.PlayerFood != PlayerManager.instance.PlayerFoodCapacity) hasBoughtFood = false;
        if (PlayerManager.instance.PlayerDrinks != PlayerManager.instance.PlayerDrinksCapacity) hasBoughtDrinks = false;

        if (NPCManager.instance.hiredCooks.Count != NPCManager.instance.maxCooks || NPCManager.instance.hiredWaitresses.Count != NPCManager.instance.maxWaitresses) hasHiredNewStaff = false;
        else hasLeveledUpStaff = true;

        hasLeveledUpStaff = true;
        for (int i = 0; i < NPCManager.instance.hiredCooks.Count; i++)
        {
            if (NPCManager.instance.hiredCooks[i].employeeValues.employeeLevelup)
            {
                hasLeveledUpStaff = false;
                break;
            }
        }
        for (int i = 0; i < NPCManager.instance.hiredWaitresses.Count; i++)
        {
            if (NPCManager.instance.hiredWaitresses[i].employeeValues.employeeLevelup)
            {
                hasLeveledUpStaff = false;
                break;
            }
        }

        hasUpgradedFurniture = false;
        for (int i = 0; i < PlayerManager.instance.tavern.tables.Length; i++)
        {
            if (PlayerManager.instance.tavern.tables[i].level != PlayerManager.instance.tavern.tables[i].maxLevel)
            {
                hasUpgradedFurniture = false;
                tableToUpgrade = PlayerManager.instance.tavern.tables[i];
                break;
            }
        }

        hasSentThief = false;
    }

    public void SeeUpgradableFurniture()
    {
        CameraManager.instance.ViewTarget(tableToUpgrade.gameObject);
        if (UIManager.instance.currentMenu == this) UIManager.instance.CloseCurrentMenu();
        SelectionManager.instance.SelectNewObject(tableToUpgrade);
    }
}
