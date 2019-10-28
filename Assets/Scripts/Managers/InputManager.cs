using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [SerializeField] private float holdingTimer = 1f;
    private float currentHoldingTimer = 0;
    private bool isDraggingObject;

    // Use this for initialization
    private void Awake () {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.instance.paused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //On regarde si l'objet UI que l'on touche est considéré comme transparent
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                bool allUIElementsTransparent = true;
                for (int i = 0; i < raycastResults.Count; i++)
                {
                    if (raycastResults[i].gameObject.tag != "Transparent")
                    {
                        allUIElementsTransparent = false;
                        break;
                    }
                }

                //si on ne clique par sur de l'UI ou que tout les UI touchés sont transparents...
                if (!EventSystem.current.IsPointerOverGameObject() || allUIElementsTransparent)
                {
                    RaycastHit hitInfo = new RaycastHit();

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)
                        && hitInfo.transform.gameObject != null
                        && hitInfo.transform.gameObject.GetComponent<InteractableObject>()
                        && hitInfo.transform.gameObject.GetComponent<InteractableObject>().selectable)
                    {
                        if (hitInfo.transform.gameObject.GetComponent<InteractableObject>() != SelectionManager.instance.selectedObject)
                            SelectionManager.instance.SelectNewObject(hitInfo.transform.gameObject.GetComponent<InteractableObject>());
                    }
                    else if (SelectionManager.instance.selectedObject != null)
                    {
                        SelectionManager.instance.UnselectObject();
                    }
                }
            }
            if (Input.GetMouseButton(0))
            {
                if (!isDraggingObject)
                {
                    //On regarde si l'objet UI que l'on touche est considéré comme transparent
                    PointerEventData pointer = new PointerEventData(EventSystem.current);
                    pointer.position = Input.mousePosition;

                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointer, raycastResults);

                    bool allUIElementsTransparent = true;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.tag != "Transparent")
                        {
                            allUIElementsTransparent = false;
                            break;
                        }
                    }

                    //si on ne clique par sur de l'UI ou que tout les UI touchés sont transparents...
                    if (!EventSystem.current.IsPointerOverGameObject() || allUIElementsTransparent)
                    {
                        RaycastHit hitInfo = new RaycastHit();

                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)
                            && hitInfo.transform.gameObject != null
                            && hitInfo.transform.gameObject.GetComponent<InteractableObject>()
                            && hitInfo.transform.gameObject.GetComponent<InteractableObject>().draggable)
                        {
                            if (hitInfo.transform.gameObject.GetComponent<InteractableObject>() == SelectionManager.instance.selectedObject)
                            {
                                currentHoldingTimer += Time.deltaTime;
                                MouseFollower.instance.mouseHoldFiller.fillAmount = currentHoldingTimer / holdingTimer;
                                if (currentHoldingTimer > holdingTimer)
                                {
                                    UIManager.instance.cursorsBank.Drag();
                                    SelectionManager.instance.DragObject();
                                    isDraggingObject = true;
                                    MouseFollower.instance.mouseHoldFiller.fillAmount = 0;
                                }
                            }
                            else MouseFollower.instance.mouseHoldFiller.fillAmount = 0;
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                MouseFollower.instance.mouseHoldFiller.fillAmount = 0;
                currentHoldingTimer = 0;
                if (isDraggingObject)
                {
                    UIManager.instance.cursorsBank.EndDrag();
                    isDraggingObject = false;

                    //On regarde si l'objet UI que l'on touche est considéré comme transparent
                    PointerEventData pointer = new PointerEventData(EventSystem.current);
                    pointer.position = Input.mousePosition;

                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointer, raycastResults);

                    bool allUIElementsTransparent = true;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.tag != "Transparent")
                        {
                            allUIElementsTransparent = false;
                            break;
                        }
                    }

                    //si on ne clique par sur de l'UI ou que tout les UI touchés sont transparents...
                    if (!EventSystem.current.IsPointerOverGameObject() || allUIElementsTransparent)
                    {
                        RaycastHit hitInfo = new RaycastHit();

                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)
                            && hitInfo.transform.gameObject != null
                            && hitInfo.transform.gameObject.GetComponent<InteractableObject>()
                            && hitInfo.transform.gameObject.GetComponent<InteractableObject>().interactable)
                        {
                            SelectionManager.instance.DropObject(hitInfo.transform.gameObject.GetComponent<InteractableObject>());
                        }
                        else SelectionManager.instance.DropObject();
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    RaycastHit hitInfo = new RaycastHit();

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)
                        && hitInfo.transform.gameObject != null
                        && hitInfo.transform.gameObject.GetComponent<InteractableObject>()
                        && hitInfo.transform.gameObject.GetComponent<InteractableObject>().interactable)
                    {
                        SelectionManager.instance.InteractWith(hitInfo.transform.gameObject.GetComponent<InteractableObject>());
                    }
                }
            }

            if (Input.GetKey(KeyCode.Space) && IngameDebug.instance.debugMode)
            {
                Time.timeScale = 5;
            }
            else Time.timeScale = 1;

            if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.B))
            {
                IngameDebug.instance.DebugMode();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && PauseMenu.instance.controlsEnabled)
        {
            if (UIManager.instance.currentMenu != null) UIManager.instance.CloseCurrentMenu();
            else
            {
                if (!PauseMenu.instance.content.gameObject.activeSelf)
                    PauseMenu.instance.PauseGame();
                else PauseMenu.instance.UnpauseGame();
            }

        }
    }
}
