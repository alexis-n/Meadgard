using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[System.Serializable]
public class ThiefValues
{
    public string thiefName;
    public int thiefHealth = 3, thiefMissionIndex;
    public float thiefMissionChance;
    public bool thiefInMission = false;
    public int[] thiefSkills = new int[3];
}

public class Thief : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static Thief draggedThief;
    public ThiefValues thiefValues = new ThiefValues();
    
    public bool locked = false;

    //public ThiefMission currentMission;
    public ThiefSlot rosterSlot, missionSlot;

    [SerializeField] private Image filler;
    public Image missionIcon;
    [SerializeField] private RectTransform barRect;
    [SerializeField] private Vector2 movementAmount;
    private Sprite originalSprite;
    [HideInInspector] public GameObject prop;

    private void Start()
    {
        name = NPCManager.instance.nameGenerator.Thief();
        originalSprite = GetComponent<Image>().sprite;
        prop = PlayerManager.instance.tavern.thiefProps[0];
        PlayerManager.instance.tavern.thiefProps.Remove(prop);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (locked) return;
        //quand on commence à glisser cet élément, on l'apparente à la souris 
        transform.SetParent(UIManager.instance.mouseFollower);
        UIManager.instance.cursorsBank.Drag();
        draggedThief = this;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        if (missionSlot != null)
        {
            missionSlot.slottedThief = null;
            missionSlot.mission.assignedThief = null;
            missionSlot.mission.NewThief();
            missionSlot = null;
        }
    }

    public void ThiefReset()
    {
        missionSlot.slottedThief = null;
        missionSlot.mission.assignedThief = null;
        missionSlot.mission.NewThief();
        missionSlot = null;
        transform.SetParent(rosterSlot.transform);
        transform.localPosition = Vector3.zero;
        if (thiefValues.thiefHealth <= 0)
        {
            gameObject.SetActive(false);
            missionIcon.gameObject.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        UIManager.instance.cursorsBank.EndDrag();
        //si on assigne un nouveau slot à cet élément
        if (missionSlot != null) FillSlot(missionSlot);
        //sinon on retourne à la position de base
        else transform.SetParent(rosterSlot.transform);
        transform.localPosition = Vector3.zero;
    }

    public void FillSlot(ThiefSlot givenSlot)
    {
        transform.SetParent(givenSlot.transform);
        transform.SetAsFirstSibling();
        transform.localPosition = Vector3.zero;
        missionSlot = givenSlot;
    }

    public void ShowUIBar()
    {
        barRect.DOAnchorPos(new Vector2(barRect.anchoredPosition.x + movementAmount.x, barRect.anchoredPosition.y + movementAmount.y), 0.2f);
        filler.DOFillAmount(1, ThiefMenu.instance.missions[thiefValues.thiefMissionIndex].baseTime).SetEase(Ease.Linear);
    }

    public void Flicker(bool missionSucess)
    {
        var baseColor = filler.color;
        Color newColor;
        if (missionSucess) newColor = Color.green;
        else
        {
            if (thiefValues.thiefHealth <= 0) newColor = Color.red;
            else newColor = Color.yellow;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.Append(filler.DOColor(newColor, 0.05f))
            .AppendInterval(0.05f)
            .Append(filler.DOColor(baseColor, 0.05f))
            .AppendInterval(0.05f)
            .Append(filler.DOColor(newColor, 0.05f))
            .AppendInterval(0.05f)
            .Append(filler.DOColor(baseColor, 0.05f))
            .AppendInterval(0.05f)
            .Append(filler.DOColor(newColor, 0.05f))
            .AppendInterval(0.05f)
            .Append(filler.DOColor(baseColor, 0.05f))
            .OnComplete(() => HideUIBar());
    }

    public void HideUIBar(bool reset = true)
    {
        barRect.DOAnchorPos(new Vector2(barRect.anchoredPosition.x - movementAmount.x, barRect.anchoredPosition.y - movementAmount.y), 0.2f)
            .OnComplete(() => filler.fillAmount = 0);

        if (reset)
        {
            missionIcon.sprite = originalSprite;
            ThiefReset();
        }
    }
}
