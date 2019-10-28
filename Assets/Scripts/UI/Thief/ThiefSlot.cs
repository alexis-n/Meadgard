using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThiefSlot : MonoBehaviour, IDropHandler
{
    public Thief slottedThief;
    public ThiefMission mission;

    private void Start()
    {
        if (slottedThief != null) slottedThief.rosterSlot = this;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (mission != null)
        {
            if (PhaseManager.instance.timeLeft <= 0 || mission.thiefLocked) return;
            //si on veux échanger deux éléments
            if (slottedThief != null)
            {
                slottedThief.FillSlot(slottedThief.rosterSlot);
                Thief.draggedThief.missionSlot.slottedThief = slottedThief;
            }
            slottedThief = Thief.draggedThief;
            slottedThief.missionSlot = this;
            mission.NewThief(slottedThief);
        }
    }
}
