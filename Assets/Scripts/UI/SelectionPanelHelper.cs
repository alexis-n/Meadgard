using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanelHelper : MonoBehaviour
{
    #region ///  VARIABLES  ///
    public Tooltipable tooltipable;
    public InteractableObject interactableObject;
    public Image slotIcon;
    private Image background;
    #endregion

    private void Start()
    {
        tooltipable = GetComponent<Tooltipable>();
        slotIcon = GetComponent<Image>();
        background = GetComponent<Image>();
        UIManager.instance.selectionPanel.openingSelectionPanel.AddListener(Empty);
    }

    public void Initialize (InteractableObject newInteractableObject)
    {
        if (newInteractableObject != null)
        {
            interactableObject = newInteractableObject;
            if (newInteractableObject.data.icon) slotIcon.sprite = newInteractableObject.data.icon;
            background.color = newInteractableObject.data.color;
            tooltipable.nameTag = interactableObject.name;
        }
    }

    public void GiveContent()
    {
        if (interactableObject != null)
        {
            CameraManager.instance.ViewTarget(interactableObject.gameObject);
            if (interactableObject.selectable) SelectionManager.instance.SelectNewObject(interactableObject);
        }
    }

    public void Empty()
    {
        interactableObject = null;
        background.color = Color.grey;
    }
}
