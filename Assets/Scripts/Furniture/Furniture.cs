using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class Furniture : InteractableObject
{
    #region ///  VARIABLES  ///
    public FurnitureData furnitureData;

    public int level;
    [HideInInspector] public int maxLevel;

    [SerializeField][Tooltip("Mettre les visuels dans l'ordre (niveau 1, niveau 2, niveau 3...")]
    protected GameObject[] visuals;
    [SerializeField]
    protected GameObject[] ghosts;
    #endregion

    private void Start()
    {
        base.data = furnitureData;
        maxLevel = visuals.Length;
        data.color = PlayerManager.instance.tavern.god.color;
        outline.OutlineColor = data.color;
        foreach (var item in visuals) { item.SetActive(false); }
        foreach (var item in ghosts) { item.SetActive(false); }
    }

    //On override la fonction CheckIfSelectable afin de préciser des conditions propres à Dish
    public override bool CheckIfSelectable()
    {
        //Dans le cas des meubles, si le meuble n'est pas construit et qu'on est en phase de service, on ne peut pas le sélectionner 
        if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Service) selectable = false;
        else selectable = true;

        return base.CheckIfSelectable();
    }

    //Cette fonction ne sert qu'à faciliter la construction des meubles avec leur propres conditions
    public virtual void Build()
    {
        PhaseManager.instance.servicePhaseEvent.AddListener(HideSpawner);
        PhaseManager.instance.prepPhaseEvent.AddListener(ShowSpawner);
    }

    //Fonction virtuelle qui permet d'upgrade un meuble avec ses propres conditions
    public virtual void Upgrade()
    {
        if (level < maxLevel) level++;
        UIManager.instance.newsboardMenu.hasUpgradedFurniture = true;

        HideSpawner();
        ShowSpawner();
    }

    //Fonction qui cache le spawner de l'objet quand on passe en phase de service et que l'object est de niveau 0
    public virtual void HideSpawner() { }

    //Fonction qui affiche le spawner de l'objet quand on passe en phase de préparation et que l'object est de niveau 0
    public virtual void ShowSpawner() { }

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
    [RequireComponent(typeof(SelectableObject))]
    public class Furniture : MonoBehaviour {

        public Data.FurnitureType type;
        public UnityEvent upgrade = new UnityEvent();

        public SelectableObject selectableObject;
        public int owner;
        public Sprite sprite;
        public new string name;
        public int level = 0;
        public bool canBeUpgraded = true, mustSpawnAtStart = false, full = false;
        public GameObject spawner, level1, level2, level3;
        public int upgradePrice, level1Price, level2Price, level3Price;

        protected void Initialization()
        {
            owner = ServiceManager.instance.tavern.owner;
            selectableObject = GetComponent<SelectableObject>();
            upgradePrice = level1Price;
            PhaseManager.instance.prepPhaseEvent.AddListener(ShowSelf);
            PhaseManager.instance.servicePhaseEvent.AddListener(HideSelf);
        }

        public void Upgrade()
        {
            if (level == 0)
            {
                PhaseManager.instance.prepPhaseEvent.RemoveListener(ShowSelf);
                PhaseManager.instance.servicePhaseEvent.RemoveListener(HideSelf);
            }
            upgrade.Invoke();
        }

        public bool VerifyOwnership()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == owner) return true;
            else return false;
        }

        public void HideSelf()
        {
            if (type != Data.FurnitureType.Queue)
            {
                spawner.SetActive(false);
                gameObject.GetComponent<SelectableObject>().enabled = false;
            }
        }

        public void ShowSelf()
        {
            if (type != Data.FurnitureType.Queue)
            {
                if (level == 0) spawner.SetActive(true);
                gameObject.GetComponent<SelectableObject>().enabled = true;
            }
        }
    }*/
    #endregion
}


