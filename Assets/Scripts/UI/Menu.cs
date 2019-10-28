using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public bool foldToggle = false;
    [SerializeField] private GameObject body;
    public GameObject content;
    public Vector2 defaultPosition;

    protected void Update()
    {
        if (content.activeSelf) UpdateContent();
    }

    public virtual void OnOpening()
    {
        GetComponent<RectTransform>().anchoredPosition = defaultPosition;

        if (!foldToggle) FoldAndUnfold();

        UIManager.instance.currentMenu = this;
    }

    //Fonction qui met à jour le contenu d'un menu
    public virtual void UpdateContent() { }

    //Fonction qui plie ou déplie le menu
    public void FoldAndUnfold()
    {
        foldToggle = !foldToggle;
        body.SetActive(foldToggle);
    }

    //fonction qui permet de glisser le menu sur l'écran
    public void OnDrag()
    {

    }
}
