using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{

    public Light sun;
    public float secondsInFullDay = 120f;
    [Range(0, 1)]
    public float currentTimeOfDay = 0;
    [HideInInspector]
    public float timeMultiplier = 1f;

    float sunInitialIntensity;
    float morningtime;

    void Start()
    {
        morningtime = currentTimeOfDay;
        sunInitialIntensity = sun.intensity;
        PhaseManager.instance.prepPhaseEvent.AddListener(ResetSun);
    }

    void Update()
    {
        UpdateSun();

        if (PhaseManager.instance.currentPhase == Data.CurrentPhase.Service && PhaseManager.instance.timeLeft > 0)
        {
            currentTimeOfDay += (Time.deltaTime / PhaseManager.instance.serviceDuration) * timeMultiplier;

            if (currentTimeOfDay >= 1)
            {
                currentTimeOfDay = 0;
            }
        }
    }

    void ResetSun()
    {
        currentTimeOfDay = morningtime;
        AudioManager.instance.PlayAmbient(AudioManager.instance.bank.dayAmbient);
    }

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
            if (AudioManager.instance.ambientSource.clip != AudioManager.instance.bank.nightAmbient)
                AudioManager.instance.PlayAmbient(AudioManager.instance.bank.nightAmbient);
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}