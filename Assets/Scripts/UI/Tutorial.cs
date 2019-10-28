using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface ITutorialable
{
    void StartTutorial();
}

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;
    public GameObject content;

    public RectTransform outline;
    private Transform originalParent, contentShown;
    public TextMeshProUGUI contentText, currentStepText, totalStepsText;

    private List<TutorialStep> steps = new List<TutorialStep>();
    private TutorialStep currentStep;

    private void Awake()
    {
        instance = this;
        if (content.activeSelf) content.SetActive(false);
    }

    public void StartTutorial(Transform givenContent, List<TutorialStep> givenSteps)
    {
        content.SetActive(true);
        Time.timeScale = 0;

        contentShown = givenContent;
        originalParent = contentShown.transform.parent;
        contentShown.SetParent(content.transform);
        contentShown.SetAsFirstSibling();

        steps = givenSteps;

        currentStep = steps[0];
        foreach (var item in steps)
        {
            if (item.objectToDisplay) item.objectToDisplay.SetActive(true);
        }
        totalStepsText.text = steps.Count.ToString();

        UpdateTutorial();
    }

    public void NextStep()
    {
        if (steps.IndexOf(currentStep) + 1 >= steps.Count)
        {
            StopTutorial();
            return;
        }
        else currentStep = steps[steps.IndexOf(currentStep) + 1];
        UpdateTutorial();
    }

    void UpdateTutorial()
    {
        outline.sizeDelta = currentStep.rect.sizeDelta;
        outline.position = currentStep.rect.position;
        contentText.text = currentStep.textToDisplay;
        currentStepText.text = (steps.IndexOf(currentStep) + 1).ToString();
    }

    void StopTutorial()
    {
        Time.timeScale = 1;

        foreach (var item in steps)
        {
            if (item.objectToDisplay) item.objectToDisplay.SetActive(false);
        }

        steps = null;
        currentStep = null;

        contentShown.SetParent(originalParent);
        contentShown = null;
        originalParent = null;

        content.SetActive(false);
    }
}
