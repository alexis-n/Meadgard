using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : InteractableObject
{
    #region ///  VARIABLES  ///
    public TrashData trashData;
    public bool takenCareOf = false;
    #endregion

    private void Awake()
    {
        base.data = trashData;

        outline = GetComponent<MouseOverOutline>();
        outline.OutlineColor = data.color;
        outline.enabled = false;
        SelectionManager.instance.ShowTrash.AddListener(SelectableVisual);
        SelectionManager.instance.HideSelectableElements.AddListener(SelectableVisual);
    }

    //fonction qui active les visuels d'intéragibilité
    public override void SelectableVisual(Color newColor, bool toggle, string tooltipRightClick)
    {
        if(outline)
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
                outline.OutlineColor = data.color;
                outline.enabled = false;
                keepOutlineOn = false;

                tooltipable.rightClick = string.Empty;
            }
        }
    }
}
