using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerStatsUI : MonoBehaviour
{
    #region /// VARIABLES ///
    [SerializeField] private float animationDuration = 1f;
    public Image fillImage = null, ghostImage = null, flickerImage;
    public TextMeshProUGUI statText, capacityText = null;
    private float addedGhostFillAmount;
    float ghostTimer;
    #endregion

    private void Start()
    {
        if (fillImage != null) fillImage.fillAmount = 0;
    }

    //fonction qui fait clignoter la stat en rouge
    public void StatFlickerRed()
    {
        var baseColor = flickerImage.color;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(flickerImage.DOColor(Color.red, 0.05f))
            .AppendInterval(0.05f)
            .Append(flickerImage.DOColor(baseColor, 0.05f))
            .AppendInterval(0.05f)
            .Append(flickerImage.DOColor(Color.red, 0.05f))
            .AppendInterval(0.05f)
            .Append(flickerImage.DOColor(baseColor, 0.05f))
            .AppendInterval(0.05f)
            .Append(flickerImage.DOColor(Color.red, 0.05f))
            .AppendInterval(0.05f)
            .Append(flickerImage.DOColor(baseColor, 0.05f));
    }

    public void GhostFiller(float newFillAmount)
    {
        if (newFillAmount < ghostImage.fillAmount)
        {
            RegularFiller(newFillAmount);
            return;
        }
        else
        {
            ghostImage.fillAmount = newFillAmount;
            fillImage.DOFillAmount(ghostImage.fillAmount, 0.5f).SetDelay(1f);
        }
    }

    public void RegularFiller(float newFillAmount)
    {
        fillImage.fillAmount = newFillAmount;
        ghostImage.DOFillAmount(fillImage.fillAmount, 0.5f).SetDelay(1f);
    }

    #region /// VIEUX CODE ///
    /*
     public Text moneyLeft;
    public Text foodLeft, foodStock;
    public Text drinksLeft, drinkStock;
    public Image money, food, drink, foodBackground, drinksBackground;

    public RectTransform foodVariation, drinkVariation, moneyVariation;
    int foodVariationAmount = 0, drinkVariationAmount = 0, moneyVariationAmount = 0;
    public Text foodVariationText, drinkVariationText, moneyVariationText;
    public float variationDuration = 1f, resourceVariationTimer, moneyVariationTimer;
    public float openedResourceVariationPosX, closedResourceVariationPosX, openedMoneyVariationPosY, closedMoneyVariationPosY;

    public RectTransform foodRestock, drinkRestock;
    bool buttonsHidden = false;
    public float restockButtonsDuration = 1f;
    public float openedRestockPosX, closedRestockPosX;

    public void OpenResourceVariation(Data.RessourceType ressourceType, int value)
    {
        switch (ressourceType)
        {
            case Data.RessourceType.Drink:
                drinkVariation.GetComponent<AudioSource>().Play();
                drinkVariationAmount += value;
                if (value > 0) drinkVariationText.text = ("+" + drinkVariationAmount);
                else drinkVariationText.text = drinkVariationAmount.ToString();
                drinkVariation.DOAnchorPos3DX(openedResourceVariationPosX, variationDuration).SetEase(Ease.OutBounce);
                break;
            case Data.RessourceType.Food:
                foodVariation.GetComponent<AudioSource>().Play();
                foodVariationAmount += value;
                if (value > 0) foodVariationText.text = ("+" + foodVariationAmount);
                else foodVariationText.text = foodVariationAmount.ToString();
                foodVariation.DOAnchorPos3DX(openedResourceVariationPosX, variationDuration).SetEase(Ease.OutBounce);
                break;
        }
        resourceVariationTimer += 3f;
        StartCoroutine(RessourceVariationTimer(ressourceType));
    }
    IEnumerator RessourceVariationTimer(Data.RessourceType ressourceType)
    {
        while (resourceVariationTimer > 0)
        {
            resourceVariationTimer -= Time.deltaTime;
            yield return null;
        }
        CloseResourceVariation(ressourceType);
    }
    public void CloseResourceVariation(Data.RessourceType ressourceType)
    {
        switch (ressourceType)
        {
            case Data.RessourceType.Drink:
                drinkVariation.DOAnchorPos3DX(closedResourceVariationPosX, variationDuration).SetEase(Ease.InBack);
                drinkVariationAmount = 0;
                break;
            case Data.RessourceType.Food:
                foodVariation.DOAnchorPos3DX(closedResourceVariationPosX, variationDuration).SetEase(Ease.InBack);
                foodVariationAmount = 0;
                break;
        }
    }
    public void OpenMoneyVariation(int value)
    {
        moneyVariation.GetComponent<AudioSource>().Play();
        moneyVariationAmount += value;
        if (value > 0) moneyVariationText.text = ("+" + moneyVariationAmount);
        else moneyVariationText.text = moneyVariationAmount.ToString();
        if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Prep) moneyVariation.DOAnchorPos3DY(openedMoneyVariationPosY - readyButtonOffset * 2, variationDuration).SetEase(Ease.OutBounce);
        else moneyVariation.DOAnchorPos3DY(openedMoneyVariationPosY, variationDuration).SetEase(Ease.OutBounce);
        moneyVariationTimer += 3f;
        StartCoroutine(MoneyVariationTimer());
    }
    IEnumerator MoneyVariationTimer()
    {
        while (moneyVariationTimer > 0)
        {
            moneyVariationTimer -= Time.deltaTime;
            yield return null;
        }
        CloseMoneyVariation();
    }
    public void CloseMoneyVariation()
    {
        moneyVariation.DOAnchorPos3DY(closedMoneyVariationPosY, variationDuration).SetEase(Ease.InBack);
        moneyVariationAmount = 0;
    }
    */
    #endregion
}
