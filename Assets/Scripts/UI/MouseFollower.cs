using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseFollower : MonoBehaviour
{
    public static MouseFollower instance;
    public Image mouseHoldFiller, draggingImage;

    private void Awake()
    {
        instance = this;
        mouseHoldFiller.fillAmount = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        gameObject.transform.position = Input.mousePosition;
    }
}
