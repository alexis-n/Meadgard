using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrioritySlot : MonoBehaviour, IDropHandler
{
    public int value = 0;
    public DragPriority slottedPriority;

    private void Start()
    {
        if (slottedPriority != null) slottedPriority.currentSlot = this;
    }

    public void OnDrop (PointerEventData eventData)
    {
        //si on veux échanger deux éléments
        if (slottedPriority != null)
        {
            slottedPriority.FillSlot(DragPriority.draggedElement.currentSlot);
            DragPriority.draggedElement.currentSlot.slottedPriority = slottedPriority;
        }
        slottedPriority = DragPriority.draggedElement;
        slottedPriority.newSlot = this;
    }
}
