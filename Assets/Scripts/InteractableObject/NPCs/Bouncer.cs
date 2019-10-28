using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bouncer : Employee
{
    #region /// VARIABLES ///
    public enum State { Free, Standby, KickingClient };
    [HideInInspector] public State state = State.Free;

    [SerializeField] private GameObject bag;
    [SerializeField] private ParticleSystem kickFX;

    [HideInInspector] public Client client;

    #endregion

    // Use this for initialization
    new void Awake()
    {
        base.Awake();
        bag.SetActive(false);
    }

    new void Update()
    {
        base.Update();
    }

    //Fonction qui passe le vigile en état d'inactivité
    public void BouncerIdle()
    {
        state = State.Free;
        client = null;
        transform.localEulerAngles = Vector3.zero;
    }

    //Fonctions gérant l'expulsion des clients
    ////1ère fonction qui envoie le vigile auprès du client et applique un délais avant KickOut2
    public void KickOut(Client givenClient)
    {
        state = State.Standby;
        client = givenClient;

        if (client.state == Client.State.Ordering)
        {
            client.chair.table.clientsOrdering.Remove(client);
            UIManager.instance.clientInfos.UpdateOrderInfos(false);
        }  
        else if (NPCManager.instance.clientsWaitingForDish.Contains(client))
        {
            SelectionManager.instance.ShowServicableClients.RemoveListener(client.SelectableVisual);
            NPCManager.instance.clientsWaitingForDish.Remove(client);
            UIManager.instance.clientInfos.UpdateDishInfos(client.order, false);
            NPCManager.instance.cookOrdersList.Remove(client.order);
        }

        client.stopTimer = true;

        SelectionManager.instance.ShowKickableClients.RemoveListener(client.SelectableVisual);

        npcBubble.bubble.icon.sprite = employeeData.skillIcons[1];
        GoTo(client.gameObject ,0.5f);
        nextAction = KickOut2;
    }

    //2ème fonction qui fait s'emparer le vigile du client, le met dans un sac et le dirige vers la porte
    public void KickOut2()
    {
        state = State.KickingClient;
        client.npcBubble.activate = false;

        PlayerManager.instance.tavern.availableRestaurantChairs.Add(client.chair);
        client.chair = null;

        if (client.bandit)
        {
            client.bandit.state = Bandit.BanditState.Caught;
            NPCManager.instance.banditsSeated.Remove(client);
        }

        client.Autodestroy();

        bag.SetActive(true);

        animator.SetTrigger(AnimStates.hold);

        GoTo(PlayerManager.instance.tavern.tavernEntrance.transform.position, 0.01f);
        nextAction = KickOut3;
    }

    //3ème fonction qui fait décolle le client et revenir le vigile à sa place d'origine
    public void KickOut3()
    {
        animator.SetTrigger(AnimStates.kick);

        kickFX.Play();
        bag.transform.DOMove(new Vector3(bag.transform.position.x,65, bag.transform.position.z + 50), 7f).SetRelative().SetDelay(0.5f)
            .OnComplete(() => ResetBag());
        bag.transform.DOLocalRotate(new Vector3(359, 359, 359), 7f).SetRelative().SetDelay(0.5f);

        nextAction = KickOut4;
        DelayAction(0.75f);
    }
    public void KickOut4()
    {
        if (client.bandit != null)
        {
            PlayerManager.instance.PlayerMoney += client.bandit.bounty;
            UIManager.instance.blackPanel.recapPanel.clientsKickValue += client.bandit.bounty;
        }
        else
        {
            PlayerManager.instance.PlayerMoney -= GameManager.instance.data.moneyToPayForKicking;
            UIManager.instance.blackPanel.recapPanel.clientsKickValue -= GameManager.instance.data.moneyToPayForKicking;
        }

        nextAction = BouncerIdle;
        npcBubble.bubble.icon.sprite = employeeData.skillIcons[0];
        GoTo(idlePosition.transform.position, navMeshAgent.stoppingDistance);
    }

    public void ResetBag()
    {
        kickFX.Stop();
        bag.transform.localPosition = Vector3.zero;
        bag.transform.localEulerAngles = Vector3.zero;
        bag.SetActive(false);
    }

    //Fonction qui override la fonction Select de base en y ajoutant des actions spécifiques à Waitress
    public override void Select()
    {
        base.Select();
        //On indique au panneau de sélection de s'ouvrir avec les valeurs de ce plat
        UIManager.instance.selectionPanel.BouncerContent(this);

        UpdateSelection();
    }
    //Fonction qui override la fonction UpdateSelection de base en y ajoutant des actions spécifiques à Waitress
    public override void UpdateSelection()
    {
        DrawPath();

        //on affiche tout les objets avec lesquels Bouncer peut intéragir
        if (state == State.Free || state == State.Standby) SelectionManager.instance.ShowKickableClients.Invoke(employeeData.skillsColor[1], true, "Expulser");
    }

    //Override InteractWith avec des paramètres propres à Waitress
    public override void InteractWith(InteractableObject interactableObject)
    {
        if (state == State.Free || state == State.Standby)
        {
            if (interactableObject.GetComponent<Client>() &&
            interactableObject.GetComponent<Client>().CheckIfInteractable())
            {
                if (interactableObject.GetComponent<Client>().state == Client.State.AwaitingDish ||
                    interactableObject.GetComponent<Client>().state == Client.State.Ordering)
                {
                    if (state == State.Standby && client != null)
                    {
                        if (state == State.Standby) CancelAction();
                        KickOut(interactableObject.GetComponent<Client>());
                        audioSource.clip = employeeData.interactionSounds[Random.Range(0, employeeData.interactionSounds.Length)];
                        audioSource.Play();
                    }
                    else
                    {
                        KickOut(interactableObject.GetComponent<Client>());
                        audioSource.clip = employeeData.interactionSounds[Random.Range(0, employeeData.interactionSounds.Length)];
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void CancelAction()
    {
        //si on s'occupe déjà d'un client...
        if (client != null)
        {
            SelectionManager.instance.ShowKickableClients.AddListener(client.SelectableVisual);
            client.stopTimer = false;

            if (client.state == Client.State.Ordering)
            {
                UIManager.instance.clientInfos.UpdateOrderInfos(true);
                client.chair.table.clientsOrdering.Add(client);
            }
            else if (client.state == Client.State.AwaitingDish)
            {
                NPCManager.instance.clientsWaitingForDish.Add(client);
                SelectionManager.instance.ShowServicableClients.AddListener(client.SelectableVisual);
                NPCManager.instance.cookOrdersList.Add(client.order);
                UIManager.instance.clientInfos.UpdateDishInfos(client.order);
            }
        }
    }
}
