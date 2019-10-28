using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Gameplay", menuName = "Gameplay/New God", order = 2)]
public class God : ScriptableObject {

    public string name;
    public Color color;
    public Sprite sprite;
    public GameObject tavern;
}
