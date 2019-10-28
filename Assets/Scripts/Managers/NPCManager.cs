using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using DG.Tweening;

[System.Serializable]
public class NPCValues
{
    public List<EmployeeValues> waitressesValues = new List<EmployeeValues>(),
        cookValues = new List<EmployeeValues>(),
        bouncerValues = new List<EmployeeValues>(),
        bardValues = new List<EmployeeValues>();
}

public class NPCManager : MonoBehaviour, ISaveable
{
    #region /// VARIABLES ///
    public static NPCManager instance;
    private NPCValues npcValues; 

    public Transform[] queuePositions;
    public Transform pool;

    [SerializeField]
    private GameObject humanPrefab, dwarfPrefab, elfPrefab, jottunPrefab,
        waitressPrefab, cookPrefab, bouncerPrefab, bardPrefab, banditSmokePrefab, fightSmokePrefab;
    public NPCNames nameGenerator;

    #region Lists
    public List<EmployeeValues> applyingWaitresses = new List<EmployeeValues>();
    public List<Waitress> hiredWaitresses = new List<Waitress>();
    public int maxWaitresses = 4;

    public List<Order> cookOrdersList = new List<Order>();
    public List<EmployeeValues> applyingCooks = new List<EmployeeValues>();
    public List<Cook> hiredCooks = new List<Cook>();
    public int maxCooks = 3;

    public List<EmployeeValues> applyingBouncers = new List<EmployeeValues>();
    public List<Bouncer> hiredBouncers = new List<Bouncer>();
    public int maxBouncers = 2;

    public List<EmployeeValues> applyingBards = new List<EmployeeValues>();
    public List<Bard> hiredBards = new List<Bard>();
    public int maxBards = 4;

    public Bandit[] bandits = new Bandit[3];
    public List<Client> banditsSeated = new List<Client>();

    public List<Client> clientsInQueue = new List<Client>();
    public List<Client> clientsInRestaurant = new List<Client>();
    public List<Client> clientsAtStage = new List<Client>();
    public List<Client> clientsWaitingForDish = new List<Client>();

    public List<GameObject> tempDish = new List<GameObject>();

    public EmployeeButton[] employeeButtons;

    public UnityEvent purge = new UnityEvent();
    #endregion

    public GameObject bifrost, bifrostBeam;
    [SerializeField] private Light bifrostLight;
    [SerializeField] private float animatedIntensity;
    [SerializeField] private float startIntensity, bifrostBeamSpawnScale = 1;

    Vector3 spawnLocation;
    GameObject[] waitingPlaces;
    GameObject selectedClient;

    public bool mustBeStopping = false;

    [HideInInspector] public int bardGeneratedPopularity;
    #endregion

    private void Awake()
    {
        instance = this;
        AddListeners();
    }

    public void Start()
    {
        spawnLocation = bifrost.transform.position;
        startIntensity = bifrostLight.intensity;

        waitingPlaces = GameObject.FindGameObjectsWithTag("WaitingPlace");

        PhaseManager.instance.prepPhaseEvent.AddListener(GenerateApplyingEmployees);
        PhaseManager.instance.prepPhaseEvent.AddListener(GenerateBandits);
    }

    public void AddListeners()
    {
        GameSaver.instance.savingGame.AddListener(SaveValues);
        GameSaver.instance.loadingEmployees.AddListener(LoadValues);
    }
    public void SaveValues()
    {
        npcValues.waitressesValues.Clear();
        foreach (var item in hiredWaitresses)
        {
            npcValues.waitressesValues.Add(item.employeeValues);
        }

        npcValues.cookValues.Clear();
        foreach (var item in hiredCooks)
        {
            npcValues.cookValues.Add(item.employeeValues);
        }

        npcValues.bouncerValues.Clear();
        foreach (var item in hiredBouncers)
        {
            npcValues.bouncerValues.Add(item.employeeValues);
        }

        npcValues.bardValues.Clear();
        foreach (var item in hiredBards)
        {
            npcValues.bardValues.Add(item.employeeValues);
        }

        GameSaver.instance.gameSave.npcValues = npcValues;
    }
    public void LoadValues()
    {
        npcValues = GameSaver.instance.gameSave.npcValues;

        foreach (var item in npcValues.waitressesValues)
        {
            HireEmployee(item, false);
        }

        foreach (var item in npcValues.cookValues)
        {
            HireEmployee(item, false);
        }

        foreach (var item in npcValues.bouncerValues)
        {
            HireEmployee(item, false);
        }

        foreach (var item in npcValues.bardValues)
        {
            HireEmployee(item, false);
        }
    }

