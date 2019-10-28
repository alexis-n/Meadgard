using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counterspy : ThiefMission
{
    public float timeBeforeChecking = 10f;
    IEnumerator counterspyCoroutine;
    private int caughtThieves = 0;

    [SerializeField] private Sprite banditSpottedSprite;
    [SerializeField] private string banditSpottedText;

    public override void PrepareMission()
    {
        assignedThief.prop.SetActive(true);
        NPCManager.instance.GenerateSmokeBomb(assignedThief.prop.transform.position);
        base.PrepareMission();
        ThiefMenu.counterpsy = assignedThief;
    }

    public override void StartMission()
    {
        counterspyCoroutine = CounterspyCheck();
        StartCoroutine(counterspyCoroutine);
        base.StartMission();
    }

    public override void Success(bool critical)
    {
        base.Success(critical);
    }

    public override void EndMission(bool autoFailure = false)
    {
        if (caughtThieves > 0) Success(false);
        else Failure();

        StopCoroutine(counterspyCoroutine);
        assignedThief.prop.SetActive(false);
        NPCManager.instance.GenerateSmokeBomb(assignedThief.prop.transform.position);
        ThiefMenu.counterpsy = null;
        thiefLocked = false;
        defChance = 0;
        caughtThieves = 0;
        budget.SetActive(true);
        timer.SetActive(false);

        assignedThief.thiefValues.thiefInMission = false;
        assignedThief.thiefValues.thiefMissionChance = 0;

        confirmButton.interactable = true;
        confirmButton.gameObject.SetActive(false);
    }

    IEnumerator CounterspyCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBeforeChecking);
            if (NPCManager.instance.banditsSeated.Count > 0)
            {
                //on fait un lancer de dé entre 0 et la chance maximale
                var diceRoll = Random.Range(0, 100);

                //si c'est un succés...
                if (diceRoll <= defChance)
                {
                    AlertPanel.instance.QueueMainAlert(banditSpottedSprite, banditSpottedText);
                    if (NPCManager.instance.banditsSeated.Count > caughtThieves) caughtThieves = NPCManager.instance.banditsSeated.Count;
                    foreach (Client bandit in NPCManager.instance.banditsSeated)
                    {
                        bandit.skin.material = bandit.clientData.banditMaterial;
                    }
                }
            }
        }
    }
}
