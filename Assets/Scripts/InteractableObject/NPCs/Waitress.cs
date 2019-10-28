using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Waitress : Employee
{
    #region ///  VARIABLES  ///
    public enum State { Free, Standby, OrderPickup, DishServing, TrashCleaning };
    [HideInInspector] public State state = State.Free;
    public int orderPickupPriority = 0, dishServingPriority = 0, trashCleaningPriority = 0;

    [SerializeField] private Transform plateTransform;

    //Liste d'objets avec lequel la serveuse est actuellement en train d'intéragir
    [HideInInspector] public List<Client> clients;
    [HideInInspector] public List<Order> orders;
    [HideInInspector] public Table table;
    [HideInInspector] public CounterSpace counterSpace;
    [HideInInspector] public Trash trash;
    [HideInInspector] public Dish dish;
    #endregion

    // Use this for initialization 
    new void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SearchJob();
    }

    new void Update()
    {
        base.Update();
    }

    //Fonction qui passe la serveuse en état d'inactivité
    public void WaitressIdle()
    {
        npcBubble.bubble.filler.color = employeeData.skillsColor[0];
        npcBubble.bubble.icon.sprite = employeeData.skillIcons[0];

        if (idlePosition == null)
        {
            Transform nearestIdlePosition = null;
            float nearestCoordinates = 100;
            for (int i = 0; i < PlayerManager.instance.tavern.freeWaitressIdlePositions.Count; i++)
            {
                if (Vector3.Distance(gameObject.transform.position, PlayerManager.instance.tavern.freeWaitressIdlePositions[i].transform.position) < nearestCoordinates)
                {
                    nearestIdlePosition = PlayerManager.instance.tavern.freeWaitressIdlePositions[i];
                    nearestCoordinates = Vector3.Distance(gameObject.transform.position, nearestIdlePosition.transform.position);
                }
            }
            if (nearestIdlePosition != null)
            {
                idlePosition = nearestIdlePosition;
                PlayerManager.instance.tavern.freeWaitressIdlePositions.Remove(nearestIdlePosition);
                PlayerManager.instance.tavern.takenWaitressIdlePositions.Add(nearestIdlePosition);
                GoTo(idlePosition.transform.position, employeeData.timeBeforeSearching);
                nextAction = SearchJob;
            }
        }
        else
        {
            nextAction = SearchJob;
            DelayAction(employeeData.timeBeforeSearching / (1 + (employeeValues.employeeSkills[1] * 0.2f)));
        }

        state = State.Free;
        clients.Clear();
        orders.Clear();
        table = null;

        if (SelectionManager.instance.selectedObject == this)
        {
            SelectionManager.instance.ShowOrderingTables.Invoke(employeeData.skillsColor[0], true, "Prendre les commandes");
            SelectionManager.instance.ShowServicableClients.Invoke(employeeData.skillsColor[1], true, "Servir un plat");
            SelectionManager.instance.ShowTrash.Invoke(employeeData.skillsColor[2], true, "Nettoyer");
        }
    }

    //Fonction qui permet à la serveuse de chercher une tâche à effectuer en prenant en compte la priorité associée à chaque tâche
    public void SearchJob()
    {
        if (SelectionManager.instance.selectedObject == this)
        {
            WaitressIdle();
            return;
        }

        //on crée une liste avec les différentes valeurs de priorité de chaque actions.
        List<int> actions = new List<int>();
        if (orderPickupPriority != 0) actions.Add(orderPickupPriority);
        if (dishServingPriority != 0) actions.Add(dishServingPriority);
        if (trashCleaningPriority != 0) actions.Add(trashCleaningPriority);

        //on crée des booléens afin de n'effectuer les recherches de tâche qu'une seule fois
        bool hasSearchedForPickJob = false, hasSearchedForServeJob = false, hasSearchedForCleaningJob = false;

        //on prends la priorité la plus élevée
        int highestPriority = Mathf.Max(actions.ToArray());

        //pour chaque actions à effectuer...
        for (int i = 0; i < actions.Count; i++)
        {
            //si la tâche ayant la plus haute priorité est de prendre une commande...(si cette tâche à la même valeur de priorité qu'une autre, elle sera effectuée en première)
            //et que la recherche n'a pas déjà été effectuée
            //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
            if (highestPriority == orderPickupPriority && !hasSearchedForPickJob && orderPickupPriority != 0)
            {
                //si il y a un client voulant commander dans la taverne
                for (int o = 0; o < PlayerManager.instance.tavern.tables.Length; o++)
                {
                    if (!PlayerManager.instance.tavern.tables[o].takenCareOf
                        && PlayerManager.instance.tavern.tables[o].clientsOrdering.Count > 0)
                    {
                        OrderPickup(PlayerManager.instance.tavern.tables[o]);
                        return;
                    }
                }

                //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                hasSearchedForPickJob = true;
            }

            //si la tâche ayant la plus haute priorité est de servir une commande...
            //et que la recherche n'a pas déjà été effectuée
            //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
            else if (highestPriority == dishServingPriority && !hasSearchedForServeJob && dishServingPriority != 0)
            {
                //parmis tout les clients ayant commandés...
                for (int o = 0; o < NPCManager.instance.clientsWaitingForDish.Count; o++)
                {
                    //parmis les commandes prêtes...
                    for (int p = 0; p < NPCManager.instance.tempDish.Count; p++)
                    {
                        //si la commande trouvée correspond à celle du client, on la lui sert
                        if (NPCManager.instance.clientsWaitingForDish[o].order == NPCManager.instance.tempDish[p].GetComponent<Dish>().dishData.order)
                        {
                            DishServing(NPCManager.instance.clientsWaitingForDish[o], NPCManager.instance.tempDish[p].GetComponent<Dish>());
                            return;
                        }
                    }
                }

                //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                hasSearchedForServeJob = true;
            }

            //si la tâche ayant la plus haute priorité est nettoyer une table...(si cette tâche à la même valeur de priorité qu'une autre, elle sera effectuée en première)
            //et que la recherche n'a pas déjà été effectuée
            //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
            if (highestPriority == trashCleaningPriority && !hasSearchedForCleaningJob && trashCleaningPriority != 0)
            {
                //si on a bien des déchêts à nettoyer...
                for (int o = 0; o < PlayerManager.instance.tavern.activeTrash.Count; o++)
                {
                    if (PlayerManager.instance.tavern.activeTrash[o].gameObject.activeSelf && !PlayerManager.instance.tavern.activeTrash[o].takenCareOf)
                    {
                        TrashCleaning(PlayerManager.instance.tavern.activeTrash[o]);
                        return;
                    }
                }

                //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                hasSearchedForCleaningJob = true;
            }
            highestPriority--;
        }
        //si on n'a pas trouvé de travail à effectuer, on relance une recherche après un certain temps
        WaitressIdle();
    }

    //Fonctions gérant la prise de commande
    ////1ère fonction qui envoie la serveuse auprès d'une table et applique un délais avant OrderPickup2
    public void OrderPickup(Table givenTable)
    {
        state = State.Standby;
        PlayerManager.instance.tavern.freeWaitressIdlePositions.Add(idlePosition);
        PlayerManager.instance.tavern.takenWaitressIdlePositions.Remove(idlePosition);
        idlePosition = null;
        table = givenTable;
        table.takenCareOf = true;

        for (int i = 0; i < table.clientsOrdering.Count; i++)
        {
            clients.Add(table.clientsOrdering[i]);
            clients[i].stopTimer = true;
        }

        var temp = table.clientsOrdering.Count;
        for (int i = 0; i < clients.Count; i++)
            table.clientsOrdering.Remove(clients[i]);

        npcBubble.bubble.filler.color = employeeData.skillsColor[0];
        npcBubble.bubble.icon.sprite = employeeData.skillIcons[1];
        GoTo(table.gameObject, (0.5f * temp));
        nextAction = OrderPickup2;
    }
    ////2ème fonction qui enregistre les commandes des clients et envoie la serveuse la rapporter en cuisine
    private void OrderPickup2()
    {
        state = State.OrderPickup;
        SelectionManager.instance.HideSelectableElements.Invoke(data.color, false, string.Empty);

        for (int i = 0; i < clients.Count; i++)
        {
            orders.Add(clients[i].order);
            //on ajoute du temps à la patience du client
            clients[i].Satisfaction += employeeValues.employeeSkills[0] * 5;
            clients[i].stopTimer = false;
            clients[i].state = Client.State.AwaitingDish;
            NPCManager.instance.clientsWaitingForDish.Add(clients[i]);
        }

        GoTo(PlayerManager.instance.tavern.counters[Random.Range(0, PlayerManager.instance.tavern.counters.Length)].gameObject, 0.5f);
        nextAction = OrderPickup3;
    }
    ////3ème fonction qui enregistre la commande du client en cuisine et renvoie la serveuse en inactivité
    private void OrderPickup3()
    {
        for (int i = 0; i < orders.Count; i++)
        {
            NPCManager.instance.cookOrdersList.Add(orders[i]);
            clients[i].npcBubble.bubble.icon.sprite = clients[i].order.ressourceSprite;
            UIManager.instance.clientInfos.UpdateDishInfos(orders[i]);
            UIManager.instance.clientInfos.UpdateOrderInfos(false);
            SelectionManager.instance.ShowServicableClients.AddListener(clients[i].SelectableVisual);
        }

        table.takenCareOf = false;

        GainExperience(employeeData.skillExpValues[0]);
        WaitressIdle();
    }

    //Fonctions gérant la prise de commande
    ////1ère fonction qui envoie la serveuse chercher le plat en cuisine
    public void DishServing(Client givenClient, Dish givenDish)
    {
        state = State.Standby;
        clients.Add(givenClient);
        clients[0].state = Client.State.Consuming;

        PlayerManager.instance.tavern.freeWaitressIdlePositions.Add(idlePosition);
        PlayerManager.instance.tavern.takenWaitressIdlePositions.Remove(idlePosition);
        idlePosition = null;

        clients.Add(givenClient);
        NPCManager.instance.clientsWaitingForDish.Remove(clients[0]);
        SelectionManager.instance.ShowKickableClients.RemoveListener(clients[0].SelectableVisual);
        SelectionManager.instance.ShowServicableClients.RemoveListener(clients[0].SelectableVisual);
        clients[0].stopTimer = true;

        dish = givenDish;
        NPCManager.instance.tempDish.Remove(dish.gameObject);
        dish.ToggleExpiration(false);
        dish.interactable = false;

        npcBubble.bubble.filler.color = employeeData.skillsColor[1];
        npcBubble.bubble.icon.sprite = employeeData.skillIcons[2];

        GoTo(dish.counter.gameObject, 0.5f);
        nextAction = DishServing2;
    }
    ////2ème fonction qui fait prendre le plat à la serveuse
    private void DishServing2()
    {
        dish.transform.position = plateTransform.position;
        dish.transform.parent = plateTransform;
        dish.counterSpace.counter.SwitchServingPlace(dish.counterSpace);

        state = State.DishServing;
        SelectionManager.instance.HideSelectableElements.Invoke(data.color, false, string.Empty);

        GoTo(clients[0].gameObject, 0.5f);
        nextAction = DishServing3;
    }
    ////3ème fonction qui donne le plat au client et fait retourner la serveuse en état d'inactivité
    private void DishServing3()
    {
        clients[0].dish = dish;
        
        if (dish.dishData.order.ressourceType == Data.RessourceType.Drink)
        {
            clients[0].animator.SetTrigger(AnimStates.drink);
            dish.transform.SetParent(clients[0].hand);
            dish.transform.localPosition = Vector3.zero;
            dish.transform.localEulerAngles = Vector3.zero;
            dish.handle.localPosition = Vector3.zero;
            dish.handle.localEulerAngles = Vector3.zero;
        }
        else
        {
            clients[0].animator.SetTrigger(AnimStates.eat);
            dish.transform.parent = clients[0].chair.plate.transform;
            dish.transform.localPosition = Vector3.zero;
        }

        clients[0].nextAction = clients[0].Pay;
        clients[0].npcBubble.bubble.filler.color = dish.dishData.order.ressourceColor;
        clients[0].DelayAction(dish.dishData.order.ressourceAmount * 5);

        GainExperience(employeeData.skillExpValues[1]);
        dish = null;
        WaitressIdle();
    }

    //Fonctions gérant le nettoyage des déchêts
    ////1ère fonction qui envoie la serveuse vers un déchêt
    public void TrashCleaning(Trash givenTrash)
    {
        trash = givenTrash;
        trash.takenCareOf = true;

        state = State.Standby;
        PlayerManager.instance.tavern.freeWaitressIdlePositions.Add(idlePosition);
        PlayerManager.instance.tavern.takenWaitressIdlePositions.Remove(idlePosition);
        idlePosition = null;

        npcBubble.bubble.filler.color = employeeData.skillsColor[2];
        npcBubble.bubble.icon.sprite = employeeData.skillIcons[3];
        GoTo(trash.gameObject);
        nextAction = TrashCleaning2;
    }
    ////2ème fonction anime la serveuse
    public void TrashCleaning2()
    {
        state = State.TrashCleaning;
        SelectionManager.instance.HideSelectableElements.Invoke(data.color, false, string.Empty);

        animator.SetTrigger(AnimStates.clean);

        nextAction = TrashCleaning3;
        DelayAction(Data.cleanTime);
    }
    ////3ème fonction qui rend enlève le déchêt et fait retourner la serveuse en état d'inactivité
    private void TrashCleaning3()
    {
        trash.gameObject.SetActive(false);
        PlayerManager.instance.tavern.unactiveTrash.Add(trash);
        PlayerManager.instance.tavern.activeTrash.Remove(trash);

        PlayerManager.instance.PlayerCleanliness += (trash.trashData.dirtyValue * (int)(employeeValues.employeeSkills[2] * 0.2f));

        GainExperience(employeeData.skillExpValues[2]);
        WaitressIdle();
    }

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Waitress
    public override void Select()
    {
        base.Select();
        //On indique au panneau de sélection de s'ouvrir avec les valeurs de ce plat
        UIManager.instance.selectionPanel.WaitressContent(this);

        UpdateSelection();
    }
    //Fonction qui override la fonction UpdateSelection de base en y ajoutant des actions spécifiques à Waitress
    public override void UpdateSelection()
    {
        DrawPath();

        //on affiche tout les objets avec lesquels Waitress peut intéragir
        if (state == State.Free || state == State.Standby)
        {
            SelectionManager.instance.ShowOrderingTables.Invoke(employeeData.skillsColor[0], true, "Prendre les commandes");
            SelectionManager.instance.ShowServicableClients.Invoke(employeeData.skillsColor[1], true, "Servir un plat");
            SelectionManager.instance.ShowTrash.Invoke(employeeData.skillsColor[2], true, "Nettoyer");
        }
    }

    //Override InteractWith avec des paramètres propres à Waitress
    public override void InteractWith(InteractableObject interactableObject)
    {
        if (state == State.Free || state == State.Standby)
        {
            if (interactableObject.GetComponent<Client>() &&
            interactableObject.GetComponent<Client>().CheckIfInteractable()
            && interactableObject.GetComponent<Client>().state == Client.State.AwaitingDish)
            {
                for (int i = 0; i < NPCManager.instance.tempDish.Count; i++)
                {
                    if (NPCManager.instance.tempDish[i].GetComponent<Dish>().dishData.order == interactableObject.GetComponent<Client>().order)
                    {
                        if (state == State.Standby && clients[0] != null ||
                        state == State.Standby && table != null)
                        {
                            if (state == State.Standby) CancelAction();
                            DishServing(interactableObject.GetComponent<Client>(), NPCManager.instance.tempDish[i].GetComponent<Dish>());
                            audioSource.clip = employeeData.interactionSounds[Random.Range(0, employeeData.interactionSounds.Length)];
                            audioSource.Play();
                        }
                        else
                        {
                            DishServing(interactableObject.GetComponent<Client>(), NPCManager.instance.tempDish[i].GetComponent<Dish>());
                            audioSource.clip = employeeData.interactionSounds[Random.Range(0, employeeData.interactionSounds.Length)];
                            audioSource.Play();
                        }
                        return;
                    }
                }
            }
            else if (interactableObject.GetComponent<Table>() &&
                interactableObject.GetComponent<Table>().CheckIfInteractable())
            {
                if (interactableObject.GetComponent<Table>().clientsOrdering.Count > 0 &&
                    !interactableObject.GetComponent<Table>().takenCareOf)
                {
                    if (state == State.Standby) CancelAction();
                    OrderPickup(interactableObject.GetComponent<Table>());
                    audioSource.clip = employeeData.interactionSounds[Random.Range(0, employeeData.interactionSounds.Length)];
                    audioSource.Play();
                }     
            }
            else if (interactableObject.GetComponent<Trash>() &&
                interactableObject.GetComponent<Trash>().CheckIfInteractable())
            {
                if (interactableObject.GetComponent<Trash>().gameObject.activeSelf &&
                    !interactableObject.GetComponent<Trash>().takenCareOf)
                {
                    if (state == State.Standby) CancelAction();
                    TrashCleaning(interactableObject.GetComponent<Trash>());
                    audioSource.clip = employeeData.interactionSounds[Random.Range(0, employeeData.interactionSounds.Length)];
                    audioSource.Play();
                }
            }
        }
    }

    //fonction qui annule l'action de la serveuse
    public void CancelAction()
    {
        //si on s'occupe déjà d'un client...
        if (clients.Count > 0 && clients[0].state == Client.State.Consuming)
        {
            clients[0].stopTimer = false;

            clients[0].state = Client.State.AwaitingDish;

            NPCManager.instance.clientsWaitingForDish.Add(clients[0]);
            SelectionManager.instance.ShowServicableClients.AddListener(clients[0].SelectableVisual);
            clients.Clear();
        }

        //si on s'occupe déjà d'une table...
        if (table != null)
        {
            for (int i = 0; i < table.clientsOrdering.Count; i++)
            {
                table.clientsOrdering[i].stopTimer = false;
                table.takenCareOf = false;
            }
            table = null;
            clients.Clear();
        }

        //si on s'occupe déjà d'un déchêt...
        if (trash != null)
        {
            trash.takenCareOf = false;
            trash = null;
        }

        //si on s'occupe déjà d'un espace de comptoir..
        if (counterSpace != null)
        {
            counterSpace.counter.SwitchServingPlace(counterSpace);
            counterSpace = null;
        }
    }

    #region ///  VIEUX CODE  ///
    /*
    public class Waitress : NPC {

    Data.state state;
    public Data.state state
    { 
        get
        {
            return state;
        }

        set
        {
            state = value;

            switch (state)
            {
                case Data.state.Free:
                    npcState = Data.NPCState.Idle;
                    if (UIManager.instance.currentNPC == this)
                    {
                        //ServiceManager.instance.ShowInteractableClients.Invoke();
                        SelectionManager.instance.instruction = Data.Instruction.GivingInstruction;
                    }
                    break;
                default:
                    npcState = Data.NPCState.Busy;
                    break;
            }

            if (UIManager.instance.currentNPC == this) UIManager.instance.UpdateNpcPanel(this);
        }
    }

    public GameObject plate, dish;
    public Client client;
    public Vector3 idlePosition;
    public Transform kitchen;
    public GameObject[] servingTables;
    public int pickPriority, servePriority;

    public float pickExperience = 1, serveExperience = 2;
    public int charisma = 1, speed = 1, cleaning = 1, waitressemployeeWage = 5, waitressemployeeRecruitmentFee = 20;
    public int Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
            navMeshAgent.speed = speed;
        }
    }

    public Order currentOrder;

    private void Awake()
    {
        employeeWage = waitressemployeeWage;
        employeeRecruitmentFee = waitressemployeeRecruitmentFee;
    }

    // Use this for initialization
    new void Start () {
        base.OnEnable();
        pickPriority = 0;
        servePriority = 0;
        npcType = Data.NPCType.Waitress;
        //PhaseManager.instance.roundEnd.AddListener(RoundEnd);
        servingTables = GameObject.FindGameObjectsWithTag("ServingTable");
        state = Data.state.Free;
        plate = transform.GetChild(0).gameObject;
        idlePosition = transform.position;
    }

    new void Update ()
    {
        base.Update();
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentOrder == null &&
            other.gameObject.GetComponent<Client>() == client &&
            state == Data.state.GettingOrder)
        {
            animator.SetBool(AnimStates.reachedDestination, true);
            destinationReached = true;
            navMeshAgent.isStopped = true;
        }

        if (other.gameObject.tag == "ServingTable" &&
            state == Data.state.BringingOrderToKitchen)
        {
            animator.SetBool(AnimStates.reachedDestination, true);
            destinationReached = true;
            navMeshAgent.isStopped = true;
        }

        if (other.gameObject.tag == "ServingTable" &&
            state == Data.state.ServingOrder)
        {
            animator.SetBool(AnimStates.reachedDestination, true);
            destinationReached = true;
            navMeshAgent.isStopped = true;
        }

        if (currentOrder != null &&
            other.gameObject.GetComponent<Client>() == client &&
            state == Data.state.ServingOrder)
        {
            animator.SetBool(AnimStates.reachedDestination, true);
            destinationReached = true;
            navMeshAgent.isStopped = true;
        }
    }

    //fonction qui permet à la serveuse de chercher une tâche à effectuer en prenant en compte la priorité associée à chaque tâche
    public void SearchJob()
    {
        //on crée une liste avec les différentes valeurs de priorité de chaque actions.
        List<int> actions = new List<int>();
        if (pickPriority != 0) actions.Add(pickPriority);
        if (servePriority != 0) actions.Add(servePriority);

        //on crée des booléens afin de n'effectuer les recherches de tâche qu'une seule fois
        bool hasSearchedForPickJob = false, hasSearchedForServeJob = false;

        //on prends la priorité la plus élevée
        int highestPriority = Mathf.Max(actions.ToArray());

        //pour chaque actions à effectuer...
        for (int i = 0; i < actions.Count; i++)
        {
            //si la tâche ayant la plus haute priorité est de prendre une commande...(si cette tâche à la même valeur de priorité qu'une autre, elle sera effectuée en première)
            //et que la recherche n'a pas déjà été effectuée
            //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
            if (highestPriority == pickPriority && !hasSearchedForPickJob && pickPriority != 0)
            {
                //si on a bien des commandes à récupérer...
                if (ServiceManager.instance.clientsWaitingToOrder.Count != 0)
                {
                    GetOrder(ServiceManager.instance.clientsWaitingToOrder[0]);
                    return;
                }

                //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                hasSearchedForPickJob = true;
            }

            //si la tâche ayant la plus haute priorité est de servir une commande...
            //et que la recherche n'a pas déjà été effectuée
            //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
            if (highestPriority == servePriority && !hasSearchedForServeJob && servePriority != 0)
            {
                //si on a bien des commandes à récupérer...
                if (ServiceManager.instance.clientsWaitingForDish.Count != 0)
                {
                    ServeOrder(ServiceManager.instance.clientsWaitingToOrder[0]);
                    return;
                }

                //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                hasSearchedForServeJob = true;
            }

            highestPriority--;
        }
    }

    public void SelectOrderToPick()
    {
        SelectionManager.instance.instruction = Data.Instruction.GivingInstruction;
        ServiceManager.instance.HideSelectableElements.Invoke();
        ServiceManager.instance.ShowOrdersToPick.Invoke();
    }

    public void SelectOrderToServe()
    {
        SelectionManager.instance.instruction = Data.Instruction.GivingInstruction;
        ServiceManager.instance.HideSelectableElements.Invoke();
        ServiceManager.instance.ShowClientsToServe.Invoke();
    }

    public void GetOrder(Client givenClient)
    {
        ServiceManager.instance.clientsWaitingToOrder.Remove(givenClient);
        animator.SetBool(AnimStates.orderToPick, true);
        client = givenClient;
        client.isBeingTakenCareOf = true;
        UIManager.instance.UpdateNpcPanel(client);
        ServiceManager.instance.ShowOrdersToPick.RemoveListener(client.CanBeSelected);
        client.Unselect();
        state = Data.state.GettingOrder;
        destination = givenClient.chair.destination.transform.position;
    }


    public void TriggerOrderPickup ()
    {
        StartCoroutine(PickingOrder());
    }

    IEnumerator PickingOrder()
    {
        yield return new WaitForSeconds(3);
        currentOrder = client.order;
        client.satisfaction += 30 * ( 0.8f + charisma / 5);
        if (client.satisfaction > client.patience) client.patience += client.satisfaction - client.patience;
        client.ClientState = Data.ClientState.Hungry;
        ServiceManager.instance.clientsWaitingForDish.Add(client);
        UIManager.instance.UpdateInfoBubble();
        BringOrderToKitchen();
        yield break;
    }

    public void BringOrderToKitchen()
    {
        state = Data.state.BringingOrderToKitchen;
        destination = servingTables[Random.Range(0, servingTables.Length)].transform.position;
        animator.SetTrigger(AnimStates.orderPicked);
    }

    public void ServeOrder (Client givenClient,  bool playerAsked = false, GameObject dishToServe = null)
    {
        if (dishToServe != null)
        {
            ServiceManager.instance.clientsWaitingForDish.Remove(givenClient);
            UIManager.instance.UpdateInfoBubble();
            animator.SetBool(AnimStates.orderToServe, true);
            client = givenClient;
            UIManager.instance.UpdateNpcPanel(client);
            client.animator.SetBool(AnimStates.waitressEnRoute, true);
            client.isBeingTakenCareOf = true;
            ServiceManager.instance.ShowClientsToServe.RemoveListener(client.CanBeSelected);
            if (playerAsked) client.Unselect();
            else ServiceManager.instance.ShowInteractableClients.Invoke();
            state = Data.state.ServingOrder;
            dish = dishToServe;
            ServiceManager.instance.tempDish.Remove(dish);
            destination = dish.transform.position;
        }

        else
        {
            for (int i = 0; i < ServiceManager.instance.tempDish.Count; i++)
            {
                //dans toute les commandes prêtes, si une d'entre elle correspond à la commande du client, je vais la chercher.
                if (ServiceManager.instance.tempDish[i].GetComponent<Dish>().order == givenClient.order)
                {
                    ServiceManager.instance.clientsWaitingForDish.Remove(givenClient);
                    UIManager.instance.UpdateInfoBubble();
                    animator.SetBool(AnimStates.orderToServe, true);
                    client = givenClient;
                    UIManager.instance.UpdateNpcPanel(client);
                    client.animator.SetBool(AnimStates.waitressEnRoute, true);
                    client.isBeingTakenCareOf = true;
                    ServiceManager.instance.ShowClientsToServe.RemoveListener(client.CanBeSelected);
                    if (playerAsked) client.Unselect();
                    else ServiceManager.instance.ShowInteractableClients.Invoke();
                    state = Data.state.ServingOrder;
                    dish = ServiceManager.instance.tempDish[i];
                    ServiceManager.instance.tempDish.Remove(dish);
                    destination = dish.transform.position;
                    break;
                }
            }
        }
    }

    public void RoundEnd()
    {
        StartCoroutine(EndRound());
    }

    IEnumerator EndRound()
    {
        animator.SetBool(AnimStates.roundEnd, true);
        yield return new WaitForSeconds(15);
        animator.SetBool(AnimStates.roundEnd, false);
    }
    }
    */
    #endregion
}


