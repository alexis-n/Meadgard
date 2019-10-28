using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioBank", menuName = "Audio/AudioBank", order = 1)]
public class AudioBank : ScriptableObject
{
    [Header("Music")]
    public AudioClip serviceMusic;
    public AudioClip prepMusic;
    [Space(7)]

    [Header("Ambient")]
    public AudioClip dayAmbient;
    public AudioClip nightAmbient, lowCrowdAmbient, highCrowdAmbient, fireplaceAmbient;
    [Space(7)]

    [Header("FX")]
    public AudioClip drinkFX;
    public AudioClip foodFX, moneyFX,
        hireEmployeeFX, fireEmployeeFX, levelupEmployeeFX,
        serviceStartFX, serviceEndFX,
        selectionPanelOpenFX, selectionPanelCloseFX,
        foodReadyFX, trashFoodFX, foodFalls, MixingMeadFX,
        BrokenGlassFX,
        upgradeFurniture,
        bifrostPop;
}