    //fonction qui lance la génération des clients pour la phase de service
    public void ServicePhaseStart()
    {
        bardGeneratedPopularity = 0;
        mustBeStopping = false;
        StartCoroutine(ClientGenerator());
        for (int i = 0; i < bandits.Length; i++)
        {
            if (bandits[i] != null) StartCoroutine(BanditGenerator(bandits[i]));
        }
    }
    IEnumerator ClientGenerator()
    {
        yield return new WaitForSeconds(1f);
        while (!mustBeStopping)
        {
            GenerateClient();
            Vector3 minMinmaxMaxClientSpawnTime = GameManager.instance.data.minMinmaxMaxClientSpawnTime;
            float timeToWait = Random.Range(minMinmaxMaxClientSpawnTime.x,
                (Mathf.Lerp(minMinmaxMaxClientSpawnTime.z, minMinmaxMaxClientSpawnTime.y, PlayerManager.instance.PlayerPopularity/100f)) * Missive.currentMissive.overallSpawnRate);
            yield return new WaitForSeconds(timeToWait);
        }
    }
    //fonction qui génère un client aléatoirement selon les chances de spawn propres aux races
    public void GenerateClient(Bandit bandit = null)
    {
        if (clientsInQueue.Count < queuePositions.Length)
        {
            float[] tempChances = new float[] {
                GameManager.instance.data.raceChances[0] * Missive.currentMissive.humanSpawnRate,
                GameManager.instance.data.raceChances[1] * Missive.currentMissive.dwarfSpawnRate,
                GameManager.instance.data.raceChances[2] * Missive.currentMissive.elfSpawnRate,
                GameManager.instance.data.raceChances[3] * Missive.currentMissive.jottunSpawnRate
            };
            int chosenRace = Choose(tempChances);
            GameObject myClientObject = null;
            Client client = new Client();
            switch (chosenRace)
            {
                case 0:
                    myClientObject = Instantiate(humanPrefab, new Vector3(spawnLocation.x, spawnLocation.y, spawnLocation.z), Quaternion.identity, pool);
                    client = myClientObject.GetComponent<Client>();
                    client.name = nameGenerator.HumanMale();
                    break;
                case 1:
                    myClientObject = Instantiate(dwarfPrefab, new Vector3(spawnLocation.x, spawnLocation.y, spawnLocation.z), Quaternion.identity, pool);
                    client = myClientObject.GetComponent<Client>();
                    client.name = nameGenerator.Dwarf();
                    break;
                case 2:
                    myClientObject = Instantiate(elfPrefab, new Vector3(spawnLocation.x, spawnLocation.y, spawnLocation.z), Quaternion.identity, pool);
                    client = myClientObject.GetComponent<Client>();
                    client.name = nameGenerator.Elf();
                    break;
                case 3:
                    myClientObject = Instantiate(jottunPrefab, new Vector3(spawnLocation.x, spawnLocation.y, spawnLocation.z), Quaternion.identity, pool);
                    client = myClientObject.GetComponent<Client>();
                    client.name = nameGenerator.Jottun();
                    break;
            }

            //... on fait déplacer le client vers sa place dans la file d'attente et on lui assigne la-dite place, jusqu'à laquelle il va se déplacer.
            client.CheckForCelebrities();

            bifrostLight.DOIntensity(animatedIntensity, 0.2f)
                .OnComplete(() => client.navMeshAgent.Warp(client.gameObject.transform.position))
                .OnComplete(() => bifrostLight.DOIntensity(startIntensity, 0.5f));

            if (bandit != null) client.bandit = bandit;

            client.npcBubble.InstantiateBubble(client);
            client.npcBubble.bubble.gameObject.SetActive(false);

            purge.AddListener(client.Autodestroy);
        }
    }
    public void UpdateQueue()
    {
        foreach (var item in NPCManager.instance.clientsInQueue)
        {
            item.GoTo(queuePositions[clientsInQueue.IndexOf(item)].position);
        }
    }
    public void GenerateSmokeBomb(Vector3 position) { Destroy(Instantiate(banditSmokePrefab, position, banditSmokePrefab.transform.rotation), 3f); }
    public void GenerateFight(Vector3 position) { Destroy(Instantiate(fightSmokePrefab, position, banditSmokePrefab.transform.rotation), 6f); }

