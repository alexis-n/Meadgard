using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Tooltipable : MonoBehaviour /*, IPointerEnterHandler, IPointerExitHandler*/
{
    [TextArea]
    public string nameTag = string.Empty;
    public string leftClick = string.Empty, holdClick = string.Empty, rightClick = string.Empty;
    public enum TooltipType { Nothing, Client, Employee, Thief, Alert, CurrentMissive, PredictedMissive, Furniture, Dish };
    public TooltipType type = TooltipType.Nothing;
    public Color rightClickColor = new Color(84, 72, 63);

    public void TooltipMe()
    {
        Tooltip.instance.UITooltip(this, nameTag, leftClick, holdClick, rightClick, rightClickColor);
    }
}
