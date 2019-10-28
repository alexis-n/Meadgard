using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class RecapPanel : MonoBehaviour
{
    #region /// VARIABLES ///
    [SerializeField]
    private float animationDelays;

    [SerializeField]
    private GameObject employeesPanel, managementPanel, clientsPanel, grandTotalObject, nextDayButton;

    [SerializeField]
    private TextMeshProUGUI dayText, missiveText,
        recruitmentText, salaryText, thiefText, employeesTotalText,
        furnitureText, restockText, caughtThievesText, managementTotalText,
        clientsServedText, clientsTipsText, clientsKickText, clientsTotalText,
        grandTotalText, markText, favorsText;

    [HideInInspector]
    public int recruitmentValue = 0, salaryValue = 0, thiefValue = 0, employeesTotalValue = 0,
        furnitureValue = 0, restockValue = 0, caughtThievesValue = 0, managementTotalValue = 0,
        clientsServedValue = 0, clientsTipsValue = 0, clientsKickValue = 0, clientsTotalValue = 0,
        grandTotalValue = 0, markValue = 0, favorsValue = 0;

    [SerializeField]
    private Color negativeColor, positiveColor;

    [SerializeField]
    private int Fthreshold = -100, Ethreshold = -50, Dthreshold = 0, Cthreshold = 50, Bthreshold = 100, Athreshold = 150,
        Ffavors = 0, Efavors = 5, Dfavors = 10, Cfavors = 15, Bfavors = 20, Afavors = 25, Sfavors = 30;

    [HideInInspector]
    public int favors = 0;
    #endregion

    private void Start()
    {
        PhaseManager.instance.prepPhaseEvent.AddListener(Reset);
    }

    public void Reset()
    {
        favors = 0;

        recruitmentValue = 0;
        salaryValue = 0;
        thiefValue = 0;
        employeesTotalValue = 0;

        furnitureValue = 0;
        restockValue = 0;
        caughtThievesValue = 0;
        managementTotalValue = 0;

        clientsServedValue = 0;
        clientsServedValue = 0;
        clientsTipsValue = 0;
        clientsTotalValue = 0;

        grandTotalValue = 0;
        markValue = 0;
        favorsValue = 0;
    }

    public void StartRecap()
    {
        NPCManager.instance.purge.Invoke();

        employeesPanel.SetActive(false);
        managementPanel.SetActive(false);
        clientsPanel.SetActive(false);
        nextDayButton.SetActive(true);
        grandTotalObject.SetActive(false);

        dayText.text = "Jour " + PhaseManager.instance.turn + ":";
        missiveText.text = Missive.currentMissive.missiveName;

        recruitmentText.text = recruitmentValue.ToString();
        salaryText.text = salaryValue.ToString();
        thiefText.text = thiefValue.ToString();
        employeesTotalValue = recruitmentValue + salaryValue + thiefValue;
        employeesTotalText.text = employeesTotalValue.ToString();

        furnitureText.text = furnitureValue.ToString();
        restockText.text = restockValue.ToString();
        caughtThievesText.text = caughtThievesValue.ToString();
        managementTotalValue = furnitureValue + restockValue;
        managementTotalText.text = managementTotalValue.ToString();

        clientsServedText.text = clientsServedValue.ToString();
        clientsTipsText.text = clientsTipsValue.ToString();
        clientsKickText.text = clientsKickValue.ToString();
        clientsTotalText.text = "+" + clientsTotalValue;

        var grandTotal = employeesTotalValue + managementTotalValue + clientsTotalValue;
        grandTotalText.text = grandTotal.ToString();
        if (grandTotal > 0) grandTotalText.color = positiveColor;
        else grandTotalText.color = negativeColor;

        if (grandTotal <= Fthreshold)
        {
            markText.text = "F";
            favors = Ffavors;
        }
        else if (Fthreshold < grandTotal && grandTotal <= Ethreshold)
        {
            markText.text = "E";
            favors = Efavors;
        }
        else if (Ethreshold < grandTotal && grandTotal <= Dthreshold)
        {
            markText.text = "D";
            favors = Dfavors;
        }
        else if (Dthreshold < grandTotal && grandTotal <= Cthreshold)
        {
            markText.text = "C";
            favors = Cfavors;
        }
        else if (Cthreshold < grandTotal && grandTotal <= Bthreshold)
        {
            markText.text = "B";
            favors = Bfavors;
        }
        else if (Bthreshold < grandTotal && grandTotal <= Athreshold)
        {
            markText.text = "A";
            favors = Afavors;
        }
        else if (grandTotal > Athreshold)
        {
            markText.text = "S";
            favors = Sfavors;
        }

        favors = favors * (1 + (PhaseManager.instance.turn / 10));
        favorsText.text = "+" + favors;

        RecapAnimation();
    }

    public void RecapAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(GetComponent<CanvasGroup>().DOFade(1, animationDelays))
            .AppendInterval(animationDelays)
            .AppendCallback(() => employeesPanel.SetActive(true))
            .AppendInterval(animationDelays)
            .AppendCallback(() => managementPanel.SetActive(true))
            .AppendInterval(animationDelays)
            .AppendCallback(() => clientsPanel.SetActive(true))
            .AppendInterval(animationDelays * 2)
            .AppendCallback(() => grandTotalObject.SetActive(true));
    }

    public void NextDay()
    {
        GameSaver.instance.SaveGame();
        GetComponent<CanvasGroup>().DOFade(0, animationDelays).OnComplete(() => PhaseManager.instance.StartPreparationPhase());
    }
}
