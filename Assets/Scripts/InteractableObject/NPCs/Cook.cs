using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Cook : Employee {

    #region ///  VARIABLES  ///
    public enum State { Free, Standby, Cooking, Mixing };
    [HideInInspector] public State state = State.Free;
    public int cookingPriority = 0, mixingPriority = 0;

    //Liste d'objets avec lequel le cuisinier est actuellement en train d'intéragir
    [HideInInspector] public Order order;
    [HideInInspector] public Dish dish;
    [HideInInspector] public Counter counter;
    [HideInInspector] public CounterSpace counterSpace;
    [HideInInspector] public Furnace furnace;
    #endregion

    // Use this for initialization
    public new void Awake()
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

    //Fonction qui passe le cuisiner en état d'inactivité
    public void CookIdle()
    {
        npcBubble.bubble.icon.sprite = employeeData.skillIcons[0];

        state = State.Free;
        order = null;
        dish = null;
        counter = null;
        counterSpace = null;

        nextAction = SearchJob;
        DelayAction(employeeData.timeBeforeSearching);
    }

    //fonction qui permet au cuisinier de chercher une tâche à effectuer en prenant en compte la priorité associée à chaque tâche
    public void SearchJob()
    {
        if (SelectionManager.instance.selectedObject == this)
        {
            CookIdle();
            return;
        }

        //si des commandes sont disponibles en cuisine...
        if (NPCManager.instance.cookOrdersList.Count != 0)
        {
            //on crée une liste avec les différentes valeurs de priorité de chaque actions.
            List<int> actions = new List<int>();
            if (cookingPriority != 0) actions.Add(cookingPriority);
            if (mixingPriority != 0) actions.Add(mixingPriority);

            //on crée des booléens afin de n'effectuer les recherches de tâche qu'une seule fois
            bool hasSearchedForFoodJob = false, hasSearchedForDrinkJob = false;

            //on prends la priorité la plus élevée
            int highestPriority = Mathf.Max(actions.ToArray());

            //on vérifie d'abord si on a la place pour poser le plat une fois préparée
            for (int o = 0; o < PlayerManager.instance.tavern.counters.Length; o++)
            {
                if (PlayerManager.instance.tavern.counters[o].freeSpaces.Count > 0)
                {
                    //pour chaque actions à effectuer...
                    for (int i = 0; i < actions.Count; i++)
                    {
                        //si la tâche ayant la plus haute priorité est de cuisiner de la nourriture...(si cette tâche à la même valeur de priorité qu'une autre, elle sera effectuée en première)
                        //et que la recherche n'a pas déjà été effectuée
                        //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
                        if (highestPriority == cookingPriority && !hasSearchedForFoodJob && cookingPriority != 0)
                        {

                            //on crée une liste de commandes de nourriture et on la remplie
                            List<Order> foodOrders = new List<Order>();
                            foreach (Order order in NPCManager.instance.cookOrdersList)
                            {
                                if (order.ressourceType == Data.RessourceType.Food) foodOrders.Add(order);
                            }

                            //si on a bien des commandes de nourriture...
                            if (foodOrders.Count > 0)
                            {
                                //parmis les commandes de nourriture...
                                for (int p = 0; p < foodOrders.Count; p++)
                                {
                                    //si la commande récupérée peut être servie
                                    if (foodOrders[p].ressourceAmount <= PlayerManager.instance.PlayerFood)
                                    {
                                        CookingAndMixing1(foodOrders[p], PlayerManager.instance.tavern.counters[o]);
                                        return;
                                    }
                                }
                                UIManager.instance.playerFood.StatFlickerRed();
                            }
                            //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                            hasSearchedForFoodJob = true;
                        }

                        //si la tâche ayant la plus haute priorité est de cuisiner de la nourriture...(si cette tâche à la même valeur de priorité qu'une autre, elle sera effectuée en première)
                        //et que la recherche n'a pas déjà été effectuée
                        //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
                        if (highestPriority == mixingPriority && !hasSearchedForDrinkJob && mixingPriority != 0)
                        {

                            //on crée une liste de commandes de boissons et on la remplie
                            List<Order> drinkOrders = new List<Order>();
                            foreach (Order order in NPCManager.instance.cookOrdersList)
                            {
                                if (order.ressourceType == Data.RessourceType.Drink) drinkOrders.Add(order);
                            }

                            //si on a bien des commandes de boissons...
                            if (drinkOrders.Count > 0)
                            {
                                //parmis les commandes de boissons...
                                for (int p = 0; p < drinkOrders.Count; p++)
                                {
                                    //si la commande récupérée peut être servie
                                    if (drinkOrders[p].ressourceAmount <= PlayerManager.instance.PlayerDrinks)
                                    {
                                        CookingAndMixing1(drinkOrders[p], PlayerManager.instance.tavern.counters[o]);
                                        return;
                                    }
                                }
                                UIManager.instance.playerDrinks.StatFlickerRed();
                            }
                            //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                            hasSearchedForDrinkJob = true;
                        }
                        highestPriority--;
                    }
                }
            }
        }
        //si on n'a pas trouvé de travail à effectuer, on relance une recherche après un certain temps
        CookIdle();
    }

    //Fonctions gérant la préparation de commande
    ////1ère fonction qui vérifie si le joueur a assez de ressources pour préparer la commande
    public void CookingAndMixing1(Order requestedOrder, Counter givenCounter)
    {
        state = State.Standby;

        if (counter == null) counter = givenCounter;
        if (counterSpace == null) counterSpace = counter.GiveSpace();

        order = requestedOrder;

        switch (order.ressourceType)
        {
            case Data.RessourceType.Drink:
                PlayerManager.instance.PlayerDrinks -= order.ressourceAmount;
                break;
            case Data.RessourceType.Food:
                PlayerManager.instance.PlayerFood -= order.ressourceAmount;
                break;
        }
        NPCManager.instance.cookOrdersList.Remove(order);

        npcBubble.bubble.filler.color = order.ressourceColor;
        npcBubble.bubble.icon.sprite = order.ressourceSprite;
        GoTo(PlayerManager.instance.tavern.reserve, 1.5f);
        nextAction = CookingAndMixing2;
    }
    ////2ème fonction qui instancie le prefab de la commande et dirige le cuisinier vers son fourneau
    public void CookingAndMixing2()
    {
        state = State.Cooking;

        switch (order.ressourceType)
        {
            case Data.RessourceType.Drink:
                UIManager.instance.RessourcePopup(npcBubble.transform.position, -order.ressourceAmount, UIManager.instance.drinkColor, UIManager.instance.drinkSprite);
                break;
            case Data.RessourceType.Food:
                UIManager.instance.RessourcePopup(npcBubble.transform.position, -order.ressourceAmount, UIManager.instance.foodColor, UIManager.instance.foodSprite);
                break;
        }

        var temp = Instantiate(order.prefab, gameObject.transform.position, Quaternion.identity);
        temp.transform.parent = gameObject.transform;
        dish = temp.GetComponent<Dish>();
        dish.counterSpace = counterSpace;
        dish.counter = counter;

        PlayerManager.instance.tavern.UpdateShelvesProps();

        GoTo(idlePosition.transform.position, navMeshAgent.stoppingDistance);
        nextAction = CookingAndMixing3;
    }
    ////3ème fonction qui place le plat sur le fourneau et calcule le temps de préparation de la commande
    public void CookingAndMixing3()
    {
        animator.SetTrigger(AnimStates.action);
        furnace.ToggleProp(true, this, dish);
        dish.particleSystem.Play();

        var timeToCook = (5 * order.ressourceAmount) - employeeValues.employeeSkills[2];

        nextAction = CookingAndMixing4;
        DelayAction(timeToCook);
    }
    ////4ème fonction qui envoie le cuisinier vers les comptoirs pour y déposer le plat
    public void CookingAndMixing4()
    {
        furnace.ToggleProp(false);
        GainExperience(employeeData.skillExpValues[0] * order.ressourceAmount);
        if (order.ressourceType == Data.RessourceType.Food) dish.QualityCalculator(employeeValues.employeeSkills[0]);
        else dish.QualityCalculator(employeeValues.employeeSkills[1]);

        GoTo(counter.gameObject);
        nextAction = CookingAndMixing5;
    }
    ////5ème fonction qui place le plat sur le comptoir et renvoir le cuisinier en inactivité
    public void CookingAndMixing5()
    {
        dish.gameObject.transform.position = counterSpace.transform.position;
        dish.gameObject.transform.parent = counterSpace.transform;
        dish.selectable = true;
        dish.interactable = true;
        dish.dishData.timeBeingFresh = dish.dishData.timeBetweenStates * (furnace.freshnessMultipliers[furnace.level - 1]);
        dish.ToggleExpiration();
        counterSpace.dish = dish;
        SelectionManager.instance.ShowServicableOrders.AddListener(dish.SelectableVisual);

        NPCManager.instance.tempDish.Add(dish.gameObject);
        UIManager.instance.clientInfos.UpdateDishInfos(order, false);

        GoTo(idlePosition.transform.position, employeeData.timeBeforeSearching);
        nextAction = SearchJob;
        CookIdle();
    }

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Cook
    public override void Select()
    {
        base.Select();

        //On indique au panneau de sélection de s'ouvrir avec les valeurs de ce plat
        UIManager.instance.selectionPanel.CookContent(this);


    }

    //Override InteractWith avec des paramètres propres à Waitress
    public override void InteractWith(InteractableObject interactableObject)
    {
        if (interactableObject.GetComponent<Dish>())
        {
            for (int i = 0; i < PlayerManager.instance.tavern.counters.Length; i++)
            {
                if (PlayerManager.instance.tavern.counters[i].freeSpaces.Count > 0)
                {
                    switch (interactableObject.GetComponent<Dish>().dishData.order.ressourceType)
                    {
                        case Data.RessourceType.Food:
                            if (interactableObject.GetComponent<Dish>().dishData.order.ressourceAmount <= PlayerManager.instance.PlayerFood)
                            {
                                if (state == State.Standby) CancelAction();
                                CookingAndMixing1(interactableObject.GetComponent<Dish>().dishData.order, PlayerManager.instance.tavern.counters[i]);
                                audioSource.clip = employeeData.interactionSounds[Random.Range(0, employeeData.interactionSounds.Length)];
                                audioSource.Play();
                                return;
                            }
                            UIManager.instance.playerFood.StatFlickerRed();
                            return;

                        case Data.RessourceType.Drink:
                            if (interactableObject.GetComponent<Dish>().dishData.order.ressourceAmount <= PlayerManager.instance.PlayerDrinks)
                            {
                                if (state == State.Standby) CancelAction();
                                CookingAndMixing1(interactableObject.GetComponent<Dish>().dishData.order, PlayerManager.instance.tavern.counters[i]);
                                audioSource.clip = employeeData.interactionSounds[Random.Range(0, employeeData.interactionSounds.Length)];
                                audioSource.Play();
                                return;
                            }
                            UIManager.instance.playerDrinks.StatFlickerRed();
                            return;
                    }
                }
            }
        }
    }

    //fonction qui annule l'action du cuisinier
    public void CancelAction()
    {
        //si on s'occupe déjà d'un commande...
        if (order != null && state == State.Standby)
        {
            NPCManager.instance.cookOrdersList.Add(order);
            switch (order.ressourceType)
            {
                case Data.RessourceType.Drink:
                    PlayerManager.instance.PlayerDrinks += order.ressourceAmount;
                    break;
                case Data.RessourceType.Food:
                    PlayerManager.instance.PlayerFood += order.ressourceAmount;
                    break;
            }
            order = null;
        }
    }

    #region ///  VIEUX CODE  ///
    /*
    public Data.CookState cookState = Data.CookState.Free;

    public Order orderToPrepare;
    public int foodPriority, drinkPriority;
    public GameObject cooker, ressource, effects, counter, reservedServingPlace;
    GameObject stock;
    GameObject[] servingPlaces;
    public Vector3 idlePosition;

    int cookemployeeWage = 5, cookemployeeRecruitmentFee = 20;
    public float cookingExperienceGain = 1;
    public int mixology = 1, cuisine = 1, talent = 1;

    private void Awake()
    {
        employeeWage = cookemployeeWage;
        employeeRecruitmentFee = cookemployeeRecruitmentFee;
    }

    // Use this for initialization
    new void Start () {
        base.Start();
        foodPriority = 0;
        drinkPriority = 0;
        stock = GameObject.FindGameObjectWithTag("Stock");
        servingPlaces = GameObject.FindGameObjectsWithTag("ServingPlace");
        idlePosition = transform.position;
    }

    new void Update ()
    {
        base.Update();
    }

    void OnTriggerEnter(Collider other)
    {
        CheckIfReachedTarget(other.gameObject);
    }

    //fonction qui permet au cuisinier de chercher une tâche à effectuer en prenant en compte la priorité associée à chaque tâche
    public void SearchJob()
    {
        //si des commandes sont disponibles en cuisine...
        if (ServiceManager.instance.cookOrdersList.Count != 0)
        {
            //on crée une liste avec les différentes valeurs de priorité de chaque actions.
            List<int> actions = new List<int>();
            if (foodPriority != 0) actions.Add(foodPriority);
            if (drinkPriority != 0) actions.Add(drinkPriority);

            //on crée des booléens afin de n'effectuer les recherches de tâche qu'une seule fois
            bool hasSearchedForFoodJob = false, hasSearchedForDrinkJob = false;

            //on prends la priorité la plus élevée
            int highestPriority = Mathf.Max(actions.ToArray());

            //pour chaque actions à effectuer...
            for (int i = 0; i < actions.Count; i++)
            {
                //si la tâche ayant la plus haute priorité est de cuisiner de la nourriture...(si cette tâche à la même valeur de priorité qu'une autre, elle sera effectuée en première)
                //et que la recherche n'a pas déjà été effectuée
                //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
                if(highestPriority == foodPriority && !hasSearchedForFoodJob && foodPriority != 0)
                {
                    //on crée une liste de commandes de nourriture et on la remplie
                    List<Order> foodOrders = new List<Order>();
                    foreach (Order order in ServiceManager.instance.cookOrdersList)
                    {
                        if (order.ressourceType == Data.RessourceType.Food) foodOrders.Add(order);
                    }

                    //si on a bien des commandes de nourriture...
                    if (foodOrders.Count != 0)
                    {
                        for (int o = 0; i < foodOrders.Count; i++)
                        {
                            if (CheckIfCanPrepare(foodOrders[o], false)) return;
                        }
                    }

                    //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                    hasSearchedForFoodJob = true;
                }

                //si la tâche ayant la plus haute priorité est de préparer une boisson...
                //et que la recherche n'a pas déjà été effectuée
                //et que le joueur a bien demandé au jeu d'effectuer cette tâche automatiquement
                if (highestPriority == drinkPriority && !hasSearchedForDrinkJob && drinkPriority != 0)
                {
                    //on crée une liste de commandes de nourriture et on la remplie
                    List<Order> drinkOrders = new List<Order>();
                    foreach (Order order in ServiceManager.instance.cookOrdersList)
                    {
                        if (order.ressourceType == Data.RessourceType.Drink) drinkOrders.Add(order);
                    }

                    //si on a bien des commandes de boissons...
                    if (drinkOrders.Count != 0)
                    {
                        for (int o = 0; i < drinkOrders.Count; i++)
                        {
                            if (CheckIfCanPrepare(drinkOrders[o], false)) return;
                        }
                    }

                    //si aucune tâche n'a été trouvée, on confirme d'avoir déjà effectué cette recherche
                    hasSearchedForDrinkJob = true;
                }

                highestPriority--;
            }
        }
    }

    public bool CheckIfCanPrepare (Order order, bool playerAsked)
    {
        if (cookState == Data.CookState.Free && PhaseManager.instance.currentPhase == Data.CurrentPhase.Service)
        {
            switch (order.ressourceType)
            {
                case Data.RessourceType.Drink:
                    if (order.ressourceAmount <= ServiceManager.instance.PlayerDrinks)
                    {
                        GetResource(order);
                        ServiceManager.instance.PlayerDrinks -= order.ressourceAmount;
                        UIManager.instance.OpenResourceVariation(order.ressourceType, -order.ressourceAmount);
                        return true;
                    }
                    else
                    {
                        UIManager.instance.FlickerRed(UIManager.instance.drink);
                        return false;
                    }

                case Data.RessourceType.Food:
                    if (order.ressourceAmount <= ServiceManager.instance.PlayerFood)
                    {
                        GetResource(order);
                        ServiceManager.instance.PlayerFood -= order.ressourceAmount;
                        UIManager.instance.OpenResourceVariation(order.ressourceType, -order.ressourceAmount);
                        return true;
                    }
                     else
                    {
                        UIManager.instance.FlickerRed(UIManager.instance.food);
                        return false;
                    }
                default:
                    return false;
            }
        }
        else return false;
    }

    public void GetResource (Order order)
    {
        orderToPrepare = order;
        GoTo(stock);
        foreach (Order orderToCheck in ServiceManager.instance.cookOrdersList)
        {
            if (orderToPrepare == orderToCheck)
            {
                ServiceManager.instance.cookOrdersList.Remove(order);
                break;
            }
        }
        cookState = Data.CookState.GettingResource;
    }

    [PunRPC]
    public void RessourcePrefab ()
    {
        ressource = Instantiate(orderToPrepare.ressourcePrefab, transform.position, orderToPrepare.ressourcePrefab.transform.rotation);
        ressource.transform.parent = gameObject.transform;
        ServiceManager.instance.tempDish.Add(ressource);
    }

    [PunRPC]
    public void EffectPrefab()
    {
        effects = Instantiate(orderToPrepare.effectsPrefab, ressource.transform);
        effects.transform.position = new Vector3(ressource.transform.position.x, (ressource.transform.position.y +1), ressource.transform.position.z);

        effects = null;
    }

    public void CookOrder ()
    {
        ressource.transform.position = cooker.transform.position;
        ressource.transform.parent = cooker.transform;
        float timeToCook;
        if (orderToPrepare.ressourceType == Data.RessourceType.Drink) timeToCook = (5 * orderToPrepare.ressourceAmount) - mixology;
        else timeToCook = (5 * orderToPrepare.ressourceAmount) - cuisine;
        StartCoroutine(Cooking(timeToCook));
    }

    IEnumerator Cooking(float timeToCook)
    {
        yield return new WaitForSeconds(timeToCook);
        experience += cookingExperienceGain * orderToPrepare.ressourceAmount;
        while (!ServiceManager.instance.tavern.GiveCounterSpace(this))
        {
            yield return new WaitForSeconds(1f);
        }
        cookState = Data.CookState.PuttingOrderOnTable;
        GetComponent<PhotonView>().RPC("EffectPrefab", RpcTarget.All);
        GoTo(counter);
        ressource.GetComponent<Dish>().order = orderToPrepare;
        ressource.GetComponent<Dish>().selectable = true;
    }


    public void RoundEnd()
    {
        Destroy(ressource);
        cookState = Data.CookState.Free;
        reservedServingPlace = null;
        GoTo(idlePosition);
    }
    */
    #endregion
}
