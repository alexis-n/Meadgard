using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectionManager : MonoBehaviour
{

    #region ///  VARIABLES  ///
    public static SelectionManager instance;
    public InteractableObject selectedObject;
    public LineRenderer lineRenderer;

    // Classe afin de créer des évènements auquels ont pourra renseigner une couleur
    [System.Serializable]
    public class ColoredEvent : UnityEvent<Color, bool, string> { }
    public ColoredEvent ShowServicableClients = new ColoredEvent();
    public ColoredEvent ShowKickableClients = new ColoredEvent();
    public ColoredEvent ShowServicableOrders = new ColoredEvent();
    public ColoredEvent ShowFreeCounters = new ColoredEvent();
    public ColoredEvent ShowOrderingTables = new ColoredEvent();
    public ColoredEvent ShowTrash = new ColoredEvent();
    public ColoredEvent HideSelectableElements = new ColoredEvent();
    #endregion

    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (selectedObject != null) selectedObject.UpdateSelection();
    }

    //fonction qui permet de sélectionner un objet
    public void SelectNewObject (InteractableObject newObject)
    {
        //avant toutes choses on déselectionne l'objet précedemment sélectionné si il y en a un
        if (selectedObject != null) UnselectObject();

        selectedObject = newObject;
        selectedObject.SelectionAnimation(selectedObject.data.color);
        selectedObject.Select();
        HideSelectableElements.RemoveListener(selectedObject.SelectableVisual);
    }

    //fonction qui permet de glisser un objet
    public void DragObject ()
    {
        selectedObject.Drag();
        MouseFollower.instance.draggingImage.gameObject.SetActive(true);
        MouseFollower.instance.draggingImage.sprite = selectedObject.data.icon;
    }

    //fonction qui permet de glisser un objet
    public void DropObject(InteractableObject droppedOn = null)
    {
        MouseFollower.instance.draggingImage.gameObject.SetActive(false);
        if (droppedOn) selectedObject.DroppedOn(droppedOn);
        else selectedObject.Drop();
    }

    //fonction qui déselectionne l'objet actuellement sélectionné
    public void UnselectObject ()
    {
        HideSelectableElements.AddListener(selectedObject.SelectableVisual);
        HideSelectableElements.Invoke(selectedObject.data.color, false, string.Empty);
        var temp = selectedObject;
        selectedObject = null;
        temp.Unselect();
    }

    //fonction qui permet à un InteractableObject d'intéragir avec un autre
    public void InteractWith(InteractableObject givenObject)
    {
        selectedObject.InteractWith(givenObject);
        HideSelectableElements.Invoke(selectedObject.data.color, false, string.Empty);
    }

        #region ///  VIEUX CODE  ///
        /*
        public static SelectionManager instance;

        public GameObject selectedObject;
        public InteractableObject interactableObject;
        public Data.Instruction instruction;
        public LineRenderer line;

        // Use this for initialization
        void Start () {
            instance = this;
            line = GetComponent<LineRenderer>();
        }

        public void SelectObject (GameObject newSelectedObject)
        {
            if (newSelectedObject != selectedObject)
            {
                if (selectedObject != null) selectedObject.GetComponent<InteractableObject>().Unselect();
                selectedObject = newSelectedObject;
                interactableObject = selectedObject.GetComponent<InteractableObject>();
                selectedObject.GetComponent<InteractableObject>().Select();

                if (selectedObject.GetComponent<NPC>())
                {
                    switch (selectedObject.GetComponent<NPC>().npcType)
                    {
                        case Data.NPCType.Waitress:
                            ServiceManager.instance.ShowInteractableClients.Invoke();
                            instruction = Data.Instruction.GivingInstruction;
                            break;
                    }
                }
                if (selectedObject.GetComponent<Dish>())
                {
                    selectedObject.GetComponent<Dish>().Select();
                }
            }
        }

        public void GiveOrder(GameObject chosenObject)
        {
            if (instruction == Data.Instruction.GivingInstruction)
            {
                if (chosenObject.GetComponent<NPC>() && selectedObject.GetComponent<Waitress>())
                {
                    if (chosenObject.GetComponent<Client>())
                    {
                        if (chosenObject.GetComponent<Client>().state == Client.State.Ordering)
                        {
                            selectedObject.GetComponent<Waitress>().OrderPickup(chosenObject.GetComponent<Client>());
                            ServiceManager.instance.HideSelectableElements.Invoke();
                            instruction = Data.Instruction.Empty;
                        }
                        else if (chosenObject.GetComponent<Client>().state == Client.State.Ordering)
                        {
                            selectedObject.GetComponent<Waitress>().DishServing(chosenObject.GetComponent<Client>());
                            ServiceManager.instance.HideSelectableElements.Invoke();
                            instruction = Data.Instruction.Empty;
                        }
                    }
                }
                if (chosenObject.GetComponent<NPC>() && selectedObject.GetComponent<Dish>())
                {
                    if (chosenObject.GetComponent<Client>() &&
                        chosenObject.GetComponent<Client>().state == Client.State.Ordering &&
                        chosenObject.GetComponent<Client>().order == selectedObject.GetComponent<Dish>().order)
                    {
                        for (int i = 0; i < ServiceManager.instance.waitresses.Count; i++)
                        {
                            if (ServiceManager.instance.waitresses[i].state == Waitress.State.Free)
                            {
                                //ServiceManager.instance.waitresses[i].ServeOrder(chosenObject.GetComponent<Client>(), true, selectedObject);
                                UIManager.instance.selectionPanel.ClosePanel();
                                ServiceManager.instance.HideSelectableElements.Invoke();
                                selectedObject.GetComponent<InteractableObject>().Unselect();
                                return;
                            }
                        }
                    }
                }
            }
        }
        */
        #endregion
    }
