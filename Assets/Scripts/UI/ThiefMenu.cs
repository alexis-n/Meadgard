using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class GuildValues
{
    public ThiefValues[] thievesValue = new ThiefValues[3];
}

public class ThiefMenu : Menu, ISaveable, ITutorialable
{
    public static ThiefMenu instance;
    private GuildValues guildValues = new GuildValues();
    [SerializeField] private List<TutorialStep> steps = new List<TutorialStep>();

    public ThiefMission[] missions;
    public Thief[] thieves;

    [SerializeField] private Scrollbar scrollbar;

    public static Thief counterpsy = null;

    private void Awake()
    {
        instance = this;
        AddListeners();
        if (content.activeSelf) content.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = GetComponent<RectTransform>().anchoredPosition;
        PhaseManager.instance.prepPhaseEvent.AddListener(ThiefSpawner);
        PhaseManager.instance.servicePhaseEvent.AddListener(LaunchMissions);
        PhaseManager.instance.serviceEndEvent.AddListener(ThievesOvernight);
    }

    private void Update()
    {
        foreach (var item in missions)
        {
            if (item.countdown)
            {
                item.timeLeft -= Time.deltaTime;

                if (item.timeLeft <= 0)
                {
                    item.countdown = false;
                    item.EndMission();
                }
            }
        }
    }

    public void StartTutorial()
    {
        if (!foldToggle) FoldAndUnfold();
        Tutorial.instance.StartTutorial(transform, steps);
    }

    public void AddListeners()
    {
        GameSaver.instance.savingGame.AddListener(SaveValues);
        GameSaver.instance.loadingPlayer.AddListener(LoadValues);
    }
    public void SaveValues()
    {
        for (int i = 0; i < thieves.Length; i++)
        {
            guildValues.thievesValue[i] = thieves[i].thiefValues;
        }

        GameSaver.instance.gameSave.guildValues = guildValues;
    }
    public void LoadValues()
    {
        guildValues = GameSaver.instance.gameSave.guildValues;

        for (int i = 0; i < guildValues.thievesValue.Length; i++)
        {
            thieves[i].thiefValues = guildValues.thievesValue[i];
        }
    }

    public override void OnOpening()
    {
        GetComponent<RectTransform>().anchoredPosition = defaultPosition;

        if (!foldToggle) FoldAndUnfold();

        UpdateContent();

        scrollbar.value = 1;
    }

    public void ThiefSpawner()
    {
        int thievesToResurrect = 1;

        for (int i = 0; i < thieves.Length; i++)
        {
            if (thieves[i].thiefValues.thiefInMission)
            {
                missions[thieves[i].thiefValues.thiefMissionIndex].assignedThief = thieves[i];
                missions[thieves[i].thiefValues.thiefMissionIndex].EndMission();
            }
            else if (thievesToResurrect > 0 && thieves[i].thiefValues.thiefHealth < 1)
            {
                thieves[i].thiefValues = new ThiefValues()
                {
                    thiefName = NPCManager.instance.nameGenerator.Thief(),
                    thiefMissionIndex = -1,
                    thiefInMission = false,
                    thiefHealth = 3,
                    thiefSkills = new int[3] { 1, 1, 1 }
                };

                thieves[i].locked = false;
                thievesToResurrect--;
            }

            if (thieves[i].thiefValues.thiefHealth > 0)
            {
                thieves[i].gameObject.SetActive(true);
                thieves[i].missionIcon.gameObject.SetActive(true);
            }
            else
            {
                thieves[i].gameObject.SetActive(false);
                thieves[i].missionIcon.gameObject.SetActive(false);
            }
        }
    }

    public void LaunchMissions()
    {
        for (int i = 0; i < thieves.Length; i++)
        {
            if (thieves[i].thiefValues.thiefInMission)
                missions[thieves[i].thiefValues.thiefMissionIndex].StartMission();
        }
    }

    public void ThievesOvernight()
    {
        for (int i = 0; i < thieves.Length; i++)
        {
            if (thieves[i].thiefValues.thiefInMission)
            {
                if (missions[thieves[i].thiefValues.thiefMissionIndex].baseTime == 0) missions[thieves[i].thiefValues.thiefMissionIndex].EndMission();
                else
                {
                    missions[thieves[i].thiefValues.thiefMissionIndex].countdown = false;
                    missions[thieves[i].thiefValues.thiefMissionIndex].barFillerTween.Pause();
                    thieves[i].HideUIBar(false);
                    missions[thieves[i].thiefValues.thiefMissionIndex].countdownText.gameObject.SetActive(true);
                    missions[thieves[i].thiefValues.thiefMissionIndex].countdownText.text = "Reviendra le lendemain";
                }
            }
        }
    }
}
