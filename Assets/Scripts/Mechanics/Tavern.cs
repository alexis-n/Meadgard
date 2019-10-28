using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[System.Serializable]
public class TavernValues
{
    public int[] tablesLevel,
        countersLevel,
        shelvesLevel,
        furnacesLevel;
}

public class Tavern : MonoBehaviour, ISaveable
 {
    private TavernValues tavernValues;

    public List<Transform> freeWaitressIdlePositions = new List<Transform>();
    public List<Transform> takenWaitressIdlePositions = new List<Transform>();
    public List<Transform> bouncersIdlePositions = new List<Transform>();

    public Stage stage;

    public Furnace[] furnaces;
    public Table[] tables;
    public Counter[] counters;
    public Shelf[] shelves;

    public List<Chair> availableRestaurantChairs = new List<Chair>();
    public List<Chair> availableStageChairs = new List<Chair>();
    public List<GameObject> shelfProps = new List<GameObject>();
    public List<GameObject> thiefProps = new List<GameObject>();
    public GameObject reserve;

    public List<Trash> unactiveTrash = new List<Trash>();
    public List<Trash> activeTrash = new List<Trash>();

    public God god;

    public GameObject employeeParent, tavernEntrance;
    public NavMeshSurface[] surfaces;

    private void Awake()
    {
        for (int i = 0; i < unactiveTrash.Count; i++)
        {
            SelectionManager.instance.ShowTrash.AddListener(unactiveTrash[i].SelectableVisual);
            SelectionManager.instance.HideSelectableElements.AddListener(unactiveTrash[i].SelectableVisual);
        }
        UpdateShelvesProps();
        UIManager.instance.godIcon.GetComponent<Image>().sprite = god.sprite;

        AddListeners();
    }

    public void AddListeners()
    {
        GameSaver.instance.savingGame.AddListener(SaveValues);
        GameSaver.instance.loadingFurnitures.AddListener(LoadValues);
    }
    public void SaveValues()
    {
        int[] tempTable = new int[tables.Length];
        for (int i = 0; i < tables.Length; i++)
        {
            tempTable[i] = tables[i].level;
        }
        tavernValues.tablesLevel = tempTable;

        int[] tempCounters = new int[counters.Length];
        for (int i = 0; i < counters.Length; i++)
        {
            tempCounters[i] = counters[i].level;
        }
        tavernValues.countersLevel = tempCounters;

        int[] tempShelves = new int[shelves.Length];
        for (int i = 0; i < shelves.Length; i++)
        {
            tempShelves[i] = shelves[i].level;
        }
        tavernValues.shelvesLevel = tempShelves;

        int[] tempFurnaces = new int[furnaces.Length];
        for (int i = 0; i < furnaces.Length; i++)
        {
            tempFurnaces[i] = furnaces[i].level;
        }
        tavernValues.furnacesLevel = tempFurnaces;

        GameSaver.instance.gameSave.tavernValues = tavernValues;
    }
    public void LoadValues()
    {
        tavernValues = GameSaver.instance.gameSave.tavernValues;

        for (int i = 0; i < tables.Length; i++)
        {
            tables[i].level = tavernValues.tablesLevel[i];
            tables[i].Build();
        }
        for (int i = 0; i < counters.Length; i++)
        {
            counters[i].level = tavernValues.countersLevel[i];
            counters[i].Build();
        }
        for (int i = 0; i < shelves.Length; i++)
        {
            shelves[i].level = tavernValues.shelvesLevel[i];
            shelves[i].Build();
        }
        for (int i = 0; i < furnaces.Length; i++)
        {
            furnaces[i].level = tavernValues.furnacesLevel[i];
            furnaces[i].Build();
        }
    }

    public void NewAvailableChair(Chair chair)
    {
        if (NPCManager.instance.clientsInQueue.Count > 0) NPCManager.instance.clientsInQueue[0].FoundChair(chair);
        else availableRestaurantChairs.Add(chair);
    }

    public void BakeTavernSurfaces()
	{
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }

    public void TrashSpawner(float probability)
    {
        if (Random.Range(0f, 1f) < probability && unactiveTrash.Count > 0)
        {
            var trashToTurnOn = unactiveTrash[Random.Range(0, unactiveTrash.Count -1)];
            trashToTurnOn.gameObject.SetActive(true);

            unactiveTrash.Remove(trashToTurnOn);
            activeTrash.Add(trashToTurnOn);

            PlayerManager.instance.PlayerCleanliness -= trashToTurnOn.trashData.dirtyValue;
        }
    }

    public void UpdateShelvesProps()
    {
        int temp = (PlayerManager.instance.PlayerFood + PlayerManager.instance.PlayerDrinks)/2;
        for (int i = 0; i < shelfProps.Count; i++)
        {
            if (i <= temp) shelfProps[i].SetActive(true);
            else shelfProps[i].SetActive(false);
        }
    }
}
