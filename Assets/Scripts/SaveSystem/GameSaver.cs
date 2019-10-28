using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameSave
{
    #region General Values
    public string gameDate, gameTime;
    public int gameTurn, gameGodIndex;
    #endregion

    public PlayerValues playerValues = new PlayerValues();
    public TavernValues tavernValues = new TavernValues();
    public NPCValues npcValues = new NPCValues();
    public GuildValues guildValues = new GuildValues();
}


public class GameSaver : MonoBehaviour
{
    public static GameSaver instance;
    public GameSave gameSave;
    public GameplayData data;

    public UnityEvent savingGame = new UnityEvent();
    public UnityEvent loadingGeneral = new UnityEvent(),
        loadingFurnitures = new UnityEvent(),
        loadingPlayer = new UnityEvent(),
        loadingEmployees = new UnityEvent(),
        loadingGuild = new UnityEvent();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this);
        }
    }

    public GameSave NewGame(int godIndex)
    {
        GameSave newSave = new GameSave()
        {
            gameTurn = 1,
            gameGodIndex = godIndex,

            playerValues = new PlayerValues()
            {
                playerMoney = GameManager.instance.data.startAmountMoney,
                playerFood = GameManager.instance.data.startAmountFood,
                playerDrinks = GameManager.instance.data.startAmountDrinks,
                playerPopularity = GameManager.instance.data.startAmountPopularity
            },

            tavernValues = new TavernValues()
            {
                tablesLevel = GameManager.instance.data.tablesLevel,
                countersLevel = GameManager.instance.data.countersLevel,
                shelvesLevel = GameManager.instance.data.shelvesLevel,
                furnacesLevel = GameManager.instance.data.furnacesLevel
            },

            npcValues = new NPCValues()
            {
                waitressesValues = new System.Collections.Generic.List<EmployeeValues>(),
                cookValues = new System.Collections.Generic.List<EmployeeValues>(),
                bouncerValues = new System.Collections.Generic.List<EmployeeValues>(),
                bardValues = new System.Collections.Generic.List<EmployeeValues>()
            },

            guildValues = new GuildValues()
            {
                thievesValue = new ThiefValues[3]
            }
        };

        foreach (var item in data.startingWaitresses)
        {
            EmployeeValues copy = new EmployeeValues()
            {
                employeeName = item.employeeName,
                employeeExperience = item.employeeExperience,
                employeeLevel = item.employeeLevel,
                employeeLevelup = item.employeeLevelup,
                employeeRarity = item.employeeRarity,
                employeeType = item.employeeType,
                employeeWage = item.employeeWage,
                employeeRecruitmentFee = item.employeeRecruitmentFee,
                employeeSkillPoints = item.employeeSkillPoints,
                employeeSkills = new int[3] { item.employeeSkills[0], item.employeeSkills[1], item.employeeSkills[2] },
            };
            newSave.npcValues.waitressesValues.Add(copy);
        }
        foreach (var item in data.startingCooks)
        {
            EmployeeValues copy = new EmployeeValues()
            {
                employeeName = item.employeeName,
                employeeExperience = item.employeeExperience,
                employeeLevel = item.employeeLevel,
                employeeLevelup = item.employeeLevelup,
                employeeRarity = item.employeeRarity,
                employeeType = item.employeeType,
                employeeWage = item.employeeWage,
                employeeRecruitmentFee = item.employeeRecruitmentFee,
                employeeSkillPoints = item.employeeSkillPoints,
                employeeSkills = new int[3] { item.employeeSkills[0], item.employeeSkills[1], item.employeeSkills[2] },
            };
            newSave.npcValues.cookValues.Add(copy);
        }
        foreach (var item in data.startingBouncers)
        {
            EmployeeValues copy = new EmployeeValues()
            {
                employeeName = item.employeeName,
                employeeExperience = item.employeeExperience,
                employeeLevel = item.employeeLevel,
                employeeLevelup = item.employeeLevelup,
                employeeRarity = item.employeeRarity,
                employeeType = item.employeeType,
                employeeWage = item.employeeWage,
                employeeRecruitmentFee = item.employeeRecruitmentFee,
                employeeSkillPoints = item.employeeSkillPoints,
                employeeSkills = new int[3] { item.employeeSkills[0], item.employeeSkills[1], item.employeeSkills[2] },
            };
            newSave.npcValues.bouncerValues
.Add(copy);
        }
        foreach (var item in data.startingBards)
        {
            EmployeeValues copy = new EmployeeValues()
            {
                employeeName = item.employeeName,
                employeeExperience = item.employeeExperience,
                employeeLevel = item.employeeLevel,
                employeeLevelup = item.employeeLevelup,
                employeeRarity = item.employeeRarity,
                employeeType = item.employeeType,
                employeeWage = item.employeeWage,
                employeeRecruitmentFee = item.employeeRecruitmentFee,
                employeeSkillPoints = item.employeeSkillPoints,
                employeeSkills = new int[3] { item.employeeSkills[0], item.employeeSkills[1], item.employeeSkills[2] },
            };
            newSave.npcValues.bardValues.Add(copy);
        }
        for (int i = 0; i < data.startingThieves.Length; i++)
        {
            ThiefValues copy = new ThiefValues()
            {
                thiefName = GameManager.instance.data.startingThieves[i].thiefName,
                thiefHealth = GameManager.instance.data.startingThieves[i].thiefHealth,
                thiefMissionIndex = GameManager.instance.data.startingThieves[i].thiefMissionIndex,
                thiefMissionChance = GameManager.instance.data.startingThieves[i].thiefMissionChance,
                thiefInMission = GameManager.instance.data.startingThieves[i].thiefInMission,
                thiefSkills = new int[3] { GameManager.instance.data.startingThieves[i].thiefSkills[0],
                    GameManager.instance.data.startingThieves[i].thiefSkills[1],
                    GameManager.instance.data.startingThieves[i].thiefSkills[2] }
            };
            newSave.guildValues.thievesValue[i] = copy;
        }

        return newSave;
    }

    public void SaveGame()
    {
        savingGame.Invoke();
        DateTime dateTime = System.DateTime.Now;
        gameSave.gameDate = dateTime.ToString("dd/MM/yyyy");
        gameSave.gameTime = dateTime.ToString("HH:mm:ss");
        gameSave.gameTurn = PhaseManager.instance.turn + 1;
        gameSave.gameGodIndex = System.Array.IndexOf(GameManager.instance.data.gods, GameManager.god);

        string gameToJson = JsonUtility.ToJson(gameSave);
        PlayerPrefs.SetString("gameSave", gameToJson);
    }
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("gameSave"))
        {
            string jsonToGame = PlayerPrefs.GetString("gameSave");
            gameSave = JsonUtility.FromJson<GameSave>(jsonToGame);
        }
        else
        {
            gameSave = new GameSave();
        }
    }

    public void ReadSave()
    {
        StartCoroutine(Loading());
    }
    IEnumerator Loading()
    {
        loadingGeneral.Invoke();
        yield return null;
        loadingFurnitures.Invoke();
        yield return null;
        loadingPlayer.Invoke();
        yield return null;
        loadingEmployees.Invoke();
        yield return null;
        loadingGuild.Invoke();
    }
}