    //fonction qui génère les employés à recruter durant ce tour
    public void GenerateApplyingEmployees ()
    {
        applyingCooks.Clear();
        applyingWaitresses.Clear();
        applyingBouncers.Clear();
        applyingBards.Clear();

        StartCoroutine(ApplyingEmployeesGenerator());
    }
    IEnumerator ApplyingEmployeesGenerator()
    {
        for (int i = 0; i < maxWaitresses; i++)
        {
            GenerateEmployee(Employee.EmployeeType.Waitress);
            yield return null;
        }
        for (int i = 0; i < maxCooks; i++)
        {
            GenerateEmployee(Employee.EmployeeType.Cook);
            yield return null;
        }
        for (int i = 0; i < maxBouncers; i++)
        {
            GenerateEmployee(Employee.EmployeeType.Bouncer);
            yield return null;
        }
        for (int i = 0; i < maxBards; i++)
        {
            GenerateEmployee(Employee.EmployeeType.Bard);
            yield return null;
        }
    }
    public void GenerateEmployee(Employee.EmployeeType type)
    {
        int rarityIndex = Choose(GameManager.instance.data.rarityChances);

        EmployeeValues employee = new EmployeeValues()
        {
            employeeLevel = 1,
            employeeRarity = (Employee.Rarity)rarityIndex,
            employeeRecruitmentFee = 20 + (rarityIndex * 3),
            employeeWage = 5 + rarityIndex,
            employeeSkills = new int[3] { 1, 1, 1 },
        };

        while (rarityIndex > 0)
        {
            employee.employeeSkills[Random.Range(0, employee.employeeSkills.Length)]++;
            rarityIndex--;
        }

        switch (type)
        {   
            case Employee.EmployeeType.Waitress:
                employee.employeeName = nameGenerator.HumanFemale();
                employee.employeeType = Employee.EmployeeType.Waitress;

                employee.employeeRecruitmentFee = (int)(employee.employeeRecruitmentFee * Missive.currentMissive.waitressFee);
                employee.employeeWage = (int)(employee.employeeWage * Missive.currentMissive.waitressSalary);

                applyingWaitresses.Add(employee);
                break;
            case Employee.EmployeeType.Cook:
                employee.employeeName = nameGenerator.Dwarf();
                employee.employeeType = Employee.EmployeeType.Cook;

                employee.employeeRecruitmentFee = (int)(employee.employeeRecruitmentFee * Missive.currentMissive.cookFee);
                employee.employeeWage = (int)(employee.employeeWage * Missive.currentMissive.cookSalary);

                applyingCooks.Add(employee);
                break;
            case Employee.EmployeeType.Bouncer:
                employee.employeeName = nameGenerator.Jottun();
                employee.employeeType = Employee.EmployeeType.Bouncer;

                employee.employeeRecruitmentFee = (int)(employee.employeeRecruitmentFee * Missive.currentMissive.bouncerFee);
                employee.employeeWage = (int)(employee.employeeWage * Missive.currentMissive.bouncerSalary);

                applyingBouncers.Add(employee);
                break;
            case Employee.EmployeeType.Bard:
                employee.employeeName = nameGenerator.Elf();
                employee.employeeType = Employee.EmployeeType.Bard;

                employee.employeeRecruitmentFee = (int)(employee.employeeRecruitmentFee * Missive.currentMissive.bardFee);
                employee.employeeWage = (int)(employee.employeeWage * Missive.currentMissive.bardSalary);

                applyingBards.Add(employee);
                break;
            default:
                break;
        }
    }
    //fonction qui permet d'embaucher un des employés de la pool
    public void HireEmployee (EmployeeValues givenValues, bool playerBought = true)
    {
        Employee employee = new Employee();

        switch (givenValues.employeeType)
        {
            case Employee.EmployeeType.Waitress:
                if (hiredWaitresses.Count < maxWaitresses)
                {
                    var spawner = PlayerManager.instance.tavern.freeWaitressIdlePositions[Random.Range(0, PlayerManager.instance.tavern.freeWaitressIdlePositions.Count)];
                    PlayerManager.instance.tavern.freeWaitressIdlePositions.Remove(spawner);
                    PlayerManager.instance.tavern.takenWaitressIdlePositions.Add(spawner);

                    var temp = Instantiate(waitressPrefab, spawner.position, Quaternion.identity, pool);

                    employee = temp.GetComponent<Employee>();
                    employee.idlePosition = spawner;

                    var waitress = employee.GetComponent<Waitress>();

                    if (applyingWaitresses.Contains(givenValues) && playerBought) applyingWaitresses.Remove(givenValues);
                    hiredWaitresses.Add(waitress);

                    employee.employeeValues.diamondColorIndex = hiredWaitresses.IndexOf(waitress);
                    employee.diamondColor = employee.employeeData.diamondColors[employee.employeeValues.diamondColorIndex];

                    for (int i = 0; i < employeeButtons.Length; i++)
                    {
                        if (employeeButtons[i].employee == null && employeeButtons[i].employeeType == Employee.EmployeeType.Waitress)
                        {
                            employeeButtons[i].employee = employee;
                            employeeButtons[i].GetComponent<Tooltipable>().nameTag = employee.employeeValues.employeeName;
                            employeeButtons[i].gameObject.SetActive(true);
                            break;
                        }
                    }
                }
                else return;
                break;
            case Employee.EmployeeType.Cook:
                if (hiredCooks.Count < maxCooks)
                {
                    Furnace furnace = new Furnace();
                    for (int i = 0; i < PlayerManager.instance.tavern.furnaces.Length; i++)
                    {
                        if (!PlayerManager.instance.tavern.furnaces[i].taken)
                        {
                            furnace = PlayerManager.instance.tavern.furnaces[i];
                            furnace.taken = true;
                            break;
                        }
                    }

                    var temp = Instantiate(cookPrefab, furnace.cookSpawner.position, Quaternion.identity, pool);

                    employee = temp.GetComponent<Employee>();
                    employee.idlePosition = furnace.cookSpawner;

                    var cook = employee.GetComponent<Cook>();
                    cook.idlePosition = furnace.cookSpawner;
                    cook.transform.position = cook.idlePosition.transform.position;
                    cook.navMeshAgent.Warp(cook.idlePosition.transform.position);
                    cook.furnace = furnace;

                    if (applyingCooks.Contains(givenValues) && playerBought) applyingCooks.Remove(givenValues);
                    hiredCooks.Add(cook);

                    employee.employeeValues.diamondColorIndex = hiredCooks.IndexOf(cook);
                    employee.diamondColor = employee.employeeData.diamondColors[employee.employeeValues.diamondColorIndex];

                    for (int i = 0; i < employeeButtons.Length; i++)
                    {
                        if (employeeButtons[i].employee == null && employeeButtons[i].employeeType == Employee.EmployeeType.Cook)
                        {
                            employeeButtons[i].employee = employee;
                            employeeButtons[i].GetComponent<Tooltipable>().nameTag = employee.employeeValues.employeeName;
                            employeeButtons[i].gameObject.SetActive(true);
                            break;
                        }
                    }
                }
                else return;
                break;
            case Employee.EmployeeType.Bouncer:
                if (hiredBouncers.Count < maxBouncers)
                {
                    var spawner = PlayerManager.instance.tavern.bouncersIdlePositions[Random.Range(0, PlayerManager.instance.tavern.bouncersIdlePositions.Count)];
                    PlayerManager.instance.tavern.bouncersIdlePositions.Remove(spawner);

                    var temp = Instantiate(bouncerPrefab, spawner.position, Quaternion.identity, pool);

                    employee = temp.GetComponent<Employee>();
                    employee.idlePosition = spawner;

                    var bouncer = employee.GetComponent<Bouncer>();

                    if (applyingBouncers.Contains(givenValues) && playerBought) applyingBouncers.Remove(givenValues);
                    hiredBouncers.Add(bouncer);

                    employee.employeeValues.diamondColorIndex = hiredBouncers.IndexOf(bouncer);
                    employee.diamondColor = employee.employeeData.diamondColors[employee.employeeValues.diamondColorIndex];

                    for (int i = 0; i < employeeButtons.Length; i++)
                    {
                        if (employeeButtons[i].employee == null && employeeButtons[i].employeeType == Employee.EmployeeType.Bouncer)
                        {
                            employeeButtons[i].employee = employee;
                            employeeButtons[i].GetComponent<Tooltipable>().nameTag = employee.employeeValues.employeeName;
                            employeeButtons[i].gameObject.SetActive(true);
                            break;
                        }
                    }
                }
                else return;
                break;
            case Employee.EmployeeType.Bard:
                if (hiredBards.Count < maxBards)
                {
                    var spawner = PlayerManager.instance.tavern.stage.bardPositions[0];
                    PlayerManager.instance.tavern.stage.bardPositions.Remove(spawner);

                    var temp = Instantiate(bardPrefab, spawner.position, Quaternion.identity, pool);

                    employee = temp.GetComponent<Employee>();
                    employee.idlePosition = spawner;

                    var bard = employee.GetComponent<Bard>();
                    bard.transform.localEulerAngles = bard.idlePosition.localEulerAngles;

                    if (applyingBards.Contains(givenValues) && playerBought) applyingBards.Remove(givenValues);
                    hiredBards.Add(bard);

                    employee.employeeValues.diamondColorIndex = hiredBards.IndexOf(bard);
                    employee.diamondColor = employee.employeeData.diamondColors[employee.employeeValues.diamondColorIndex];

                    bard.instruments[hiredBards.IndexOf(bard)].SetActive(true);
                    AudioManager.instance.playBards[hiredBards.IndexOf(bard)] = true;
                    bard.animator.SetInteger("Instrument", hiredBards.IndexOf(bard));

                    for (int i = 0; i < employeeButtons.Length; i++)
                    {
                        if (employeeButtons[i].employee == null && employeeButtons[i].employeeType == Employee.EmployeeType.Bard)
                        {
                            employeeButtons[i].employee = employee;
                            employeeButtons[i].GetComponent<Tooltipable>().nameTag = employee.employeeValues.employeeName;
                            employeeButtons[i].gameObject.SetActive(true);
                            break;
                        }
                    }
                }
                else return;
                break;
            default:
                break;
        }

        employee.employeeValues = givenValues;
        employee.name = givenValues.employeeName;

        employee.navMeshAgent.Warp(employee.idlePosition.transform.position);
        UIManager.instance.hireMenu.UpdateContent();

        if (playerBought)
        {
            PlayerManager.instance.PlayerMoney -= givenValues.employeeRecruitmentFee;
            PlayerManager.instance.employeeMoney -= givenValues.employeeRecruitmentFee;
            UIManager.instance.blackPanel.recapPanel.recruitmentValue -= givenValues.employeeRecruitmentFee;
        }

        employee.npcBubble.InstantiateBubble(employee);
        employee.npcBubble.bubble.icon.sprite = employee.employeeData.skillIcons[0];
    }
    //fonction qui permet de virer un employé
    public void FireEmployee(Employee employee)
    {
        for (int i = 0; i < employeeButtons.Length; i++)
        {
            if (employeeButtons[i].employee == employee)
            {
                employeeButtons[i].employee = null;
                employeeButtons[i].GetComponent<Tooltipable>().nameTag = string.Empty;
                employeeButtons[i].gameObject.SetActive(false);
                break;
            }
        }

        switch (employee.employeeValues.employeeType)
        {
            case Employee.EmployeeType.Waitress:
                hiredWaitresses.Remove(employee.GetComponent<Waitress>());
                if (PlayerManager.instance.tavern.takenWaitressIdlePositions.Contains(employee.GetComponent<Waitress>().idlePosition))
                    PlayerManager.instance.tavern.takenWaitressIdlePositions.Remove(employee.GetComponent<Waitress>().idlePosition);
                break;
            case Employee.EmployeeType.Cook:
                hiredCooks.Remove(employee.GetComponent<Cook>());
                employee.GetComponent<Cook>().furnace.taken = false;
                break;
            case Employee.EmployeeType.Bouncer:
                hiredBouncers.Remove(employee.GetComponent<Bouncer>());
                PlayerManager.instance.tavern.bouncersIdlePositions.Add(employee.GetComponent<Bouncer>().idlePosition);
                break;
            case Employee.EmployeeType.Bard:
                hiredBards.Remove(employee.GetComponent<Bard>());
                PlayerManager.instance.tavern.stage.bardPositions.Add(employee.GetComponent<Bard>().idlePosition);
                break;
        }

        employee.npcBubble.activate = false;
        Destroy(employee.gameObject, 0.5f);
    }
    //fonction qui paie le salaire des employés
    public void PayEmployees()
    {
        var totalToPay = 0;
        for (int i = 0; i < hiredWaitresses.Count; i++) totalToPay += hiredWaitresses[i].employeeValues.employeeWage;
        for (int i = 0; i < hiredCooks.Count; i++) totalToPay += hiredCooks[i].employeeValues.employeeWage;
        for (int i = 0; i < hiredBouncers.Count; i++) totalToPay += hiredBouncers[i].employeeValues.employeeWage;
        for (int i = 0; i < hiredBards.Count; i++) totalToPay += hiredBards[i].employeeValues.employeeWage;

        PlayerManager.instance.PlayerMoney -= totalToPay;
        UIManager.instance.blackPanel.recapPanel.salaryValue = -totalToPay;
    }

