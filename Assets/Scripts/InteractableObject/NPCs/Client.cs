using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Client : NPC
{
    #region ///  VARIABLES  ///
    public ClientData clientData;

    public enum State {  Moving, Ordering, AwaitingDish, Consuming, Listening};
    [HideInInspector] public State state = State.Moving;

    [HideInInspector] public bool stopTimer = true;
    public float satisfaction = 75;
    public float Satisfaction
    {
        get
        {
            return satisfaction;
        }
        set
        {
            satisfaction = value;
            satisfaction = Mathf.Clamp(satisfaction, 0f, 100f);
        }
    }

    public Transform hand;

    [HideInInspector] public Bandit bandit;

    //Liste d'objets avec lequel le client est actuellement en train d'intéragir
    [HideInInspector] public Order order;
    [HideInInspector] public Chair chair;
    [HideInInspector] public Dish dish;
    [HideInInspector] public Client opponent;

    public Renderer skin;
    private bool watchingBard;
    #endregion

    #region InteractableObject & Unity Methods
    // Use this for initialization
    new void Start()
    {
        base.data = clientData;
        base.Initialization();
        selectable = true;
    }

    private new void Update()
    {
        base.Update();

        if (!stopTimer) satisfaction -= Time.deltaTime;
        if(satisfaction <= 0 && !stopTimer)
        {
            LostPatience();
        }
    }

    //fonction qui détermine si un client peut être interagible
    public override bool CheckIfInteractable()
    {
        //si le client passe commande, on peut le sélectionner
        if (state == State.Ordering) interactable = true;

        //sinon si le client a passé commande mais qu'il peut être servis, on peut le sélectionner
        else if (state == State.AwaitingDish)
        {
            if (SelectionManager.instance.selectedObject != null &&
                SelectionManager.instance.selectedObject.GetComponent<Bouncer>() != null) interactable = true;

            else if (SelectionManager.instance.selectedObject != null &&
                SelectionManager.instance.selectedObject.GetComponent<Dish>() != null &&
                SelectionManager.instance.selectedObject.GetComponent<Dish>().dishData.order == order) interactable = true;

            else
            {
                bool orderReady = false;
                if (SelectionManager.instance.selectedObject != null &&
                    SelectionManager.instance.selectedObject.GetComponent<Waitress>() != null)
                {
                    if (SelectionManager.instance.selectedObject.GetComponent<Waitress>().dish != null &&
                        SelectionManager.instance.selectedObject.GetComponent<Waitress>().dish.dishData.order == order)
                        orderReady = true;
                    else if (SelectionManager.instance.selectedObject.GetComponent<Waitress>().dish == null)
                    {
                        //dans toute les commandes prêtes, si une d'entre elle correspond à la commande de ce client, on passe canBeServed en vrai
                        for (int i = 0; i < NPCManager.instance.tempDish.Count; i++)
                        {
                            if (NPCManager.instance.tempDish[i].GetComponent<Dish>().dishData.order == order) orderReady = true;
                        }
                    }
                }
                if (orderReady) interactable = true;
                else interactable = false;
            }
        }
        else interactable = false;
        return base.CheckIfInteractable();
    }

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Client
    public override void Select()
    {
        base.Select();

        //On indique au panneau de sélection de s'ouvrir avec les valeurs de ce plat
        UIManager.instance.selectionPanel.ClientContent(this);
    }
    #endregion

    //fonction qui fait voir si la taverne a des bardes assez connus
    public void CheckForCelebrities()
    {
        npcBubble.activate = false;
        if (NPCManager.instance.hiredBards.Count > 0 && PlayerManager.instance.tavern.availableStageChairs.Count > 0)
        {
            var clientToStageChance = GameManager.instance.data.straightToStageBaseChance;
            foreach (Bard bard in NPCManager.instance.hiredBards)
            {
                clientToStageChance += bard.employeeValues.employeeSkills[2];
            }

            if (Random.Range(0, 100) < clientToStageChance)
            {
                chair = PlayerManager.instance.tavern.availableStageChairs[Random.Range(0, PlayerManager.instance.tavern.availableStageChairs.Count - 1)];
                PlayerManager.instance.tavern.availableStageChairs.Remove(chair);

                NPCManager.instance.clientsAtStage.Add(this);

                stopTimer = true;

                GoTo(PlayerManager.instance.tavern.tavernEntrance.gameObject, 0.5f);
                nextAction = GetSearched;
                return;
            }
        }

        CheckForChairs();
    }
    //fonction qui indique si oui ou non le client à trouvé une place libre dans la taverne
    public void CheckForChairs()
    {
        //si il y a des chaises libres...
        if (PlayerManager.instance.tavern.availableRestaurantChairs.Count > 0)
        {
            FoundChair(PlayerManager.instance.tavern.availableRestaurantChairs[Random.Range(0, PlayerManager.instance.tavern.availableRestaurantChairs.Count - 1)]);
        }
        //sinon, si je ne suis pas déjà dans la file d'attente
        else if (!NPCManager.instance.clientsInQueue.Contains(this))
        {
            stopTimer = false;
            npcBubble.bubble.icon.sprite = clientData.question;

            PhaseManager.instance.serviceEndEvent.AddListener(Leave);

            NPCManager.instance.clientsInQueue.Add(this);
            GoTo(NPCManager.instance.queuePositions[NPCManager.instance.clientsInQueue.IndexOf(this)].transform.position);
        }
    }
    public void FoundChair(Chair givenChair)
    {
        chair = givenChair;
        PlayerManager.instance.tavern.availableRestaurantChairs.Remove(chair);

        if (NPCManager.instance.clientsInQueue.Contains(this))
        {
            PhaseManager.instance.serviceEndEvent.RemoveListener(Leave);
            NPCManager.instance.clientsInQueue.Remove(this);
            NPCManager.instance.UpdateQueue();
        }

        NPCManager.instance.clientsInRestaurant.Add(this);
        stopTimer = true;

        GoTo(PlayerManager.instance.tavern.tavernEntrance.gameObject, 0.5f);
        nextAction = GetSearched;
    }
    public void GetSearched()
    {
        GoTo(chair.destination.transform.position, navMeshAgent.stoppingDistance);
        nextAction = Sit;
    }
    public void Sit()
    {
        if (NPCManager.instance.clientsInRestaurant.Contains(this)) nextAction = Order;
        else if (NPCManager.instance.clientsAtStage.Contains(this)) nextAction = Watch;

        //on désactive le navmeshagent pour le moment afin que le client puisse s'asseoir hors de la surface navigable
        navMeshAgent.enabled = false;
        gameObject.transform.DOMove(new Vector3(chair.transform.position.x, gameObject.transform.position.y, chair.transform.position.z), 1)
            .OnComplete(() => DelayAction(0f));
        gameObject.transform.LookAt(new Vector3(chair.plate.transform.position.x, gameObject.transform.position.y, chair.plate.transform.position.z));
    }

    //fonction qui gère ce que doit faire le client quand il a perdu patience
    public void LostPatience()
    {
        state = State.Moving;
        stopTimer = true;
        npcBubble.activate = false;
        PlayerManager.instance.PlayerPopularity += -1;

        if (NPCManager.instance.clientsInQueue.Contains(this))
        {
            NPCManager.instance.clientsInQueue.Remove(this);
            NPCManager.instance.UpdateQueue();
            Leave();
        }

        else if (NPCManager.instance.clientsInRestaurant.Contains(this))
        {
            if (chair.table.clientsOrdering.Contains(this))
            {
                chair.table.clientsOrdering.Remove(this);
                UIManager.instance.clientInfos.UpdateOrderInfos(false);
            }

            if (NPCManager.instance.clientsWaitingForDish.Contains(this))
            {
                NPCManager.instance.clientsWaitingForDish.Remove(this);
                UIManager.instance.clientInfos.UpdateDishInfos(order, false);
                NPCManager.instance.cookOrdersList.Remove(order);
            }

            NPCManager.instance.clientsInRestaurant.Remove(this);
            PlayerManager.instance.tavern.NewAvailableChair(chair);

            nextAction = Leave;

            if (opponent) nextAction = AnswerProvocation;
            else if (NPCManager.instance.clientsInRestaurant.Count > 1 &&
                PlayerManager.instance.unhappyClients >= GameManager.instance.data.unhappyClientsBeforeFight &&
                opponent == null)
            {
                Debug.Log("Starting fight...");
                PlayerManager.instance.unhappyClients = 0;

                List<Client> clientsToFight = new List<Client>();
                foreach (Client client in NPCManager.instance.clientsInRestaurant) { if (client.state == State.AwaitingDish || client.state == State.Ordering) clientsToFight.Add(client); }
                if (clientsToFight.Count > 0)
                {
                    opponent = clientsToFight[Random.Range(0, clientsToFight.Count - 1)];

                    opponent.opponent = this;
                    opponent.LostPatience();

                    nextAction = FindOpponent;
                }
                else GetUp(AnimStates.walk);
            }
            else if (opponent == null)PlayerManager.instance.unhappyClients++;

            GetUp(AnimStates.walk);
        }
    }
    public void FindOpponent()
    {
        chair = null;
        nextAction = Fight;
        GoTo(opponent.gameObject);
    }
    public void AnswerProvocation()
    {
        chair = null;
        GoTo(opponent.gameObject);
    }
    public void Fight()
    {
        NPCManager.instance.GenerateFight(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
        PlayerManager.instance.PlayerPopularity -= GameManager.instance.data.removedPopularityPerFighter;
        AlertPanel.instance.GenerateAlert(Alert.AlertType.Brawl, name, opponent.name);

        nextAction = Autodestroy;
        DelayAction(0.5f);
    }

    //fonction qui fait s'asseoir le client et lui fait commander quelque chose
    public void Order ()
    {
        stopTimer = false;
        npcBubble.activate = true;
        state = State.Ordering;
        animator.SetTrigger(AnimStates.sit);
        UIManager.instance.clientInfos.UpdateOrderInfos();

        if(bandit != null)
        {
            NPCManager.instance.banditsSeated.Add(this);
            order = clientData.banditOrders[Random.Range(0, clientData.banditOrders.Length)];
        }
        else
        {
            order = clientData.allOrders[Random.Range(0, clientData.allOrders.Length)];
        }

        chair.table.clientsOrdering.Add(this);
        SelectionManager.instance.ShowKickableClients.AddListener(SelectableVisual);
    }
    //fonction qui fait consommer et payer sa commande au client
    public void Pay ()
    {
        NPCManager.instance.clientsInRestaurant.Remove(this);
        PlayerManager.instance.tavern.NewAvailableChair(chair);

        if (SelectionManager.instance.selectedObject == dish) SelectionManager.instance.UnselectObject();
        Destroy(dish.gameObject);

        npcBubble.activate = false;

        Debug.Log(Satisfaction + "+ qualité du plat:" + (int)dish.quality*5 + "+ fraîcheur du plat:" + (int)dish.freshness*5);
        Satisfaction += ((int)dish.quality * 5) + ((int)dish.freshness * 5);
        Debug.Log(Satisfaction.ToString());

        PlayerManager.instance.tavern.TrashSpawner(1 - (Satisfaction / 100));
        PlayerManager.instance.PlayerPopularity += Mathf.Lerp(-1f, 1f, Satisfaction/100f);

        nextAction = DecideAfterPaying;

        if (bandit == null)
        {
            var temp = PlayerManager.instance.GainMoney(dish);
            UIManager.instance.RessourcePopup(npcBubble.transform.position, temp, UIManager.instance.silverColor);
            UIManager.instance.blackPanel.recapPanel.clientsServedValue += temp;

            if (opponent) nextAction = AnswerProvocation;
            else
            { 
                if (NPCManager.instance.clientsInRestaurant.Count > 1 && Satisfaction <= GameManager.instance.data.unhappyThreshold)
                {
                    if (PlayerManager.instance.unhappyClients >= GameManager.instance.data.unhappyClientsBeforeFight)
                    {
                        Debug.Log("Starting fight...");
                        PlayerManager.instance.unhappyClients = 0;

                        List<Client> clientsToFight = new List<Client>();
                        foreach (Client client in NPCManager.instance.clientsInRestaurant) { if (client.state == State.AwaitingDish || client.state == State.Ordering) clientsToFight.Add(client); }
                        opponent = clientsToFight[Random.Range(0, clientsToFight.Count - 1)];

                        opponent.opponent = this;
                        opponent.LostPatience();

                        nextAction = FindOpponent;
                    }
                    else PlayerManager.instance.unhappyClients++;
                }
                else PlayerManager.instance.unhappyClients = 0;
            }
        }

        GetUp(AnimStates.drunk);
    }

    //fonction qui détermine ce que fait le client une fois qu'il a payé
    public void DecideAfterPaying()
    {
        if (bandit != null)
        {
            bandit.Mission(npcBubble.transform.position);
            BanditAutodestruct();
        }
        else
        {
            if (PhaseManager.instance.timeLeft > 10f &&
            Satisfaction >= 100 - GameManager.instance.data.happyThreshold &&
            NPCManager.instance.hiredBards.Count > 0 &&
             PlayerManager.instance.tavern.availableStageChairs.Count > 0)
            {
                chair = PlayerManager.instance.tavern.availableStageChairs[Random.Range(0, PlayerManager.instance.tavern.availableStageChairs.Count - 1)];
                PlayerManager.instance.tavern.availableStageChairs.Remove(chair);

                NPCManager.instance.clientsAtStage.Add(this);

                stopTimer = true;

                GoTo(chair.destination.transform.position, 0.5f);
                nextAction = Sit;
            }
            else Leave();
        }
    }

    public void GetUp(int state)
    {
        gameObject.transform.DOMove(new Vector3(chair.destination.transform.position.x, gameObject.transform.position.y, chair.destination.transform.position.z), 1)
            .OnComplete(() => animator.SetTrigger(state))
            .OnComplete(() => chair = null)
            .OnComplete(() => DelayAction(0f));
    }

    //fonction qui fait s'asseoir le client en face de la scène
    public void Watch()
    {
        animator.SetTrigger(AnimStates.sit);
        PhaseManager.instance.serviceEndEvent.AddListener(LeaveStage);
        watchingBard = true;
        StartCoroutine(WatchBard());
    }
    IEnumerator WatchBard()
    {
        while (watchingBard)
        {
            yield return new WaitForSeconds(5f);
            int temp = 0;
            if (!Bard.generatePopularity)
            {
                foreach (var item in NPCManager.instance.hiredBards)
                {
                    temp += item.employeeValues.employeeSkills[0];
                }
                temp = Random.Range(1, temp / 2);

                PlayerManager.instance.PlayerMoney += temp;
                UIManager.instance.RessourcePopup(npcBubble.transform.position, temp, UIManager.instance.silverColor);
                UIManager.instance.blackPanel.recapPanel.clientsTipsValue += temp;
            }
            else
            {
                foreach (var item in NPCManager.instance.hiredBards)
                {
                    temp += item.employeeValues.employeeSkills[1];
                }
                temp = Random.Range(1, temp / 5);

                PlayerManager.instance.PlayerPopularity += temp;
                UIManager.instance.RessourcePopup(npcBubble.transform.position, temp, UIManager.instance.popularityColor, NPCManager.instance.hiredBards[0].employeeData.tokens[2]);
                NPCManager.instance.bardGeneratedPopularity += temp;
            }
        }
    }
    public void LeaveStage()
    {
        watchingBard = false;
        NPCManager.instance.clientsAtStage.Remove(this);
        PlayerManager.instance.tavern.availableStageChairs.Add(chair);

        nextAction = Leave;
        GetUp(AnimStates.walk);
    }

    //fonction qui fait partir le client
    public void Leave ()
    {
        GoTo(NPCManager.instance.bifrost);
        nextAction = Autodestroy;
    }
    public void BanditAutodestruct()
    {
        NPCManager.instance.GenerateSmokeBomb(transform.position);
        nextAction = Autodestroy;
        DelayAction(0.5f);
    }
    //fonction qui fait disparaître le client
    public void Autodestroy()
    {
        if (opponent)
        {
            Destroy(opponent.npcBubble);
            Destroy(opponent.gameObject);
        }

        NPCManager.instance.purge.RemoveListener(Autodestroy);
        Destroy(npcBubble, 0.1f);
        Destroy(gameObject, 0.1f);
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //si on a pas déjà selectionné l'objet et qu'on peut le selectionner
            if (SelectionManager.instance.selectedObject != this &&
                CheckIfSelectable())
            {
                Tooltip.instance.overTooltipableObject = true;
                Tooltip.instance.UITooltip(tooltipable, name, tooltipable.leftClick, tooltipable.holdClick, tooltipable.rightClick, tooltipable.rightClickColor);
            }

            //si on a déjà selectionné un objet et que celui-ci peut intéragir avec l'object sélectionné, on affiche le visuel
            else if (SelectionManager.instance.selectedObject != null &&
                CheckIfInteractable())
            {
                Tooltip.instance.overTooltipableObject = true;
                Tooltip.instance.UITooltip(tooltipable, name, tooltipable.leftClick, tooltipable.holdClick, tooltipable.rightClick, tooltipable.rightClickColor);
            }
        }
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        Tooltip.instance.overTooltipableObject = false;
    }

    #region ///  VIEUX CODE  ///
    /*
    Data.ClientState clientState;
    public Data.ClientState ClientState
    {
        get
        {
            return clientState;
        }
        set
        {
            clientState = value;

            switch(clientState)
            {
                case Data.ClientState.Ordering:
                    ServiceManager.instance.ShowOrdersToPick.AddListener(CanBeSelected);
                    ServiceManager.instance.ShowInteractableClients.AddListener(CanBeSelected);
                    break;
                case Data.ClientState.Hungry:
                    ServiceManager.instance.ShowClientsToServe.AddListener(CanBeSelected);
                    ServiceManager.instance.ShowInteractableClients.AddListener(CanBeSelected);
                    break;
                default:
                    ServiceManager.instance.ShowOrdersToPick.RemoveListener(CanBeSelected);
                    ServiceManager.instance.ShowClientsToServe.RemoveListener(CanBeSelected);
                    ServiceManager.instance.ShowInteractableClients.RemoveListener(CanBeSelected);
                    break;
            }

            //if (UIManager.instance.currentNPC == this) UIManager.instance.UpdateNpcPanel(this);
        }
    }

    //Les places attitrées au client
    public Table table;
    public GameObject waitingPlace, dish;
    public Chair chair;
    public Vector3 queuePosition;
    public Order order;
    public ClampBubble clampBubble;
    public float satisfaction, patience = 30;
    bool hasLostPatience = false;
    public bool isBeingTakenCareOf = false;

    new IEnumerator Start()
    {
        base.Start();
        clampBubble.CreateBubble();
        ClientState = Data.ClientState.Walking;
        ServiceManager.instance.HideSelectableElements.AddListener(Unselect);
        satisfaction = patience;
        yield return new WaitForSeconds(1);
    }

    private new void Update()
    {
        base.Update();

        if (clientState == Data.ClientState.Waiting) satisfaction -= Time.deltaTime;

        if ((clientState == Data.ClientState.Ordering || clientState == Data.ClientState.Hungry || clientState == Data.ClientState.Waiting) && !isBeingTakenCareOf && patience > 0)
        {
            satisfaction -= Time.deltaTime;
            clampBubble.bubble.fillAmount = satisfaction / patience;

            if (satisfaction < patience / 4) clampBubble.bubble.color = Color.red;
            else if (satisfaction < patience / 2) clampBubble.bubble.color = Color.yellow;
            else if (satisfaction > patience / 2) clampBubble.bubble.color = Color.green;
        }

        if (satisfaction < 0 && !hasLostPatience && clampBubble.bubble.gameObject) LostPatience();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (clientState == Data.ClientState.Dying && other.gameObject.name == "Bifrost") PhotonNetwork.Destroy(gameObject);
    }

    //fonction qui détermine si un client peut être sélectionné
    public void CanBeSelected ()
    {
        bool canBeServed = false;

        //dans toute les commandes prêtes, si une d'entre elle correspond à la commande de ce client, on passe canBeServed en vrai
        for (int i = 0; i < ServiceManager.instance.tempDish.Count; i++)
        {
            if (ServiceManager.instance.tempDish[i].GetComponent<Dish>().order == order) canBeServed = true; 
        }

        //si le client passe commande, on peut le sélectionner
        if (clientState == Data.ClientState.Free && ClientState == Data.ClientState.Ordering) GetComponent<InteractableObject>().InteractableVisual();

        //sinon si le client a passé commande mais qu'il peut être servis, on peut le sélectionner
        else if (clientState == Data.ClientState.Free && ClientState == Data.ClientState.Hungry && canBeServed) GetComponent<InteractableObject>().InteractableVisual();
    }

    public void FoundSeat()
    {
        ClientState = Data.ClientState.Walking;
        ServiceManager.instance.clientsInQueue.Remove(this);
        satisfaction = patience;
        GoTo(chair.destination);
        ServiceManager.instance.tavern.queue.SwitchQueueSpace(waitingPlace);
        waitingPlace = null;
    }
    public void HaveASeat ()
    {
        navMeshAgent.enabled = false;
        gameObject.transform.DOMove(new Vector3(chair.transform.position.x, gameObject.transform.position.y, chair.transform.position.z), 1);
        gameObject.transform.LookAt(new Vector3(chair.plate.transform.position.x, gameObject.transform.position.y, chair.plate.transform.position.z));
        ServiceManager.instance.clientsWaitingToOrder.Add(this);
        UIManager.instance.UpdateInfoBubble();
        ClientState = Data.ClientState.Ordering;
    }

    public void GenerateClientOrder ()
    {
        ClientState = Data.ClientState.Ordering;

        order = ServiceManager.instance.allPossibleConsumptions
          [Random.Range(0, ServiceManager.instance.allPossibleConsumptions.Length - 1)];

        clampBubble.showBubble = true;

        if (SelectionManager.instance.instruction == Data.Instruction.GivingInstruction) CanBeSelected();
    }

    public void ConsumeOrder (GameObject givenDish)
    {
        StartCoroutine(Consuming(givenDish));
    }

    IEnumerator Consuming (GameObject givenDish)
    {
        yield return new WaitForSeconds(givenDish.GetComponent<Dish>().order.ressourceAmount * 5);
        if (PhotonNetwork.OfflineMode) Destroy(givenDish);
        else PhotonNetwork.Destroy(givenDish);
        ServiceManager.instance.GainMoney(order);
        //if (UIManager.instance.currentNPC == this) UIManager.instance.UpdateClientOnNpcPanel(this);
    }

    public void LostPatience ()
    {
        if (waitingPlace) ServiceManager.instance.tavern.queue.SwitchQueueSpace(waitingPlace);
        hasLostPatience = true;
        ServiceManager.instance.ShowOrdersToPick.RemoveListener(CanBeSelected);
        Unselect();
    }

    public void Leave()
    {
        //gameObject.transform.DOMove(new Vector3(seatDestination.x, gameObject.transform.position.y, seatDestination.z), 1).OnComplete(() => navMeshAgent.enabled = true);
        navMeshAgent.enabled = true;
        clientState = Data.ClientState.Dying;
        Destroy(clampBubble.bubble.gameObject);
        ServiceManager.instance.clientsInQueue.Remove(this);
        ServiceManager.instance.clientsWaitingForDish.Remove(this);
        ServiceManager.instance.clientsWaitingToOrder.Remove(this);
        UIManager.instance.UpdateInfoBubble();
        GoTo(ServiceManager.instance.bifrost);
        if (chair) table.SwitchChair(chair);
    }
    */
    #endregion
}
