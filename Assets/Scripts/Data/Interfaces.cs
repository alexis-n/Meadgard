using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Interface qui permet de sélectionner un objet et de le déselectionner,
//et d'indiquer au joueur qu'il peut être sélectionner dans certaines situations
public interface ISelection
{
    void Select();
    void Unselect();
    void InteractaVisual();
}*/

    //Interface qui permet d'ouvrir, plier, déplacer et fermer une fenêtre
    public interface IMenu
{
    void OpenAndClose();
    void FoldAndUnfold();
    void OnDrag();
}
