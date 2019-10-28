using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCBank", menuName = "NPC/NPCBank", order = 1)]
public class NPCBank : ScriptableObject
{
    [Header("Human")]
    public Color humanColor;
    public Material humanSkin;
    public Sprite humanSprite;
    [Space(7)]

    [Header("Dwarf")]
    public Color dwarfColor;
    public Material dwarfSkin;
    public Sprite dwarfSprite;
    [Space(7)]

    [Header("Elf")]
    public Color elfColor;
    public Material elfSkin;
    public Sprite elfSprite;
    [Space(7)]

    [Header("Jottun")]
    public Color jottunColor;
    public Material jottunSkin;
    public Sprite jottunSprite;
    [Space(7)]

    public Color[] rarityColors;
}

