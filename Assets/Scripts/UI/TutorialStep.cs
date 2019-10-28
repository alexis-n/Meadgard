using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Step", menuName = "Gameplay/Tutorial Step")]
public class TutorialStep : MonoBehaviour
{
    public RectTransform rect;
    [TextArea]
    public string textToDisplay;
    public GameObject objectToDisplay;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (objectToDisplay) objectToDisplay.SetActive(false);
    }
}
