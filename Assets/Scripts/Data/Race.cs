using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay", menuName = "Gameplay/Race", order = 2)]
public class Race : ScriptableObject
{
    public new string name;
    public Color color;
    public Sprite sprite;
}
