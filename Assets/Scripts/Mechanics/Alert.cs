using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour {

    #region /// VARIABLES ///
    public enum AlertType
    { LevelUp, Bankrupt, Delivery, ThiefSuccess, ThiefFailed, ThiefKilled, EnnemyThief, Brawl};
    public AlertType type;

    public List<string> alertTimeList = new List<string>();
    public List<string> alertContentList = new List<string>();
    public int index;

    public GameObject token;
    public TextMeshProUGUI tokenCount;

    [SerializeField] [TextArea] [Header("'x1' et 'x2' seront remplacés")]
    private string originalContent;
    #endregion

    public void ClearContents()
    {
        alertTimeList.Clear();
        alertContentList.Clear();
        gameObject.SetActive(false);
    }

    public void GenerateContent(string firstContent, string secondContent = null)
    {
        string newContent = originalContent.Replace("x1", firstContent);
        if (secondContent != null) newContent = newContent.Replace("x2", secondContent);
        alertContentList.Add(newContent);
        AlertPanel.instance.QueueMainAlert(GetComponent<Image>().sprite, newContent);

        if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Service && PhaseManager.instance.timeLeft > 0)
        {
            string minutes = Mathf.Floor(PhaseManager.instance.timeLeft / 60).ToString("00");
            string seconds = (PhaseManager.instance.timeLeft % 60).ToString("00");
            string newTime = PhaseManager.instance.currentPhase.ToString() + " " + minutes + ":" + seconds;
            alertTimeList.Add(newTime);
        }
        else
        {
            string newPhase = PhaseManager.instance.currentPhase.ToString();
            alertTimeList.Add(newPhase);
        }

        if (alertTimeList.Count > 1)
        {
            token.SetActive(true);
            tokenCount.text = alertTimeList.Count.ToString();
        }
        else token.SetActive(false);

        if (index == alertTimeList.Count - 2) index = alertTimeList.Count - 1;
    }

    public void Next()
    {
        if (index == alertTimeList.Count - 1) index = 0;
        else index++;
    }

    public void Previous()
    {
        if (index == 0) index = alertTimeList.Count - 1;
        else index--;
    }

    #region /// VIEUX CODE ///
    /*
     #region /// VARIABLES ///
    [HideInInspector] public AlertPanel alertPanel;
    public enum Content { EmployeeLevelUP, PreparationPhaseStart, ServicePhaseStart, ServicePhaseFinish, RecapitulationPhaseStart, RestockingSuccesful};

    [TextArea]
    public string employeeLevelUp, preparationPhaseStart, servicePhaseStart, servicePhaseFinish, recapitulationPhaseStart, restockingSuccesful;

    private float alertHiddenPosX;
    [SerializeField] private float alertShownPosX;

    public RectTransform alert;
    public GameObject target, plus;
    public Image icon, gradient;
    public TextMeshProUGUI contentText;
    public float shownDuration = 5f, alertAnimationDuration = 0.2f;
    #endregion

    private void Start()
    {
        alert = GetComponent<RectTransform>();
    }

    //fonction qui génère le contenu d'une alerte
    public void GenerateContent(Content content, InteractableObject interactableObject = null)
    {
        alertHiddenPosX = alert.anchoredPosition.x;

        alertPanel.hideAllAlerts.AddListener(HideAlert);
        if (interactableObject != null) target = interactableObject.gameObject;
        plus.SetActive(false);

        switch (content)
        {
            case Content.EmployeeLevelUP:
                gradient.color = interactableObject.color;
                contentText.text = interactableObject.name + "\n" +employeeLevelUp;
                icon.sprite = interactableObject.sprite;
                plus.SetActive(true);
                break;

            case Content.PreparationPhaseStart:
                contentText.text = preparationPhaseStart;
                icon.sprite = UIManager.instance.bank.prepSprite;
                break;
            case Content.ServicePhaseStart:
                contentText.text = servicePhaseStart;
                icon.sprite = UIManager.instance.bank.serviceStartSprite;
                break;
            case Content.ServicePhaseFinish:
                contentText.text = servicePhaseFinish;
                icon.sprite = UIManager.instance.bank.serviceEndSprite;
                break;
            case Content.RecapitulationPhaseStart:
                contentText.text = recapitulationPhaseStart;
                icon.sprite = UIManager.instance.bank.recapSprite;
                break;

            case Content.RestockingSuccesful:
                contentText.text = restockingSuccesful;
                icon.sprite = UIManager.instance.bank.prepSprite;
                plus.SetActive(true);
                break;
        }
        
        if (!alertPanel.toggled) ShowAlert();
    }

    public void CameraTarget()
    {
        if (target != null)
        {
            CameraManager.instance.ViewTarget(target);
            SelectionManager.instance.SelectNewObject(target.GetComponent<InteractableObject>());

            if (target.GetComponent<Employee>())
            {
                UIManager.instance.menus[2].GetComponent<LevelUpMenu>().employee = target.GetComponent<Employee>();
                UIManager.instance.menus[2].gameObject.SetActive(true);
            }
        }
    }

    public void Autodestruct()
    {
        Destroy(this.gameObject);
    }

    public void ShowAlert()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(alert.DOAnchorPosX(alertShownPosX, alertAnimationDuration).SetEase(Ease.OutQuad))
            .AppendInterval(shownDuration)
            .AppendCallback(() => HideAlert());
    }

    public void HideAlert()
    {
        alert.DOAnchorPosX(alertHiddenPosX, alertAnimationDuration).SetEase(Ease.OutQuad);
        alertPanel.hideAllAlerts.RemoveListener(HideAlert);
    }
     */
    #endregion
}
