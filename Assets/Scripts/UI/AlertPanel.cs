using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class AlertPanel : MonoBehaviour
{
    public static AlertPanel instance;

    #region /// VARIABLES ///
    private RectTransform rectTransform;
    [SerializeField]
    private Alert levelUpAlert, bankruptAlert, deliveryAlert,
        thiefFailedAlert, thiefSuccessAlert, thiefKilledAlert, ennemyThiefAlert, brawlAlert;
    private List<Alert> allAlerts = new List<Alert>();

    public float positionAmount = 80f;

    [SerializeField] private Image mainAlertImage;
    [SerializeField] private TextMeshProUGUI mainAlertText;
    [SerializeField] private CanvasGroup mainAlertCanvasGroup;
    [SerializeField] private RectTransform mainAlertRect;
    private bool isDisplayingMainAlert = false;
    private List<Sprite> mainAlertSpriteQueue = new List<Sprite>();
    private List<string> mainAlertStringQueue = new List<string>();
    private float mainAlertBasePosition;

    private Vector2 baseAnchor;
    #endregion

    private void Awake()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        baseAnchor = rectTransform.anchoredPosition;

        allAlerts.Add(levelUpAlert);
        allAlerts.Add(bankruptAlert);
        allAlerts.Add(deliveryAlert);
        allAlerts.Add(thiefFailedAlert);
        allAlerts.Add(thiefKilledAlert);
        allAlerts.Add(thiefSuccessAlert);
        allAlerts.Add(ennemyThiefAlert);
        allAlerts.Add(brawlAlert);

        mainAlertBasePosition = mainAlertRect.anchoredPosition.y;
    }

    public void GenerateAlert(Alert.AlertType alertType, string firstContent, string secondContent = null)
    {
        switch(alertType)
        {
            case Alert.AlertType.LevelUp:
                if (levelUpAlert.alertContentList.Count <= 0) levelUpAlert.gameObject.SetActive(true);
                levelUpAlert.GenerateContent(firstContent, secondContent);
                levelUpAlert.transform.SetAsFirstSibling();
                break;
            case Alert.AlertType.Bankrupt:
                if (bankruptAlert.alertContentList.Count <= 0) bankruptAlert.gameObject.SetActive(true);
                bankruptAlert.GenerateContent(firstContent, secondContent);
                bankruptAlert.transform.SetAsFirstSibling();
                break;
            case Alert.AlertType.Delivery:
                if (deliveryAlert.alertContentList.Count <= 0) deliveryAlert.gameObject.SetActive(true);
                deliveryAlert.GenerateContent(firstContent, secondContent);
                deliveryAlert.transform.SetAsFirstSibling();
                break;
            case Alert.AlertType.ThiefSuccess:
                if (thiefSuccessAlert.alertContentList.Count <= 0) thiefSuccessAlert.gameObject.SetActive(true);
                thiefSuccessAlert.GenerateContent(firstContent, secondContent);
                thiefSuccessAlert.transform.SetAsFirstSibling();
                break;
            case Alert.AlertType.ThiefFailed:
                if (thiefFailedAlert.alertContentList.Count <= 0) thiefFailedAlert.gameObject.SetActive(true);
                thiefFailedAlert.GenerateContent(firstContent, secondContent);
                thiefFailedAlert.transform.SetAsFirstSibling();
                break;
            case Alert.AlertType.ThiefKilled:
                if (thiefKilledAlert.alertContentList.Count <= 0) thiefKilledAlert.gameObject.SetActive(true);
                thiefKilledAlert.GenerateContent(firstContent, secondContent);
                thiefKilledAlert.transform.SetAsFirstSibling();
                break;
            case Alert.AlertType.EnnemyThief:
                if (ennemyThiefAlert.alertContentList.Count <= 0) ennemyThiefAlert.gameObject.SetActive(true);
                ennemyThiefAlert.GenerateContent(firstContent);
                ennemyThiefAlert.transform.SetAsFirstSibling();
                break;
            case Alert.AlertType.Brawl:
                if (brawlAlert.alertContentList.Count <= 0) brawlAlert.gameObject.SetActive(true);
                brawlAlert.GenerateContent(firstContent, secondContent);
                brawlAlert.transform.SetAsFirstSibling();
                break;
        }
        ChangePosition();
    }

    public void QueueMainAlert(Sprite sprite = null, string text = null)
    {
        if (sprite != null || text != null)
        {
            mainAlertSpriteQueue.Add(sprite);
            mainAlertStringQueue.Add(text);
        }

        if (mainAlertSpriteQueue.Count != 0 && mainAlertStringQueue.Count != 0 && !isDisplayingMainAlert)
        {
            ShowMainAlert(mainAlertSpriteQueue[0], mainAlertStringQueue[0]);
            mainAlertSpriteQueue.Remove(mainAlertSpriteQueue[0]);
            mainAlertStringQueue.Remove(mainAlertStringQueue[0]);
        }
    }

    private void ShowMainAlert(Sprite sprite, string text)
    {
        isDisplayingMainAlert = true;
        mainAlertImage.sprite = sprite;
        mainAlertText.text = text;
        mainAlertCanvasGroup.alpha = 0;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(mainAlertRect.DOAnchorPos(new Vector2(mainAlertRect.anchoredPosition.x, mainAlertBasePosition - 40), 0.2f))
            .Join(mainAlertCanvasGroup.DOFade(1, 0.2f))
            .AppendInterval(3f)
            .Append(mainAlertRect.DOAnchorPos(new Vector2(mainAlertRect.anchoredPosition.x, mainAlertBasePosition), 0.2f).OnComplete(() => isDisplayingMainAlert = false))
            .Join(mainAlertCanvasGroup.DOFade(0, 0.2f).OnComplete(() => QueueMainAlert()));
    }

    public void ClearAlerts()
    {
        for (int i = 0; i < allAlerts.Count; i++)
        {
            allAlerts[i].ClearContents();
            allAlerts[i].gameObject.SetActive(false);
        }
        ChangePosition();
    }

    public void ChangePosition()
    {
        float moveTo = baseAnchor.y;
        for (int i = 0; i < allAlerts.Count; i++)
        {
            if (allAlerts[i].gameObject.activeSelf)
                moveTo -= positionAmount;
        }

        rectTransform.DOAnchorPos(new Vector2(rectTransform.anchoredPosition.x, moveTo), 0.5f);
    }


    #region /// VIEUX CODE ///
    /*
    #region /// VARIABLES ///
    public UnityEvent hideAllAlerts = new UnityEvent();

    [SerializeField] private RectTransform alertPanel;
    [SerializeField] private GameObject alertPrefab, alertParent;
    public bool toggled = false;

    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private Vector2 alertPanelClosedPosition;
    private Vector2 alertPanelOpenedPosition;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        alertPanelOpenedPosition = alertPanel.anchoredPosition;
        alertPanel.anchoredPosition = alertPanelClosedPosition;
    }

    //fonction qui ouvre ou ferme le panneau d'alertes
    public void Toggle()
    {
        //ouverture du panneau
        if (!toggled)
        {
            alertPanel.DOAnchorPos(alertPanelOpenedPosition, animationDuration);
                        hideAllAlerts.Invoke();
        }

        //fermeture du panneau
        else
        {
            alertPanel.DOAnchorPos(alertPanelClosedPosition, animationDuration);
        }

        toggled = !toggled;
    }

    //fonction qui génère une alerte et la range dans le panel
    public void GenerateAlert(Alert.Content content, InteractableObject interactableObject = null)
    {
        var alert = Instantiate(alertPrefab, alertParent.transform).GetComponent<Alert>();
        alert.alertPanel = this;
        alert.GenerateContent(content, interactableObject);
    }
    public void GenerateAlert()
    {
        var alert = Instantiate(alertPrefab, alertParent.transform).GetComponent<Alert>();
        alert.alertPanel = this;
        alert.GenerateContent(Alert.Content.PreparationPhaseStart);
    }
    */
    #endregion
}
