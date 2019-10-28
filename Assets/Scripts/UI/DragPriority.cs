using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPriority : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DragPriority draggedElement;
    public string identifier;
    public PrioritySlot currentSlot, newSlot;

    public void OnBeginDrag (PointerEventData eventData)
    {
        //quand on commence à glisser cet élément, on l'apparente à la souris 
        transform.SetParent(UIManager.instance.mouseFollower);
        UIManager.instance.cursorsBank.Drag();
        draggedElement = this;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        currentSlot.slottedPriority = null;
    }

    public void OnDrag(PointerEventData eventData){}

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        UIManager.instance.cursorsBank.EndDrag();
        //si on assigne un nouveau slot à cet élément
        if (newSlot != null) FillSlot(newSlot);
        //sinon on retourne à la position de base
        else transform.SetParent(currentSlot.transform);
        transform.localPosition = Vector3.zero;
    }

    public void FillSlot(PrioritySlot givenSlot)
    {
        transform.SetParent(givenSlot.transform);
        transform.SetAsFirstSibling();
        transform.localPosition = Vector3.zero;
        currentSlot = givenSlot;
        newSlot = null;
        UIManager.instance.selectionPanel.EmployeePriority(identifier, currentSlot.value);
    }
}
