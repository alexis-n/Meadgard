using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Prediction : ThiefMission
{
    [SerializeField]
    private GameObject missive;

    private void Start()
    {
        PhaseManager.instance.recapPhaseEvent.AddListener(HideMissive);
        HideMissive();
    }

    public override void PrepareMission()
    {
        assignedThief.prop.SetActive(true);
        NPCManager.instance.GenerateSmokeBomb(assignedThief.prop.transform.position);
        base.PrepareMission();
    }

    public void HideMissive()
    {
        missive.SetActive(false);
    }

    public override void Success(bool critical)
    {
        if (Missive.predictedMissive == null)
        {
            Missive.predictedMissive = PhaseManager.instance.missives[Random.Range(0, PhaseManager.instance.missives.Length)];
            missive.SetActive(true);
        }
        base.Success(critical);
    }

    public override void EndMission(bool autoFailure = false)
    {
        assignedThief.prop.SetActive(false);
        NPCManager.instance.GenerateSmokeBomb(assignedThief.prop.transform.position);
        base.EndMission(autoFailure);
    }
}
