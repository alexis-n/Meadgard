using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CursorsBank", menuName = "UI/Cursors", order = 1)]
public class CursorsBank : ScriptableObject
{
    public Texture2D basic, drag, hover, rotate;
    public bool draggingElement = false;

    public void Basic()
    {
        if (UIManager.instance != null && !draggingElement || UIManager.instance == null)
            Cursor.SetCursor(basic, Vector2.zero, CursorMode.Auto);
    }

    public void Drag()
    {
        Cursor.SetCursor(drag, Vector2.zero, CursorMode.Auto);
        draggingElement = true;
    }
    public void EndDrag()
    {
        draggingElement = false;
        Basic();
    }

    public void Hover()
    {
        if (UIManager.instance != null && !draggingElement || UIManager.instance == null)
            Cursor.SetCursor(hover, Vector2.zero, CursorMode.Auto);
    }
}
