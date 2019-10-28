using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReadyButton : MonoBehaviour
{
    #region ///  VARIABLES  ///
    private RectTransform rect;
    [SerializeField] private Vector2 movementAmount;
    bool ready=false;
    public float animationDuration = 1f;
    #endregion

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    //fonction qui affiche les visuels quand le joueur est prêt, et en informe le GameManager
    public void LaunchService()
    {
        PhaseManager.instance.StartServicePhase();
    }

    public void Open()
    {
        rect.DOLocalRotate(new Vector3(0, 0, 0), animationDuration).SetEase(Ease.OutElastic);
    }

    public void Close()
    {
        rect.DOLocalRotate(new Vector3(0, 0, 180), animationDuration).SetEase(Ease.InQuad)
                .OnComplete(() => UIManager.instance.clientInfos.Open());
    }
}
