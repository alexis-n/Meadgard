using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : InteractableObject
{
    #region ///  VARIABLES  ///
    public DishData dishData;

    public enum Quality { Bland = -1, Average = 0, Tasty = 1, Delicious = 2, Masterpiece = 3};
    public Quality quality = Quality.Average;

    public enum Freshness { Stale = -1, Normal = 0, Fresh = 1};
    public Freshness freshness = Freshness.Fresh;
    private Coroutine coroutine;

    [HideInInspector] public CounterSpace counterSpace;
    [HideInInspector] public Counter counter;
    public new ParticleSystem particleSystem;

    public Transform handle;

    public float qualityPriceModifier = 2;
    #endregion

    private void Start()
    {
        base.data = dishData;
        base.Initialization();
        PhaseManager.instance.recapPhaseEvent.AddListener(Autodestruct);
    }

    //On override la fonction CheckIfSelectable afin de préciser des conditions propres à Dish
    public override bool CheckIfSelectable()
    {
        //On a pas de conditions spécifiques dans le cas de Dish
        return base.CheckIfSelectable();
    }

    //fonction qui au plat un niveau de qualité en fonction du cuisinier qui la prépare
    public void QualityCalculator(int level)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        var temp = Random.Range(0, 100);

        switch (level)
        {
            case 1:
                if (temp < 25) quality = Quality.Bland;
                else if (temp > 75) quality = Quality.Tasty;
                else quality = Quality.Average;
                break;
            case 2:
                if (temp < 25) quality = Quality.Average;
                else if (temp > 75) quality = Quality.Delicious;
                else quality = Quality.Tasty;
                break;
            case 3:
                if (temp < 25) quality = Quality.Tasty;
                else if (temp > 75) quality = Quality.Masterpiece;
                else quality = Quality.Delicious;
                break;
            case 4:
                if (temp < 50) quality = Quality.Delicious;
                else quality = Quality.Masterpiece;
                break;
            case 5:
                if (temp < 25) quality = Quality.Delicious;
                else quality = Quality.Masterpiece;
                break;
        }
    }
    //Fonction qui permet de calculer le prix final d'un plat et de la retourner en tant qu'int
    public int FinalPriceCalculator()
    {
        float finalPrice = dishData.order.ressourcePrice;
        if (dishData.order.ressourceType == Data.RessourceType.Food) finalPrice = finalPrice * (int)Missive.currentMissive.foodRestockPrice;
        else finalPrice = finalPrice * (int)Missive.currentMissive.drinkRestockPrice;

        //si on ne peut pas descendre ou monter en qualité, on applique juste le niveau de qualité...
        if (freshness == Freshness.Stale && quality == Quality.Bland ||
            freshness == Freshness.Fresh && quality == Quality.Masterpiece)
            finalPrice += qualityPriceModifier * (int)quality;
        //...sinon on applique le niveau de qualité ET le niveau de fraîcheur
        else finalPrice += qualityPriceModifier * ((int)freshness + (int)quality);

        return (int)finalPrice;
    }

    //Fonction qui lance la péremption du plat
    public void ToggleExpiration(bool yes = true)
    {
        if (yes) coroutine = StartCoroutine(Expiration());
        else StopCoroutine(coroutine);

    }
    //Coroutine qui fait périr le plat
    IEnumerator Expiration()
    {
        var emission = particleSystem.emission;

        if (dishData.timeBeingFresh > 0)
        {
            if(dishData.order.ressourceType == Data.RessourceType.Food) yield return new WaitForSeconds(dishData.timeBeingFresh * Missive.currentMissive.foodRotting);
            else yield return new WaitForSeconds(dishData.timeBeingFresh * Missive.currentMissive.drinkRotting);
        }

        freshness = Freshness.Normal;
        emission.rateOverTime = 5f;
        if (dishData.timeBetweenStates > 0)
        {
            if (dishData.order.ressourceType == Data.RessourceType.Food) yield return new WaitForSeconds(dishData.timeBetweenStates * Missive.currentMissive.foodRotting);
            else yield return new WaitForSeconds(dishData.timeBetweenStates * Missive.currentMissive.drinkRotting);
        }
        freshness = Freshness.Stale;
        particleSystem.Stop();
    }

    //fonction qui permet au plat de s'autodétruire en fin de phase de service
    public void Autodestruct()
    {
        if (NPCManager.instance.tempDish.Contains(gameObject))
        {
            NPCManager.instance.tempDish.Remove(gameObject);
            counter.SwitchServingPlace(counterSpace);
            Destroy(gameObject, 1f);
        }
    }

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Dish
    public override void Select()
    {
        base.Select();

        //On indique au panneau de sélection de s'ouvrir avec les valeurs de ce plat
        UIManager.instance.selectionPanel.DishContent(this);

        for (int i = 0; i < NPCManager.instance.clientsWaitingForDish.Count; i++)
        {
            if (NPCManager.instance.clientsWaitingForDish[i].order == dishData.order) NPCManager.instance.clientsWaitingForDish[i].SelectableVisual(dishData.order.ressourceColor, true, string.Empty);
        }

        //Si une serveuse est disponible, on lui indique les clients à qui il peut servir ce plat
        for (int i = 0; i < NPCManager.instance.hiredWaitresses.Count; i++)
        {
            if (NPCManager.instance.hiredWaitresses[i].state == Waitress.State.Free)
            {
                for (int o = 0; o < NPCManager.instance.clientsWaitingForDish.Count; o++)
                {
                    //if (ServiceManager.instance.clientsWaitingForDish[i].order == order) ServiceManager.instance.clientsWaitingForDish[i].Selectable();
                }
                break;
            }
        }
    }
    //Override InteractWith avec des paramètres propres à Waitress
    public override void DroppedOn(InteractableObject interactableObject)
    {
        if (interactableObject.GetComponent<Client>() &&
            interactableObject.GetComponent<Client>().CheckIfInteractable())
        {
            if (interactableObject.GetComponent<Client>().order == dishData.order)
            {
                for (int i = 0; i < NPCManager.instance.hiredWaitresses.Count; i++)
                {
                    if (NPCManager.instance.hiredWaitresses[i].state == Waitress.State.Free)
                    {
                        NPCManager.instance.hiredWaitresses[i].DishServing(interactableObject.GetComponent<Client>(), this);
                    }
                }
            }
        }
    }

    #region ///  VIEUX CODE  ///
    /*
    public Order order;
    public bool onWait = false;

    public void SelectClientToServe()
    {
        SelectionManager.instance.instruction = Data.Instruction.GivingInstruction;
        ServiceManager.instance.HideSelectableElements.Invoke();
        for (int i = 0; i < ServiceManager.instance.clientsWaitingForDish.Count; i++)
        {
            if (ServiceManager.instance.clientsWaitingForDish[i].order == order) ServiceManager.instance.clientsWaitingForDish[i].CanBeSelected();
        }
    }
    */
    #endregion
}