    //fonction qui génère les bandits du tour
    public void GenerateBandits()
    {
        for (int i = 0; i < bandits.Length; i++)
        {
            bandits[i] = null;
            int chance = Random.Range(0, 100);
            if (chance <= (30 + (10 * PhaseManager.instance.turn))) bandits[i] = ScriptableObject.CreateInstance<Bandit>();
        }
    }
    IEnumerator BanditGenerator(Bandit bandit)
    {
        yield return new WaitForSeconds(bandit.willSpawnAt);
        GenerateClient(bandit);
    }

    //fonction qui permet de sélectionner un nombre au hasard selon plusieurs pourcentages de probabilité
    int Choose (float[] probs)
    {
        float total = 0;

        for (int i =0; i < probs.Length; i++)
        {
            total += probs[i];
        }

        Random.InitState(System.DateTime.Now.Millisecond);
        float randomPoint = Random.value * total;

        for (int i= 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else randomPoint -= probs[i];
        }

        return probs.Length - 1;
    }

    //fonction qui stoppe la génération de clients
    public void StopService()
    {
        mustBeStopping = true;
        StartCoroutine(CheckRemainingClients());
        PhaseManager.instance.serviceEndEvent.Invoke();
        PayEmployees();
    }
    IEnumerator CheckRemainingClients()
    {
        bool clientsStillRemaining = true;
        while (clientsStillRemaining)
        {
            if (clientsInRestaurant.Count == 0 && clientsAtStage.Count == 0) clientsStillRemaining = false;
            else
            {
                foreach (var item in clientsAtStage)
                {
                    item.LeaveStage();
                }
                yield return new WaitForSeconds(3);
            }
        }

        if (hiredBards.Count > 0) BardsExp();
        PhaseManager.instance.StartRecapitulationPhase();
        yield break;
    }

    public void BardsExp()
    {
        var temp = UIManager.instance.blackPanel.recapPanel.clientsTipsValue / hiredBards.Count;
        foreach (Bard bard in hiredBards)
        {
            bard.employeeValues.employeeExperience += temp*bard.employeeData.skillExpValues[0];
            bard.employeeValues.employeeExperience += bardGeneratedPopularity * bard.employeeData.skillExpValues[1];
        }
    }
}


