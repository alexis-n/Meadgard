using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmployeeData", menuName = "Data/Employee")]
public class EmployeeData : InteractableObjectData
{
    [Header("Variables")]
    public int maxLevel = 3;
    public float timeBeforeSearching = 10f;
    public int[] levelThresholds, skillExpValues;
    public int skillPointsPerLevel = 2;
    [Space(7)]

    [Header("UI")]
    public string[] skillNames;
    public string[] featTexts;
    public Sprite[] tokens, skillIcons;
    public Color[] diamondColors, skillsColor;
    [TextArea]
    public string[] skillDesc;

    public AudioClip[] selectionSounds;
    public AudioClip[] interactionSounds;
}
