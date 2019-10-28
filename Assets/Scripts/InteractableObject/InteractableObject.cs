using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//On a besoin d'un collider afin de pouvoir sélectionner cet object (raycast)
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MouseOverOutline))]
[RequireComponent(typeof(Tooltipable))]
public class InteractableObject : MonoBehaviour
{
    #region ///  VARIABLES  ///
    public new string name;
    private InteractableObjectData privateData;
    [HideInInspector] public InteractableObjectData data { get { return privateData; } set { privateData = value;  Initialization(); }}

    [SerializeField] private new SelectionAnimation animation;
    [SerializeField] private bool displayOnUI = true;

    [HideInInspector] protected MouseOverOutline outline;
    [HideInInspector] protected Tooltipable tooltipable;
    private Color startcolor;

    protected bool keepOutlineOn = false;

    public bool selectable = false, interactable = false, draggable = false;
    #endregion

    protected void Initialization()
    {
        outline = GetComponent<MouseOverOutline>();
        tooltipable = GetComponent<Tooltipable>();
        outline.OutlineColor = privateData.color;
        outline.enabled = false;
        selectable = CheckIfSelectable();
        SelectionManager.instance.HideSelectableElements.AddListener(SelectableVisual);
    }

    //Fonction qui lance l'animation
    public virtual void SelectionAnimation(Color color)
    {
        if (animation != null) animation.Play(color);
    }
    //Fonction qui stoppe l'animation
    public void RewindAnimation()
    {
        if (animation != null) animation.Rewind();
    }
    //Fonction qui lance l'animation
    public virtual void SelectableVisual(Color newColor, bool toggle, string tooltipRightClick)
    {
        if (CheckIfInteractable() && toggle)
        {
            outline.enabled = true;

            if (outline.OutlineColor.r != newColor.r &&
               outline.OutlineColor.g != newColor.g &&
               outline.OutlineColor.b != newColor.b)
                outline.OutlineColor = new Color(newColor.r, newColor.g, newColor.b, 0.25f);
            outline.OutlineWidth = 5f;

            keepOutlineOn = true;

            tooltipable.rightClickColor = newColor;
            tooltipable.rightClick = tooltipRightClick;
        }
        else if (!toggle)
        {
            outline.OutlineColor = privateData.color;
            outline.enabled = false;
            keepOutlineOn = false;

            tooltipable.rightClick = string.Empty;
        }
    }

    //Fonction qui demande aux classes enfant de vérifier si elle sont intéragibles
    ///Cette fonction est virtuelle car on a besoin de l'overrider pour préciser des conditions de 
    ///sélection en fonction de l'objet
    public virtual bool CheckIfInteractable()
    {
        return interactable;
    }
    //Fonction qui demande aux classes enfant de vérifier si elle sont sélectionnables
    ///Cette fonction est virtuelle car on a besoin de l'overrider pour préciser des conditions de 
    ///sélection en fonction de l'objet
    public virtual bool CheckIfSelectable()
    {
        return selectable;
    }

    //Fonction qui vérifie si l'objet appartient bien au joueur qui essaie de le sélectionner
    /*
    public bool VerifyOwnership()
    {

    }
    */

    //Fonction qui sélectionne l'objet et l'indique au joueur, et qui effectue des actions spécifique
    //si une autre méthode l'override
    public virtual void Select()
    {
        //Si ces variables ont été remplies, on agit en conséquence
        if (animation != null) animation.Play(animation.baseColor);
    }
    //Fonction qui sélectionne l'objet et l'indique au joueur, et qui effectue des actions spécifique
    //si une autre méthode l'override
    public virtual void Drag() { }
    public virtual void DroppedOn(InteractableObject droppedOn) { }
    public virtual void Drop() { }
    //Fonction qui update un objet sélectionné, et qui effectue des actions spécifique
    //si une autre méthode l'override
    public virtual void UpdateSelection()
    {
        //Cette fonction sera overridée
    }

    //Fonction qui désélectionne l'objet et l'indique au joueur
    public virtual void Unselect()
    {
        //Si ces variables ont été remplies, on agit en conséquence
        if (animation != null) animation.Rewind();
        UIManager.instance.selectionPanel.ClosePanel();
        OnMouseExit();
    }

    //fonction virtuelle appelée quand on veut que cet objet intéragisse avec un autre
    public virtual void InteractWith(InteractableObject interactableObject)
    {
        //Cette fonction sera overridée
    }

    //Fonction qui permet d'activer les visuels si il y en a quand le joueur pose son curseur sur l'objet
    public virtual void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //si on a pas déjà selectionné l'objet, si on peut le selectionner, et si on a bien un visuel à affiché
            if (SelectionManager.instance.selectedObject != this &&
                CheckIfSelectable())
            {
                UIManager.instance.cursorsBank.Hover();
                outline.enabled = true;
                outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, 1f);
                outline.OutlineWidth = 3f;
            }

            //si on a déjà selectionné un objet et que celui-ci peut intéragir avec l'object sélectionné, on affiche le visuel
            else if (SelectionManager.instance.selectedObject != null &&
                CheckIfInteractable())
            {
                UIManager.instance.cursorsBank.Hover();
                outline.enabled = true;
                outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, 1f);
                outline.OutlineWidth = 3f;
            }
        }
    }

    //Fonction qui désactive les visuels de souris si il y en a quand le joueur n'a plus son curseur sur l'objet
    public virtual void OnMouseExit()
    {
        UIManager.instance.cursorsBank.Basic();
        if (SelectionManager.instance.selectedObject == this) return;
        if (!keepOutlineOn) outline.enabled = false;
        else
        {
            outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, 0.25f);
            outline.OutlineWidth = 3f;
        }

        SelectionManager.instance.lineRenderer.positionCount = 0;
    }
}
