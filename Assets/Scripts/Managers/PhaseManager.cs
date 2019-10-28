using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager instance;

    public UnityEvent serviceEndEvent = new UnityEvent();
    public UnityEvent prepPhaseEvent = new UnityEvent();
    public UnityEvent servicePhaseEvent = new UnityEvent();
    public UnityEvent recapPhaseEvent = new UnityEvent();

    public Missive[] missives;

    public Data.CurrentPhase currentPhase;

    double countdownServicePhase = 300;
    double timeleftServicePhase = 180;
    double prepPhaseStartTimer = 5;

    public float timeLeft, serviceDuration = 300;

    int storedStartTime, storedDuration;
    public int turn = 1;

    bool checkpoint = false, firstTurn = true;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        turn = GameSaver.instance.gameSave.gameTurn;
        StartPreparationPhase();
    }

    void Update()
    {
        if (currentPhase == Data.CurrentPhase.Service)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0 && !checkpoint)
            {
                NPCManager.instance.StopService();
                checkpoint = true;
            }
        }
    }

    IEnumerator DelayedStart()
    {
        GameSaver.instance.ReadSave();
        yield return new WaitForSeconds(1);
        prepPhaseEvent.Invoke();
    }

    public void StartPreparationPhase ()
    {
        currentPhase = Data.CurrentPhase.Prep;

        if (firstTurn)
        {
            StartCoroutine(DelayedStart());
            firstTurn = false;
        }
        else
        {
            prepPhaseEvent.Invoke();
            turn++;
        }

        AlertPanel.instance.ClearAlerts();
        RandomMissive();
        timeLeft = serviceDuration;
        PlayerManager.instance.clientsMoney = 0;
        PlayerManager.instance.furnitureMoney = 0;
        PlayerManager.instance.employeeMoney = 0;
        PlayerManager.instance.restockMoney = 0;
        PlayerManager.instance.PlayerCleanliness = 100;
        AudioManager.instance.PlayPrepMusic();
        UIManager.instance.PreparationPhaseGUI();
    }
    public void StartServicePhase()
    {
        currentPhase = Data.CurrentPhase.Service;
        servicePhaseEvent.Invoke();
        AudioManager.instance.PlayServiceMusic();
        NPCManager.instance.ServicePhaseStart();
        checkpoint = false;
        UIManager.instance.ServicePhaseGUI();
    }
    public void StartRecapitulationPhase()
    {
        
        currentPhase = Data.CurrentPhase.Recap;
        recapPhaseEvent.Invoke();

        NPCManager.instance.cookOrdersList.Clear();
        NPCManager.instance.clientsInQueue.Clear();
        NPCManager.instance.clientsInRestaurant.Clear();
        NPCManager.instance.clientsWaitingForDish.Clear();

        UIManager.instance.RecapitulationPhaseGUI();
        AudioManager.instance.StopMusic();
    }

    public Missive RandomMissive()
    {
        if (Missive.predictedMissive == null) Missive.currentMissive = missives[Random.Range(0, missives.Length)];
        else
        {
            Missive.currentMissive = Missive.predictedMissive;
            Missive.predictedMissive = null;
        }

        if (PlayerManager.instance.tavern.god.name == "Odin")
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == 0 && Missive.predictedMissive != null)
                {
                    Missive.currentMissive = Missive.predictedMissive;
                    Missive.predictedMissive = null;
                }
                Missive.odinMissives[i] = missives[Random.Range(0, missives.Length)];
            }
        }

        return Missive.currentMissive;
    }
}
