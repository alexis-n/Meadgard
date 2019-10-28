using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class ClientInfos : MonoBehaviour
{
    #region ///  VARIABLES  ///
    private RectTransform rect;
    [SerializeField] private Vector2 movementAmount;
    bool toggled = false;
    public float animationDuration = 1f;

    [Header("Ordre: Commande, Poulet, Poisson, Sanglier, Pinte, Bouteille et Tonnelet")]

    [SerializeField] private GameObject[] bubbles;
    [SerializeField] private TextMeshProUGUI[] texts;

    private int orderAmount, chickenAmount, fishAmount, boarAmount, pinteAmount, bottleAmount, kegAmount;
    #endregion

    private void Start()
    {
        ResetClientInfos();
        rect = GetComponent<RectTransform>();
    }

    public void Open()
    {
        rect.DOLocalRotate(new Vector3(0, 0, 0), animationDuration).SetEase(Ease.OutElastic);
    }

    public void Close()
    {
        rect.DOLocalRotate(new Vector3(0, 0, 180), animationDuration).SetEase(Ease.InBack)
            .OnComplete(() => UIManager.instance.readyButton.Open());
    }

    //fonction qui réinitialise les infobulles
    public void ResetClientInfos()
    {
        for (int i = 0; i < bubbles.Length; i++)
        {
            bubbles[i].SetActive(false);
        }

        orderAmount = 0;
        chickenAmount = 0;
        fishAmount = 0;
        boarAmount = 0;
        pinteAmount = 0;
        bottleAmount = 0;
        kegAmount = 0;
    }

    //fonction qui update les infobulles de plats
    public void UpdateDishInfos(Order order, bool add = true)
    {
        switch (order.ressourceType)
        {
            case Data.RessourceType.Food:
                switch (order.ressourceAmount)
                {
                    case 1:
                        if (add) chickenAmount++;
                        else chickenAmount--;

                        if (chickenAmount <= 0) bubbles[1].SetActive(false);
                        else bubbles[1].SetActive(true);

                        texts[1].text = chickenAmount.ToString();
                        return;
                    case 2:
                        if (add) fishAmount++;
                        else fishAmount--;

                        if (fishAmount <= 0) bubbles[2].SetActive(false);
                        else bubbles[2].SetActive(true);

                        texts[2].text = fishAmount.ToString();
                        return;
                    case 3:
                        if (add) boarAmount++;
                        else boarAmount--;

                        if (boarAmount <= 0) bubbles[3].SetActive(false);
                        else bubbles[3].SetActive(true);

                        texts[3].text = boarAmount.ToString();
                        return;
                }
                break;

            case Data.RessourceType.Drink:
                switch (order.ressourceAmount)
                {
                    case 1:
                        if (add) pinteAmount++;
                        else pinteAmount--;

                        if (pinteAmount <= 0) bubbles[4].SetActive(false);
                        else bubbles[4].SetActive(true);

                        texts[4].text = pinteAmount.ToString();
                        return;
                    case 2:
                        if (add) bottleAmount++;
                        else bottleAmount--;

                        if (bottleAmount <= 0) bubbles[5].SetActive(false);
                        else bubbles[5].SetActive(true);

                        texts[5].text = bottleAmount.ToString();
                        return;
                    case 3:
                        if (add) kegAmount++;
                        else kegAmount--;

                        if (kegAmount <= 0) bubbles[6].SetActive(false);
                        else bubbles[6].SetActive(true);

                        texts[6].text = kegAmount.ToString();
                        return;
                }
                break;
        }
    }

    //fonction qui update l'infobulle de commandes
    public void UpdateOrderInfos(bool add = true)
    {
        if (add) orderAmount++;
        else orderAmount--;

        if (orderAmount == 0) bubbles[0].SetActive(false);
        else bubbles[0].SetActive(true);

        texts[0].text = orderAmount.ToString();
    }
}
