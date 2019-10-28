using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    public GameObject diamondObj;
    public RectTransform rect;
    public Image filler, icon, diamondColor;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
}
