using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ThiefMission : MonoBehaviour
{
    #region /// VARIABLES ///
    public Thief assignedThief;
    public Sprite missionSprite;
    private enum ThiefSkill { Abilities, Eloquence, Surveillance};
    [HideInInspector] public Tween barFillerTween;
    [SerializeField] private ThiefSkill skillType;
    [SerializeField] protected string alertText;
    [SerializeField] private TextMeshProUGUI timeText, budgetText, chanceText;
    public TextMeshProUGUI countdownText;
    [SerializeField] protected Button confirmButton;
    [SerializeField] private Image timerFiller;
    [SerializeField] protected GameObject budget, timer;
    [SerializeField] private Sprite confirmSprite, lockedSprite;
    [SerializeField] private Slider budgetSlider;
    [SerializeField] private int basePrice = 20, baseChance = 0, chancePerSkill = 5, prepChance = 10;
    [SerializeField] private int maxChance = 150;
    [HideInInspector] public bool countdown = false, thiefLocked = false;
    [SerializeField] private float chancePerMoney = 1;
    public float baseTime = 60;
    [HideInInspector] public float timeLeft = 0;
    protected int defChance = 0;
    #endregion

    private void Start()
    {
        NewThief();
        budgetSlider.minValue = basePrice;
        budgetText.text = basePrice.ToString();
        PhaseManager.instance.serviceEndEvent.AddListener(CleanSlot);
    }

    private void Update()
    {
        if (countdown)
        {
            string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
            string seconds = (timeLeft % 60).ToString("00");
            timeText.text = minutes + ":" + seconds;
        }
    }

    //fonction qui calcule la chance et la valeur max du budget selon le voleur alloué
    public void NewThief(Thief newThief = null)
    {
        if(baseTime <= 0) timeText.text = "Tour";
        else
        {
            string minutes = Mathf.Floor(baseTime / 60).ToString("00");
            string seconds = (baseTime % 60).ToString("00");
            timeText.text = minutes + ":" + seconds;
        }

        budgetSlider.value = 0;

        if (newThief == null)
        {
            budgetText.text = 0.ToString();
            chanceText.text = baseChance + "%";
            confirmButton.gameObject.SetActive(false);
            budgetSlider.interactable = false;
        }
        else
        {
            newThief.thiefValues.thiefMissionIndex = System.Array.IndexOf(ThiefMenu.instance.missions, this);
            budgetSlider.interactable = true;
            confirmButton.gameObject.SetActive(true);
            confirmButton.image.sprite = confirmSprite;
            assignedThief = newThief;
            switch (skillType)
            {
                case ThiefSkill.Abilities:
                    budgetSlider.maxValue = (maxChance - (baseChance + chancePerSkill * assignedThief.thiefValues.thiefSkills[0])) / chancePerMoney;
                    break;
                case ThiefSkill.Eloquence:
                    budgetSlider.maxValue = (maxChance - (baseChance + chancePerSkill * assignedThief.thiefValues.thiefSkills[1])) / chancePerMoney;
                    break;
                case ThiefSkill.Surveillance:
                    budgetSlider.maxValue = (maxChance - (baseChance + chancePerSkill * assignedThief.thiefValues.thiefSkills[2])) / chancePerMoney;
                    break;
            }
            if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Prep) budgetSlider.maxValue -= prepChance;
            CalculateChance();
        }
    }

    //fonction qui la chance de réussite selon le budget alloué et les compétences du voleur
    public void CalculateChance()
    {
        budgetText.text = ((int)(budgetSlider.value * chancePerMoney)).ToString();

        if(assignedThief != null)
        {
            switch (skillType)
            {
                case ThiefSkill.Abilities:
                    defChance = (int)((budgetSlider.value * chancePerMoney) + baseChance + chancePerSkill * assignedThief.thiefValues.thiefSkills[0]);
                    break;
                case ThiefSkill.Eloquence:
                    defChance = (int)((budgetSlider.value * chancePerMoney) + baseChance + chancePerSkill * assignedThief.thiefValues.thiefSkills[1]);
                    break;
                case ThiefSkill.Surveillance:
                    defChance = (int)((budgetSlider.value * chancePerMoney) + baseChance + chancePerSkill * assignedThief.thiefValues.thiefSkills[2]);
                    break;
            }
        }

        if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Prep) defChance += prepChance;
        chanceText.text = defChance + "%";
    }

    //fonction qui lance la mission
    public virtual void StartMission()
    {
        countdownText.gameObject.SetActive(false);
        if (baseTime != 0)
        {
            barFillerTween = timerFiller.DOFillAmount(1, baseTime).SetEase(Ease.Linear);
            timeLeft = baseTime;
            countdown = true;
        }  
        else
        {
            timerFiller.fillAmount = 1;
            countdown = false;
        }

        assignedThief.ShowUIBar();
    }

    public virtual void PrepareMission()
    {
        if (PlayerManager.instance.PlayerMoney >= budgetSlider.value)
        {
            PlayerManager.instance.PlayerMoney -= (int)budgetSlider.value;
            PlayerManager.instance.thiefMoney -= (int)budgetSlider.value;
            UIManager.instance.blackPanel.recapPanel.recruitmentValue -= (int)budgetSlider.value;

            thiefLocked = true;
            assignedThief.missionIcon.sprite = missionSprite;
            budget.SetActive(false);
            timer.SetActive(true);
            confirmButton.interactable = false;
            confirmButton.image.sprite = lockedSprite;
            assignedThief.locked = true;
            timerFiller.fillAmount = 0;

            assignedThief.thiefValues.thiefInMission = true;
            assignedThief.thiefValues.thiefMissionChance = defChance;

            if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Service) StartMission();
            else
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = "Se prépare...";
            }

            UIManager.instance.newsboardMenu.hasSentThief = true;
        }
        else UIManager.instance.playerMoney.StatFlickerRed();
    }

    //fonction virtuelle qui sera overridée par le résultat de la mission
    public virtual void EndMission(bool autoFailure = false)
    {
        thiefLocked = false;

        if (autoFailure) Failure();
        else
        {
            //on fait un lancer de dé entre 0 et la chance maximale
            var diceRoll = Random.Range(0, 100);

            //si c'est un succés...
            if (diceRoll <= assignedThief.thiefValues.thiefMissionChance)
            {
                //si c'est un succés critique...
                if (assignedThief.thiefValues.thiefMissionChance > 100)
                {
                    var criticalDiceRoll = Random.Range(0, maxChance - 100);
                    if (criticalDiceRoll <= (defChance - 100)) Success(true);
                    else Success(false);
                }
                else Success(false);
            }
            else Failure();
        }

        assignedThief.thiefValues.thiefInMission = false;
        assignedThief.thiefValues.thiefMissionChance = 0;

        budget.SetActive(true);
        timer.SetActive(false);
        confirmButton.interactable = true;
        confirmButton.gameObject.SetActive(false);
    }

    public virtual void Success(bool critical)
    {
        AlertPanel.instance.GenerateAlert(Alert.AlertType.ThiefSuccess, assignedThief.thiefValues.thiefName, alertText);
        switch (skillType)
        {
            case ThiefSkill.Abilities:
                if (assignedThief.thiefValues.thiefSkills[0] < 5) assignedThief.thiefValues.thiefSkills[0]++;
                break;
            case ThiefSkill.Eloquence:
                if (assignedThief.thiefValues.thiefSkills[1] < 5) assignedThief.thiefValues.thiefSkills[1]++;
                break;
            case ThiefSkill.Surveillance:
                if (assignedThief.thiefValues.thiefSkills[2] < 5) assignedThief.thiefValues.thiefSkills[2]++;
                break;
        }
        assignedThief.Flicker(true);
        assignedThief.locked = false;
    }

    public virtual void Failure()
    {
        assignedThief.thiefValues.thiefHealth--;
        assignedThief.Flicker(false);
        if (assignedThief.thiefValues.thiefHealth <= 0) AlertPanel.instance.GenerateAlert(Alert.AlertType.ThiefKilled, assignedThief.thiefValues.thiefName, alertText);
        else
        {
            AlertPanel.instance.GenerateAlert(Alert.AlertType.ThiefFailed, assignedThief.name, alertText);
            assignedThief.locked = false;
        }
    }

    public void CleanSlot()
    {
        if (assignedThief != null && !thiefLocked) assignedThief.ThiefReset();
    }
}
