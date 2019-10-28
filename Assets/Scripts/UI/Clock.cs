using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Clock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;
    public RectTransform clock;
    public float nightRotation, morningRotation;
    public Image clockFill, tavernImage;
    public DOTweenAnimation[] gearsAnimations;

    /* Start is called before the first frame update
    void Update()
    {
        var minutes = 0;
        var seconds = 0;
        if (!NPCManager.instance.mustBeStopping)
        {
            minutes = Mathf.FloorToInt(PhaseManager.instance.timeLeft / 60f);
            seconds = Mathf.FloorToInt(PhaseManager.instance.timeLeft - minutes * 60);
        }
        else
        {
            minutes = 0;
            seconds = 0;
        }
        timer.text = minutes + ":" + seconds;
    }*/

    public void ResetClock()
    {
        clock.DORotate(new Vector3(0, 0, morningRotation), 1f);
        for (int i = 0; i < gearsAnimations.Length; i++) gearsAnimations[i].DORewind();
    }

    public void StopGears()
    {
        for (int i = 0; i < gearsAnimations.Length; i++) gearsAnimations[i].DOPause();
    }

    // Update is called once per frame
    public void StartRotatingClock()
    {
        for (int i = 0; i < gearsAnimations.Length; i++) gearsAnimations[i].DOPlay();
        clock.DORotate(new Vector3(0, 0, (nightRotation - morningRotation)), PhaseManager.instance.serviceDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
            .OnComplete(() => StopGears());
    }
}
