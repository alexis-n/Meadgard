using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC : InteractableObject {

    #region ///  VARIABLES  ///
    private new Collider collider;

    private LineRenderer lineRenderer;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public GameObject target;
    private bool destinationReached = false;

    [HideInInspector] public UnityAction nextAction;
    private float delay;
    private IEnumerator coroutine;

    public NPCBubble npcBubble;
    #endregion

    public void Awake()
    {
        collider = GetComponent<Collider>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnDisable()
    {
        if (npcBubble.bubble) npcBubble.bubble.gameObject.SetActive(false);
    }

    protected void Update()
    {
        if (SelectionManager.instance.selectedObject == this &&
            lineRenderer != null) DrawPath();
    }

    //Fonction qui envoir le NPC à une destination donnée
    public void GoTo(Vector3 coordinates, float givenDelay = 0, bool overrideIdle = false)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        npcBubble.bubble.gameObject.SetActive(false);
        delay = givenDelay;

        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;

        animator.ResetTrigger(AnimStates.idle);
        animator.SetTrigger(AnimStates.walk);

        navMeshAgent.destination = coordinates;

        if (delay >= 0) StartCoroutine(CheckIfReachedDestination());
        else Debug.LogError("Given delay is negative: " + delay);
    }
    //Fonction qui envoir le NPC à une cible donnée
    public void GoTo(GameObject givenTarget, float givenDelay = 0)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        delay = givenDelay;
        target = givenTarget;

        animator.ResetTrigger(AnimStates.idle);
        animator.SetTrigger(AnimStates.walk);

        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        coroutine = GoToUpdate();
        StartCoroutine(coroutine);
    }
    IEnumerator GoToUpdate ()
    {
        while (true)
        {
            navMeshAgent.destination = target.transform.position;
            yield return new WaitForSeconds(1);
        }
    }

    //A chaque fois que l'on rencontre un autre collider lors d'un mouvement, on vérifie si c'est notre cible
    void OnTriggerEnter(Collider other)
    {
        if(target != null) CheckIfReachedTarget(other.gameObject);
    }

    //Coroutine qui vérifie chaque seconde si le NPC a atteint sa destination (coordonnées)
    IEnumerator CheckIfReachedDestination()
    {
        destinationReached = false;
        while (!destinationReached)
        {
            yield return new WaitForSeconds(1);
            //on vérifie que le NPC est arrivé à destination et on lui assigne un état en fonction
            if (!navMeshAgent.pathPending && !destinationReached)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (!navMeshAgent.hasPath || Mathf.Abs(navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
                    {
                        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                        {
                            destinationReached = true;
                            navMeshAgent.isStopped = true;

                            animator.ResetTrigger(AnimStates.walk);
                            animator.SetTrigger(AnimStates.idle);

                            coroutine = NextAction(delay);
                            StartCoroutine(coroutine);
                        }
                    }
                }
            }
        }

        yield return null;
    }
    //Fonction qui vérifie si le NPC a rencontré sa cible (objet)
    public void CheckIfReachedTarget(GameObject objectEncountered)
    {
        if (objectEncountered == target)
        {
            destinationReached = true;
            navMeshAgent.isStopped = true;

            animator.ResetTrigger(AnimStates.walk);
            animator.SetTrigger(AnimStates.idle);

            target = null;
            StopCoroutine(coroutine);
            coroutine = NextAction(delay);
            StartCoroutine(coroutine);
        }
    }

    //Fonction qui lance une action après un certain delais
    public void DelayAction(float givenDelay)  
    {
        if (coroutine != null) StopCoroutine(coroutine);
        if (npcBubble.bubble != null) npcBubble.ActivateBubble(givenDelay);
        delay = givenDelay;
        coroutine = NextAction(delay);
        StartCoroutine(coroutine);
    }
    //Coroutine qui lance des méthodes après un certain temps
    IEnumerator NextAction(float givenDelay)
    {
        if (delay > 0) npcBubble.ActivateBubble(delay);
        yield return new WaitForSeconds(givenDelay);
        if (nextAction != null) nextAction();
    }

    //Fonction qui dessine l'itinéraire du NPC
    public void DrawPath()
    { 
        var path = navMeshAgent.path;
        lineRenderer.positionCount = path.corners.Length;
        for (int i = 0; i < path.corners.Length; i++)
        {
            lineRenderer.SetPosition(i, path.corners[i]);
        }
    }

    //override Unselect pour effacer le linerenderer
    public override void Unselect()
    {
        base.Unselect();
        if ( lineRenderer != null) lineRenderer.positionCount = 0;
    }

    #region ///  VIEUX CODE  ///
    /*
    //NavMeshAgent du client
    public string firstName, lastName;
    public Data.NPCType npcType;

    public NavMeshAgent navMeshAgent;
    protected bool destinationReached;

    public LineRenderer line;

    public new Rigidbody rigidbody;
    public Vector3 destination;
    public int owner;
    public Animator animator;
    public Data.NPCState npcState = Data.NPCState.Idle;
    public bool isSelected = false;

    public Data.ClientRace race;

    public float level = 1, level2Threshold = 10, level3Threshold = 20;
    public int skillPoints = 0, skillPointsPerLevel = 2, employeeWage, employeeRecruitmentFee;
    public bool waitingToLevelUp = false;
    float experience = 0;
    public float Experience
    {
        get
        {
            return experience;
        }
        set
        {
            if (!waitingToLevelUp)
            {
                if (level != 3) experience = value;
                else experience = level3Threshold;

                if (level == 1 && experience >= level2Threshold)
                {
                    experience = level2Threshold;
                    waitingToLevelUp = true;
                    skillPoints += skillPointsPerLevel;
                    UIManager.instance.GenerateAlert(Data.AlertContent.EmployeeLevelUP, this);
                }
                else if (level == 2 && experience >= level3Threshold)
                {
                    experience = level3Threshold;
                    waitingToLevelUp = true;
                    skillPoints += skillPointsPerLevel;
                    UIManager.instance.GenerateAlert(Data.AlertContent.EmployeeLevelUP, this);
                }
            }
        }
    }

    protected void OnEnable()
    { 
        rigidbody = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
        owner = ServiceManager.instance.tavern.owner;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        line = gameObject.GetComponent<LineRenderer>();
    }

    public bool VerifyOwnership()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == owner) return true;
        else return false;
    }

    protected void Update()
    {
        if (isSelected) DrawPath();
    }

    public void DrawPath()
    {
        var path = navMeshAgent.path;
        line.positionCount = path.corners.Length;
        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }
    }

    public void NameGenerator()
    {
        string[] lines;

        switch (race)
        {
            case Data.ClientRace.Human:
                if (Random.value < 0.5)
                {
                    lines = File.ReadAllLines(Data.namesFolder + "HumanMaleNames");
                    firstName = lines[Random.Range(0, lines.Length)];
                    lastName = (lines[Random.Range(0, lines.Length)] + "sson");
                }
                else
                {
                   lines = File.ReadAllLines(Data.namesFolder + "HumanFemaleNames");
                    firstName = lines[Random.Range(0, lines.Length)];
                    lastName = (lines[Random.Range(0, lines.Length)] + "dottir");
                }
                break;
            case Data.ClientRace.Dwarf:
                lines = File.ReadAllLines(Data.namesFolder + "DwarfFirstNames");
                firstName = lines[Random.Range(0, lines.Length)];
                string[] prefixesLines = File.ReadAllLines(Data.namesFolder + "DwarfPrefixes");
                string[] suffixesLines = File.ReadAllLines(Data.namesFolder + "DwarfSuffixes");
                lastName = (suffixesLines[Random.Range(0, suffixesLines.Length)] + prefixesLines[Random.Range(0, prefixesLines.Length)]);
                break;
            case Data.ClientRace.Elf:
                lines = File.ReadAllLines(Data.namesFolder + "ElfTitles");
                firstName = lines[Random.Range(0, lines.Length)];
                lines = File.ReadAllLines(Data.namesFolder + "ElfNames");
                lastName = lines[Random.Range(0, lines.Length)];
                break;
            case Data.ClientRace.Jottun:
                lines = File.ReadAllLines(Data.namesFolder + "JottunNames");
                firstName = lines[Random.Range(0, lines.Length)];
                lines = File.ReadAllLines(Data.namesFolder + "JottunTitles");
                lastName = lines[Random.Range(0, lines.Length)];
                break;

            default:
                return;
        }
    }

    public void GoToDestination()
    {
        navMeshAgent.isStopped = false;
        animator.SetBool(AnimStates.reachedDestination, false);
        navMeshAgent.destination = destination;
        StartCoroutine(CheckIfReachedDestination());
    }

    IEnumerator CheckIfReachedDestination()
    {
        destinationReached = false;
        while (!destinationReached)
        {
            yield return new WaitForSeconds(1);
            //on vérifie que le client est arrivé à destination et on lui assigne un état en fonction
            if (!navMeshAgent.pathPending && !destinationReached)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        animator.SetBool(AnimStates.reachedDestination, true);
                        destinationReached = true;
                        navMeshAgent.isStopped = true;
                    }
                }
            }
        }
        yield return null;
    }

    public string StateForUI(UIBank bank)
    {
        switch (npcType)
        {
            case Data.NPCType.Client:
                switch (GetComponent<Client>().ClientState)
                {
                    case Data.ClientState.Consuming:
                        return bank.clientConsuming;
                    case Data.ClientState.Dying:
                        return bank.clientLeaving;
                    case Data.ClientState.Ordering:
                        return bank.clientOrdering;
                    case Data.ClientState.Hungry:
                        return bank.clientHungry;
                    case Data.ClientState.Walking:
                        return bank.clientWalking;
                }
                break;

            case Data.NPCType.Cook:
                switch (GetComponent<Cook>().CookState)
                {
                    case Data.CookState.Cooking:
                        return bank.cookCooking;
                    case Data.CookState.Free:
                        return bank.employeeFree;
                    case Data.CookState.GettingResource:
                        return bank.cookGettingRessources;
                    case Data.CookState.PuttingOrderOnTable:
                        return bank.cookPuttingOrderOnTable;
                }
                break;

            case Data.NPCType.Waitress:
                switch (GetComponent<Waitress>().WaitressState)
                {
                    case Data.WaitressState.BringingOrderToKitchen:
                        return bank.waitressBringingOrderToKitchen;
                    case Data.WaitressState.Free:
                        return bank.employeeFree;
                    case Data.WaitressState.GettingOrder:
                        return bank.waitressGettingOrder;
                    case Data.WaitressState.ServingOrder:
                        return bank.waitressServingOrder;
                }
                break;
        }
        return null;
    }
    */
    #endregion
}
